#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
	#define HIGH_PRECISION highp
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

// Define CRT geometry correction parameters
float2 CRTCenter = float2(0.5, 0.5); // Center of CRT distortion
float CRTRadius = 0.5; // Radius of CRT distortion
float CRTStrength = 0.02; // Strength of CRT distortion

struct VertexShaderInput
{
	float4 Position : POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Color = input.Color;
	output.TextureCoordinates = input.TextureCoordinates;

	// Calculate the distance from the CRTCenter
	float2 offset = input.Position.xy - CRTCenter;

	// Apply CRT distortion based on distance from CRTCenter
	float dist = length(offset);
	if (dist < CRTRadius)
	{
		float distortion = sin(dist / CRTRadius * 3.14159265 * 2.0) * CRTStrength;
		offset *= (1.0 + distortion);
	}

	// Apply the CRT distortion to the vertex position
	output.Position = float4(offset + CRTCenter, input.Position.zw);

	return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
	float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	return col;
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};