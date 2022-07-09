#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 View;
float4x4 Projection;
texture ParticleTexture;

bool AlphaTest = true;
bool AlphaTestGreater = true;
float AlphaTestValue = 0.5f;

sampler2D texSampler = sampler_state {
 texture = <ParticleTexture>;
};
float2 Size;
float3 Up; // Camera's 'up' vector
float3 Side; // Camera's 'side' vector

struct VertexShaderInput
{
 float4 Position : POSITION0;
 float2 UV : TEXCOORD0;
};
struct VertexShaderOutput
{
 float4 Position : POSITION0;
 float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input, float4 instanceTransform : POSITION1)
{
    VertexShaderOutput output;
    float4 position = input.Position + instanceTransform;
    // Determine which corner of the rectangle this vertex
    // represents
    float2 offset = float2((input.UV.x - 0.5f) * 2.0f, -(input.UV.y - 0.5f) * 2.0f);
    // Move the vertex along the camera's 'plane' to its corner
    position.xyz += offset.x * Size.x * Side + offset.y * Size.y * Up;
    // Transform the position by view and projection
    output.Position = mul(position, mul(View, Projection));
    output.UV = input.UV;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(texSampler, input.UV);
    if (AlphaTest)
        clip((color.a - AlphaTestValue) * (AlphaTestGreater ? 1 : -1));
    
    return color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}