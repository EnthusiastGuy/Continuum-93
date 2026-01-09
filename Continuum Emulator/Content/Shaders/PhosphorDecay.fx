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
    Texture = <SpriteTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D HistoryTexture;
sampler2D HistoryTextureSampler = sampler_state
{
    Texture = <HistoryTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// 0..1. Higher = longer trails.
// Typical: 0.85 (short) .. 0.97 (long)
float Decay = 0.92;

// How much the current frame “wins” over history.
// Keep at 1 for crispness; lower makes it more smeary.
float CurrentGain = 1.0;

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;

    float3 curr = tex2D(SpriteTextureSampler, uv).rgb * CurrentGain;
    float3 prev = tex2D(HistoryTextureSampler, uv).rgb * Decay;

    // Persistence model: keep bright values around.
    // max() gives a very CRT-like “linger” without ghosting dark areas.
    float3 outc = max(curr, prev);

    return float4(outc, 1.0) * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
