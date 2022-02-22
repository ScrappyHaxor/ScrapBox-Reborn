#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler s0;



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
	float4 AmbientColor = float4(1, 1, 1, 1);
	float AmbientIntensity = 0.1;
	AmbientColor = AmbientColor * AmbientIntensity;
	AmbientColor.a = 1;

	float4 color = tex2D(s0, input.TextureCoordinates);
	//return color;
	return color * AmbientColor;
}

technique Lights
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};