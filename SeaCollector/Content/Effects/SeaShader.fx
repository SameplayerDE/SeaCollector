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

float3 CameraPosition;

float Delta;
float Total;

float FogStart = 5;
float FogEnd = 10;
float3 FogColor = float3(78, 202, 255) / 255;

    
   
    
float scale = 4.0;
float2 Offset;

Texture2D Texture : register(t0);
sampler TextureSampler : register(s0)
{
	Texture = (Texture);
	MinFilter = Point; // Minification Filter
    MagFilter = Point;// Magnification Filter
    MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureUVs : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureUVs : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 Color : COLOR0;
	float3 ViewDirection : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	float4 position = input.Position;
	float2 textureUVs = input.TextureUVs;
	float4 color = float4(1, 1, 1, 1);
    float3 normals = input.Normal;
    
    float4 worldPosition = mul(position, World);
    
    //worldPosition.x += sin(Total + worldPosition.x + worldPosition.y) * 0.05;
    //worldPosition.y += cos(Total + worldPosition.x + worldPosition.y) * 0.05;
    
    float4 viewPosition = mul(worldPosition, View);

    
    textureUVs *= scale;
    
    textureUVs += Offset / (20 / scale);
    //viewPosition /= 10;
    //textureUVs.x += sin(Total + worldPosition.x + worldPosition.y) * 0.05;
    //textureUVs.y += cos(Total + worldPosition.x + worldPosition.y) * 0.05;
    //textureUVs.x += sin(Total + viewPosition.x + viewPosition.y) * 0.05;
    //textureUVs.y += cos(Total + viewPosition.x + viewPosition.y) * 0.05;
    
    output.Position = mul(viewPosition, Projection);
    output.Normal = mul(normals, World);
    output.Color = color;
    output.TextureUVs = textureUVs;
	output.ViewDirection = worldPosition - CameraPosition;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = input.Color;
    color *= tex2D(TextureSampler, input.TextureUVs);
    
    float dist = length(input.ViewDirection);
    float fog = clamp((dist - FogStart) / (FogEnd - FogStart), 0, 1);
    return float4(lerp(float3(color.rgb), FogColor, fog), 1);
    
	//return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
		CullMode = NONE;
	}
};