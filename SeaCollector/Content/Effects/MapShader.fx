#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D Texture00;
sampler Sampler0;

Texture2D Texture01 : register(t1);
sampler Sampler1 : register(s1)
{
	Texture = (Texture01);
	MinFilter = Point; // Minification Filter
    MagFilter = Point;// Magnification Filter
    MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(Sampler0, input.TextureCoordinates) * input.Color;
    float value = color.r;
    if (value >= 0.2 && value <= 0.5) {
        color = float4(250, 255, 161, 255) / 255;
    }
    else if (value > 0.5) {
        color = float4(154, 235, 0, 255) / 255;
    }
    else {
        color = tex2D(Sampler1, input.TextureCoordinates * 8);// * float4(166, 255, 250, 255) / 255;
    }
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};