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

// ===== Params =====
float2 SourceSize = float2(320.0, 180.0);

float Time = 0.0;          // seconds
float Strength = 1.0;      // master 0..1

float ChromaShift = 1.25;  // pixels (typ 0.8..2.0)  -> color delay/bleed
float LumaSmear = 0.85;    // pixels (typ 0.4..1.6)  -> horizontal luma blur

float Jitter = 0.45;       // pixels (typ 0.1..1.0)  -> per-line horizontal wobble
float Wobble = 0.20;       // pixels (typ 0.0..0.6)  -> slow vertical-ish wobble (implemented as x wobble)
float Noise = 0.10;        // 0..0.35                -> tape grain
float Frame = 0.0;                          // increments by 1 each frame
float2 NoiseSeed = float2(0.0, 0.0);        // randomized per frame (0..1)
float Dropouts = 0.12;     // 0..0.35                -> occasional line dropouts
float HeadSwitch = 0.35;   // 0..1                   -> bottom noise band strength

static const float3 LUMA = float3(0.299, 0.587, 0.114);

// cheap hash noise
float hash12(float2 p)
{
    // "hash without sine" style (less patterned than sin(dot()))
    float3 p3 = frac(float3(p.x, p.y, p.x) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}

// RGB <-> YIQ (good for analog artifacts; cheap)
float3 RGBtoYIQ(float3 c)
{
    float Y = dot(c, LUMA);
    float I = dot(c, float3(0.596, -0.275, -0.321));
    float Q = dot(c, float3(0.212, -0.523,  0.311));
    return float3(Y, I, Q);
}

float3 YIQtoRGB(float3 y)
{
    float Y = y.x, I = y.y, Q = y.z;
    return float3(
        Y + 0.956 * I + 0.621 * Q,
        Y - 0.272 * I - 0.647 * Q,
        Y - 1.106 * I + 1.703 * Q
    );
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float2 inv = 1.0 / max(SourceSize, 1.0);

    // ----- timebase error / jitter (horizontal, line-dependent) -----
    float y = uv.y * SourceSize.y;

    // Per-line random + a little periodic wobble
    float rn = hash12(float2(floor(y), Frame) + NoiseSeed * 4096.0);
    float wob = sin(Time * 1.7 + uv.y * 6.0) * Wobble;

    float jitterPx = (rn - 0.5) * 2.0 * Jitter + wob;
    float xJit = jitterPx * inv.x;

    // Slight “tearing” band (occasional larger shift)
    float tear = step(0.985, hash12(float2(floor(Time * 3.0), floor(y * 0.25))));
    xJit += tear * (hash12(float2(y, Time)) - 0.5) * 8.0 * inv.x * Strength;

    float2 uvt = uv + float2(xJit, 0.0);

    // ----- Sample base color -----
    float3 c0 = tex2D(SpriteTextureSampler, uvt).rgb;

    // ----- Luma smear (horizontal blur on Y only, keeps chroma nastier like VHS) -----
    float lpx = LumaSmear * inv.x * Strength;
    float3 cL = tex2D(SpriteTextureSampler, uvt - float2(lpx, 0)).rgb;
    float3 cR = tex2D(SpriteTextureSampler, uvt + float2(lpx, 0)).rgb;

    float3 y0 = RGBtoYIQ(c0);
    float3 yL = RGBtoYIQ(cL);
    float3 yR = RGBtoYIQ(cR);

    // blur luminance a bit, keep I/Q from center for now
    float Y = (yL.x + 2.0 * y0.x + yR.x) * 0.25;

    // ----- Chroma shift / bleed (delay I/Q horizontally) -----
    float cpx = ChromaShift * inv.x * Strength;
    float3 cC = tex2D(SpriteTextureSampler, uvt - float2(cpx, 0)).rgb; // delayed chroma
    float3 yiqC = RGBtoYIQ(cC);

    // Light chroma lowpass (very VHS)
    float3 cC2 = tex2D(SpriteTextureSampler, uvt + float2(cpx * 0.6, 0)).rgb;
    float3 yiqC2 = RGBtoYIQ(cC2);

    float I = lerp(yiqC.y, (yiqC.y + yiqC2.y) * 0.5, 0.35);
    float Q = lerp(yiqC.z, (yiqC.z + yiqC2.z) * 0.5, 0.35);

    // Compose
    float3 outc = YIQtoRGB(float3(Y, I, Q));

    // ----- Mild vertical “line” shading (not CRT scanlines; more like VHS luma ripple) -----
    float ripple = 1.0 - 0.05 * Strength * (0.5 + 0.5 * sin(uv.y * 180.0 + Time * 8.0));
    outc *= ripple;

    // ----- Tape noise (grain + speckle) -----
    float n = hash12(uv * SourceSize + NoiseSeed * 4096.0 + Frame * 0.07);
    float g = (n - 0.5) * 2.0 * Noise * Strength;
    outc += g;

    // occasional bright specks
    float speck = step(0.9975, n) * 0.35 * Noise * Strength;
    outc += speck;

    // ----- Dropouts (brief horizontal desat/dark line corruption) -----
    float dline = step(0.992, hash12(float2(floor(y * 0.5), Frame) + NoiseSeed * 2048.0));
    float dmask = dline * Dropouts * Strength;
    float lum = saturate(dot(outc, LUMA));
    outc = lerp(outc, lum.xxx * 0.6, dmask);

    // ----- Head switching noise band (bottom) -----
    // band height ~ 6% of picture
    float band = smoothstep(0.92, 1.0, uv.y);
    if (band > 0.0)
    {
        float bn = hash12(float2(uv.x * SourceSize.x * 2.0, Frame) + NoiseSeed * 8192.0);
        float b = (bn - 0.5) * 2.0;
        outc += b * 0.25 * HeadSwitch * Strength * band;

        // add some horizontal tearing inside band
        outc *= 1.0 - 0.10 * HeadSwitch * Strength * band;
    }

    outc = saturate(outc);
    return float4(outc, 1.0) * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
