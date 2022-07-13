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

texture ParticleTexture;
sampler2D texSampler = sampler_state {
 texture = <ParticleTexture>;
};
float Time;
float Lifespan;
float2 Size;
float3 Wind;
float3 Up;
float3 Side;
float FadeInTime;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
    float3 Direction : TEXCOORD1;
    float Speed : TEXCOORD2;
    float StartTime : TEXCOORD3;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float2 UV : TEXCOORD0;
    float2 RelativeTime : TEXCOORD1;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
 VertexShaderOutput output;
 float3 position = input.Position;
 // Move to billboard corner
 float2 offset = Size * float2((input.UV.x - 0.5f) * 2.0f, 
 -(input.UV.y - 0.5f) * 2.0f);
 position += offset.x * Side + offset.y * Up;
 // Determine how long this particle has been alive
 float relativeTime = (Time - input.StartTime);
 output.RelativeTime = relativeTime;

 position += (input.Direction * input.Speed + Wind) * relativeTime;
 // Transform the final position by the view and projection matrices
 output.Position = mul(float4(position, 1), mul(View, Projection));
 output.UV = input.UV;
 return output;
}


float4 MainPS(VertexShaderOutput input) : COLOR0
{
 // Ignore particles that aren't active
 clip(input.RelativeTime);
 // Sample texture
 float4 color = tex2D(texSampler, input.UV);
 // Fade out towards end of life
 float d = clamp(1.0f - pow((input.RelativeTime / Lifespan), 10), 
 0, 1);
 // Fade in at beginning of life
 d *= clamp((input.RelativeTime / FadeInTime), 0, 1);
 // Return color * fade amount
 return float4(color * d);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};