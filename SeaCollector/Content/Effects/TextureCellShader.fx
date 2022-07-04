#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;

float3 LightDirection = float3(0, -1, -1);

Texture2D Texture00 : register(t0);
sampler Sampler00 : register(s0)
{
	Texture = (Texture00);
	MinFilter = Point; // Minification Filter
    MagFilter = Point;// Magnification Filter
    MipFilter = Linear; // Mip-mapping
	AddressU = Mirror; // Address Mode for U Coordinates
	AddressV = Mirror; // Address Mode for V Coordinates
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 position = input.Position;
	float4 color = input.Color;
	float3 normals = input.Normal;
	float2 textureCoordinate = input.TextureCoordinate;

    float4 worldPosition = mul(position, World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Position = mul(viewPosition, Projection);
    output.Normal = mul(normals, World);
    output.Color = color;
    output.TextureCoordinate = textureCoordinate;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 Color = input.Color;
    float3 Normal = input.Normal;
    float3 N = normalize(Normal);
    float3 L = normalize(-LightDirection);
    if (dot(L, N) < 0.5 && dot(L, N) > 0.25) {
        Color.rgb *= 0.9;
    }else if (dot(L, N) < 0.25 && dot(L, N) > 0.0) {
        Color.rgb *= 0.7;
    } else if (dot(L, N) < 0.0) {
        Color.rgb *= 0.5;
    }
	return tex2D(Sampler00, input.TextureCoordinate) * Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};