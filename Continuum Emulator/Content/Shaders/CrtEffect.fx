// CrtEffect.fx - CRT post effect built on your working SpriteTexture-based shader template.
// Works with Reach (vs_4_0_level_9_1 / ps_4_0_level_9_1) and OpenGL (vs_3_0 / ps_3_0).
//
// Features (single pass):
// - mild curvature
// - mild RGB bleed (3 taps)
// - scanlines
// - subtle vignette
// - rounded corners + feather
// - optional tiny noise (very cheap)
//
// Parameters are optional; sensible defaults are provided here.

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

// ===== Params you set from C# (defaults here) =====
float2 SourceSize = float2(320.0, 180.0);   // emulated buffer size
float2 OutputSize = float2(1280.0, 720.0);  // backbuffer size

float  Curvature = 0.12;         // 0..0.25
float  Bleed = 1.0;              // pixels (0..2)
float  ScanlineIntensity = 0.35; // 0..1
float  Vignette = 0.20;          // 0..1

float  CornerRadius = 0.06;      // half-height units (0..~0.2)
float  CornerFeather = 0.012;    // softness (0..~0.05)

float  NoiseIntensity = 0.0;     // 0..0.1 (optional)

static const float PI = 3.14159265;
static const float Epsilon = 1e-8;

// Very cheap noise (1 sin, 1 dot). Keep intensity low.
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

    // Aspect-correct radius for nicer curvature.
    p.x *= aspect;
    float r2 = dot(p, p);
    p *= (1.0 + Curvature * r2);
    p.x /= aspect;

    return p * 0.5 + 0.5;
}

// Rounded rectangle mask in half-height units (half-height=1, half-width=aspect).
float RoundedMask(float2 uv)
{
    float2 p = uv * 2.0 - 1.0;
    float aspect = OutputSize.x / max(OutputSize.y, 1.0);
    float2 sp = float2(p.x * aspect, p.y);

    float2 halfSize = float2(aspect, 1.0);
    float r = CornerRadius;

    // Signed distance to rounded box (uses length; ok on ps_3_0/9_1).
    float2 q = abs(sp) - (halfSize - r);
    float dist = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;

    // Feathered edge
    return 1.0 - smoothstep(0.0, max(CornerFeather, 1e-5), dist);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;

    // Warp
    float2 wuv = WarpUV(uv);

    // In-bounds mask (prevents sampling edge smear after warp)
    float2 inside = step(0.0, wuv) * step(wuv, 1.0);
    float inBounds = inside.x * inside.y;

    float2 texel = 1.0 / max(SourceSize, 1.0);

    // Mild RGB bleed (3 taps)
    float2 o = float2(Bleed * texel.x, 0.0);

    float r = tex2D(SpriteTextureSampler, wuv - o).r;
    float g = tex2D(SpriteTextureSampler, wuv).g;
    float b = tex2D(SpriteTextureSampler, wuv + o).b;

    float3 col = float3(r, g, b);

    // Scanlines locked to source Y
    float y = wuv.y * SourceSize.y;
    float scan = 0.85 + 0.15 * sin(y * PI);
    col *= lerp(1.0, scan, ScanlineIntensity);

    // Vignette in screen space (use warped UV so it "bows" with curvature)
    float2 p = wuv * 2.0 - 1.0;
    col *= saturate(1.0 - Vignette * dot(p, p));

    // Optional tiny noise
    if (NoiseIntensity > 0.0)
    {
        float n = rand_1_05(wuv * SourceSize);
        col += (n - 0.5) * NoiseIntensity;
        col = saturate(col);
    }

    // Rounded corners (in warped space so corners follow curvature)
    float alpha = RoundedMask(wuv) * inBounds;

    return float4(col, 1.0) * input.Color * alpha;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
