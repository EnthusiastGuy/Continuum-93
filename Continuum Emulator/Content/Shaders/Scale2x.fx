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
    MinFilter = Point;
    MagFilter = Point;
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

// source size in pixels (original, before 2x)
float2 SourceSize = float2(320.0, 180.0);
// output size in pixels (SourceSize * 2)
float2 OutputSize = float2(640.0, 360.0);

static const float3 LUMA = float3(0.299, 0.587, 0.114);

// “are these colors similar?”
// (small threshold avoids false edges from tiny color noise)
bool Similar(float3 a, float3 b)
{
    float d = abs(dot(a - b, LUMA));
    return d < 0.02; // tweak if needed
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Identify which output pixel we are (in integer coords)
    float2 outPix = floor(input.TextureCoordinates * OutputSize);

    // Map to source pixel (integer)
    float2 srcPix = floor(outPix * 0.5);

    // Subpixel within 2x2 block (0 or 1)
    float2 sub = outPix - srcPix * 2.0; // (0/1, 0/1)

    float2 invSrc = 1.0 / max(SourceSize, 1.0);
    float2 baseUV = (srcPix + 0.5) * invSrc;

    // Neighbors (Scale2x notation)
    float3 E = tex2D(SpriteTextureSampler, baseUV).rgb;                     // center
    float3 B = tex2D(SpriteTextureSampler, baseUV + float2(0, -invSrc.y)).rgb; // up
    float3 D = tex2D(SpriteTextureSampler, baseUV + float2(-invSrc.x, 0)).rgb; // left
    float3 F = tex2D(SpriteTextureSampler, baseUV + float2(invSrc.x, 0)).rgb;  // right
    float3 H = tex2D(SpriteTextureSampler, baseUV + float2(0, invSrc.y)).rgb;  // down

    // Scale2x rule:
    // if (B != H && D != F) then
    //  E0 = (D==B) ? D : E
    //  E1 = (B==F) ? F : E
    //  E2 = (D==H) ? D : E
    //  E3 = (H==F) ? F : E
    // else all E

    bool bh = !Similar(B, H);
    bool df = !Similar(D, F);

    float3 outc = E;

    if (bh && df)
    {
        // sub.x: 0 left, 1 right; sub.y: 0 top, 1 bottom
        if (sub.y < 0.5)
        {
            // top row
            outc = (sub.x < 0.5)
                ? (Similar(D, B) ? D : E)   // E0
                : (Similar(B, F) ? F : E);  // E1
        }
        else
        {
            // bottom row
            outc = (sub.x < 0.5)
                ? (Similar(D, H) ? D : E)   // E2
                : (Similar(H, F) ? F : E);  // E3
        }
    }

    return float4(outc, 1.0) * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
