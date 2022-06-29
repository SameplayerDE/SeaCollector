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
float3 PlayerPosition;

float Delta;
float Total;

float3 DiffuseColor = float3(1, 1, 1);
float3 AmbientColor = float3(0.1, 0.1, 0.1) * 6;
float3 LightDirection = float3(1, 1, 0);
float3 LightColor = float3(0.9, 0.9, 0.9);
float SpecularPower = 256;
float3 SpecularColor = float3(1, 1, 1);

float3 FogCenter;

float FogStart = 5;
float FogEnd = 10;
float3 FogColor = float3(1, 1, 1);

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
	float4 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;
	float4 Position3D : TEXCOORD2;
	float3 ViewDirection : TEXCOORD3;
	float3 FogDirection : TEXCOORD5;
	float Distance : TEXCOORD4;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	float4 position = input.Position;
	float4 color = input.Color;
	float3 normals = input.Normal;
	float2 textureCoordinate = input.TextureCoordinate;

    float4 worldPosition = mul(position, World);
    
    worldPosition.y += cos(Total + worldPosition.x + worldPosition.y) * 0.05;
    
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
    output.Normal = mul(normals, World);
    output.Color = color;
    output.TextureCoordinate = textureCoordinate;
	output.Position3D = mul(position, World);
	output.ViewDirection = worldPosition - CameraPosition;
	output.FogDirection = worldPosition - FogCenter;
	output.Distance = sqrt(pow(PlayerPosition.x - worldPosition.x, 2) + pow(PlayerPosition.z - worldPosition.z, 2));

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = input.Color;
    
    float3 lighting = AmbientColor;
    float3 lightDir = normalize(LightDirection);
    float3 normal = normalize(input.Normal);
    
    // Add lambertian lighting
    lighting += saturate(dot(lightDir, normal)) * LightColor;
    float3 refl = reflect(lightDir, normal);
    float3 view = normalize(input.ViewDirection);
    
     // Add specular highlights
    lighting += pow(saturate(dot(refl, view)), SpecularPower) * 
    SpecularColor;
    
    float distance = saturate(input.Distance / 25);
    
    float alpha = 1 - distance;
    
    // Calculate final color
    float3 output = saturate(lighting) * color;

    float dist = length(input.FogDirection);
    float fog = clamp((dist - FogStart) / (FogEnd - FogStart), 0, 1);
    return float4(lerp(output, FogColor, fog), 1);
}


technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
		CullMode = CW;
		FillMode = Solid;
		ZEnable = true;
        ZWriteEnable = true;
	}
};