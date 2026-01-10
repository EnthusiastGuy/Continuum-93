#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture   = <SpriteTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// ===== Params (set from C#; defaults here) =====
float2 SourceSize = float2(320.0, 180.0);
float2 OutputSize = float2(1280.0, 720.0);

float  Curvature = 0.12;         // 0..0.25
float  Bleed = 1.0;              // pixels
float  ScanlineIntensity = 0.35; // 0..1
float  Vignette = 0.20;          // 0..1

float  CornerRadius = 0.06;      // half-height units (0..~0.2)
float  CornerFeather = 0.012;    // softness (0..~0.05)

// Feather along the entire border (UV units).
// Set from C# as: edgePx / min(OutputW, OutputH)
float  EdgeFeather = 0.003;      // ~2px at 720p

// “Not pitch black” outside the tube area (within the drawn quad)
float  BorderGlow = 1.0;         // 0..1
float3 BorderColor = float3(0.02, 0.02, 0.025); // very dark bluish gray

// Monochrome “green monitor” mode
float Monochrome = 0.0; // 0 = normal, 1 = full green
float3 MonoTint = float3(0.20, 1.00, 0.35); // phosphor green tint
float MonoGamma = 1.65; // >1 = darker mids, <1 = brighter mids
float MonoGain = 1.65; // overall brightness boost in mono mode

float ScanlineScale = 1.0;


float  NoiseIntensity = 0.0;     // optional tiny noise (0..0.08)

static const float PI = 3.14159265;

// cheap noise
float rand_1_05(in float2 uv)
{
    float n = sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453;
    return frac(n);
}

// Barrel distortion in UV space, aspect-correct.
float2 WarpUV(float2 uv)
{
    float2 p = uv * 2.0 - 1.0; // [-1..1]
    float aspect = OutputSize.x / max(OutputSize.y, 1.0);

    p.x *= aspect;
    float r2 = dot(p, p);
    p *= (1.0 + Curvature * r2);
    p.x /= aspect;

    return p * 0.5 + 0.5;
}

// Rounded rectangle mask in half-height units.
float RoundedMask(float2 uv)
{
    float2 p = uv * 2.0 - 1.0;
    float aspect = OutputSize.x / max(OutputSize.y, 1.0);
    float2 sp = float2(p.x * aspect, p.y);

    float2 halfSize = float2(aspect, 1.0);
    float r = CornerRadius;

    float2 q = abs(sp) - (halfSize - r);
    float dist = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;

    return 1.0 - smoothstep(0.0, max(CornerFeather, 1e-5), dist);
}

// Feather across the entire border (also gracefully handles uv outside [0..1])
float EdgeMask(float2 uv)
{
    // distance to closest edge in UV; goes negative if outside [0..1]
    float2 d = min(uv, 1.0 - uv);
    float m = min(d.x, d.y);
    return smoothstep(0.0, max(EdgeFeather, 1e-6), m);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv  = input.TextureCoordinates;
    float2 wuv = WarpUV(uv);

    // Sample safely (don’t pull black from outside the texture after warp)
    float2 suv = saturate(wuv);

    float2 texel = 1.0 / max(SourceSize, 1.0);

    // RGB bleed (3 taps)
    float2 o = float2(Bleed * texel.x, 0.0);
    float3 col;
    col.r = tex2D(SpriteTextureSampler, suv - o).r;
    col.g = tex2D(SpriteTextureSampler, suv).g;
    col.b = tex2D(SpriteTextureSampler, suv + o).b;

    // Scanlines locked to source Y
    float y = suv.y * (SourceSize.y * ScanlineScale);
    float scan = 0.85 + 0.15 * sin(y * PI);
    col *= lerp(1.0, scan, ScanlineIntensity);

    // Vignette (use warped coords so it “bows” with curvature)
    float2 p = wuv * 2.0 - 1.0;
    col *= saturate(1.0 - Vignette * dot(p, p));

    // Optional tiny noise
    if (NoiseIntensity > 0.0)
    {
        float n = rand_1_05(suv * SourceSize);
        col = saturate(col + (n - 0.5) * NoiseIntensity);
    }

    // Combine corner mask + straight-edge feather
    float tube = RoundedMask(wuv) * EdgeMask(wuv);

    // --- Green monitor conversion (applied to the image area only) ---
    if (Monochrome > 0.0)
    {
    // Luma
        float l = dot(col, float3(0.299, 0.587, 0.114));
        l = saturate(l + l * l * 0.25); // lifts bright parts more than dark parts
    // Simple “phosphor response” curve
        l = pow(saturate(l), MonoGamma);

        float3 mono = l * MonoTint;

    // Blend normal -> mono
        col = lerp(col, mono, saturate(Monochrome));
    }

    
    // Instead of hard black outside, blend to a very dark “tube glow”
    float3 outCol = lerp(BorderColor, col, tube);
    
    if (Monochrome > 0.0)
    {
        float l = dot(col, float3(0.299, 0.587, 0.114));

    // Gentler response curve (lower gamma = brighter mids)
        l = pow(saturate(l), MonoGamma);

    // Boost brightness, clamp
        l = saturate(l * MonoGain);

        float3 mono = l * MonoTint;
        col = lerp(col, mono, saturate(Monochrome));
    }

    return float4(outCol, 1.0) * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
