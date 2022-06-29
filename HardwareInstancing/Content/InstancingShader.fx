#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 WVP;
texture cubeTexture;

sampler TextureSampler = sampler_state
{
    texture = <cubeTexture>;
    mipfilter = LINEAR;
    minfilter = LINEAR;
    magfilter = LINEAR;
};

struct InstancingVSinput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct InstancingVSoutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

InstancingVSoutput InstancingVS(InstancingVSinput input, float4 instanceTransform : POSITION1, 
                                float2 atlasCoord : TEXCOORD1)
{
    InstancingVSoutput output;

    float4 pos = input.Position + instanceTransform;
    pos = mul(pos, WVP);

    output.Position = pos;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 InstancingPS(InstancingVSoutput input) : COLOR0
{
    return tex2D(TextureSampler, input.TexCoord);
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