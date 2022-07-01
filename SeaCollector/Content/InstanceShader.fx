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

float3 DiffuseColor = float3(1, 1, 1);
float3 AmbientColor = float3(0.1, 0.1, 0.1) * 6;
float3 LightDirection = float3(1, 1, 0);
float3 LightColor = float3(0.9, 0.9, 0.9);
float SpecularPower = 256;
float3 SpecularColor = float3(1, 1, 1);

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
	float3 ViewDirection : TEXCOORD2;
};

VertexShaderOutput InstancingVS(VertexShaderInput input, float4 instanceTransform : POSITION1, float3 rotation : TEXCOORD3)
{
    VertexShaderOutput output;

    float4 position = input.Position + instanceTransform;
	float4 color = input.Color;
	float3 normals = input.Normal;
	float2 textureCoordinate = input.TextureCoordinate;

    float4 worldPosition = position;
    float4 viewPosition = mul(worldPosition, View) * rotation;
    
    output.Position = mul(viewPosition, Projection);
    output.Normal = mul(normals, World);
    output.Color = color;
    output.TextureCoordinate = textureCoordinate;
    
	output.ViewDirection = worldPosition - CameraPosition;
    return output;
}

float4 InstancingPS(VertexShaderOutput input) : COLOR0
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
    lighting += pow(saturate(dot(refl, view)), SpecularPower) * SpecularColor;
    
    // Calculate final color
    float3 output = saturate(lighting) * color;
    
    return float4(output, 1);
}

technique Instancing
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL InstancingVS();
        PixelShader = compile PS_SHADERMODEL InstancingPS();
        CullMode = NONE;
    }
}