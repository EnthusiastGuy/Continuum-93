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
	float2 TextureCoordinates: TEXCOORD0;
};

float Epsilon = 1e-10;

// Define CRT geometry correction parameters
float2 CRTCenter = float2(0.5, 0.5); // Center of CRT distortion
float CRTRadius = 0.5; // Radius of CRT distortion
float CRTStrength = 0.02; // Strength of CRT distortion


float3 RGBtoHCV(in float3 RGB)
{
	// Based on work by Sam Hocevar and Emil Persson
	float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
	float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
	return float3(H, C, Q.x);
}

// Function to convert RGB to HSV (Hue, Saturation, Value)
float3 RGBtoHSV(in float3 RGB)
{
	float3 HCV = RGBtoHCV(RGB);
	float S = HCV.y / (HCV.z + Epsilon);
	return float3(HCV.x, S, HCV.z);
}

float3 HUEtoRGB(in float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float3(R, G, B));
}

float3 HSVtoRGB(in float3 HSV)
{
	float3 RGB = HUEtoRGB(HSV.x);
	return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

float rand_1_05(in float2 uv)
{
	float2 noise = (frac(sin(dot(uv, float2(12.9898, 78.233) * 2.0)) * 43758.5453));
	return abs(noise.x + noise.y) * 0.5;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

	// Hue
	// Rotate the hue (0.0 - 1.0)
	//float hueRotationAngle = 2.0;
	//float3 hsv = RGBtoHSV(col.rgb);
	//hsv.x = fmod(hsv.x + hueRotationAngle, 1.0); // Rotate the hue
	//col.rgb = HSVtoRGB(hsv);
	// End hue

	// Brightness
	// Adjust brightness (<1.0 is less bright, 0.0 is normal, >0.0 is brighter)
	//float brightness = -0.3;
	//col.rgb += brightness;
	// End brightness

	// Contrast
	// Adjust contrast (0.0 is lower contrast, 1.0 is normal, >1.0 is higher contrast)
	//float contrast = 1.9;
	//col.rgb = (col.rgb - 0.5) * contrast + 0.5;
	// End contrast

	// Saturation
	// Adjust saturation (0.0 is grayscale, 1.0 is normal, >1.0 is more saturated)
	//float saturation = 1.0;

	// Convert to grayscale
	//float grayscale = dot(col.rgb, float3(0.3, 0.59, 0.11));

	// Interpolate between grayscale and original color based on saturation
	//col.rgb = lerp(grayscale, col.rgb, saturation);
	// End saturation


	// Grayscale
	// Calculate grayscale value by averaging RGB channels
	//float grayscale = (col.r + col.g + col.b) / 3.0;

	// Set the RGB values to the grayscale value
	//col.rgb = float3(grayscale, grayscale, grayscale);

	// Paper effect
	// Calculate random brightness offset using your noise function
	float randomOffset = rand_1_05(input.TextureCoordinates.xy);

	// Apply the random offset to each color channel
	col.rgb += randomOffset * 0.06; // Adjust for intensity

	// Clamp the color channels to keep them within the [0, 1] range
	col.rgb = saturate(col.rgb);

	// Interlace
	//if (fmod(input.TextureCoordinates.y * 540, 2.0) < 1.0) {
	//	col.rgb *= 0.8;
	//}

	// Vignette
	// Calculate the distance from the center of the screen
	//float2 center = float2(0.5, 0.5); // Assuming the center of the screen is at (0.5, 0.5)
	//float distance = length(input.TextureCoordinates - center);

	// Adjust the parameters to control the vignette effect
	//float vignetteRadius = 5.0;    // Adjust the radius of the vignette
	//float vignetteIntensity = 17.0; // Adjust the intensity of the vignette

	// Apply the vignette effect
	//col.rgb *= 1.0 - smoothstep(0.2, vignetteRadius, distance) * vignetteIntensity;

	return col;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};