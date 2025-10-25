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
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    // Calculate the distance from the center of the screen
    float2 center = float2(0.5, 0.5); // Assuming the center of the screen is at (0.5, 0.5)
    float distance = length(input.TextureCoordinates - center);

    // Adjust the parameters to control the vignette effect
    float vignetteRadius = 5.0;    // Adjust the radius of the vignette
    float vignetteIntensity = 17.0; // Adjust the intensity of the vignette

    // Apply the vignette effect
    col.rgb *= 1.0 - smoothstep(0.0, vignetteRadius, distance) * vignetteIntensity;

    return col;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};