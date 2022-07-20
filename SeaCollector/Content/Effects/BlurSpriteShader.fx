#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

Texture2D SpriteTexture;
float TextureWidth = 256 * 1;
float TextureHeight = 192 * 1;

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

// Blurs using a 3x3 filter kernel
float4 BlurFunction3x3(VertexShaderOutput input) : COLOR0
{
    float3x3 filter = float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

    // TOP ROW
    float4 s11 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / TextureWidth, -1.0f / TextureHeight));    // LEFT
    float4 s12 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -1.0f / TextureHeight));              // MIDDLE
    float4 s13 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(1.0f / TextureWidth, -1.0f / TextureHeight)); // RIGHT
    
    // MIDDLE ROW
    float4 s21 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / TextureWidth, 0));             // LEFT
    float4 s22 = tex2D(SpriteTextureSampler, input.TextureCoordinates);                                          // DEAD CENTER
    float4 s23 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / TextureWidth, 0));                 // RIGHT
    
    // LAST ROW
    float4 s31 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / TextureWidth, 1.0f / TextureHeight)); // LEFT
    float4 s32 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 1.0f / TextureHeight));                   // MIDDLE
    float4 s33 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(1.0f / TextureWidth, 1.0f / TextureHeight));  // RIGHT
 
    if (false) {
        s11 = (0.2126 * s11.r + 0.7152 * s11.g + 0.0722 * s11.b); 
        s12 = (0.2126 * s12.r + 0.7152 * s12.g + 0.0722 * s12.b); 
        s13 = (0.2126 * s13.r + 0.7152 * s13.g + 0.0722 * s13.b);
        
        s21 = (0.2126 * s21.r + 0.7152 * s21.g + 0.0722 * s21.b); 
        s22 = (0.2126 * s22.r + 0.7152 * s22.g + 0.0722 * s22.b); 
        s23 = (0.2126 * s23.r + 0.7152 * s23.g + 0.0722 * s23.b); 
        
        s31 = (0.2126 * s31.r + 0.7152 * s31.g + 0.0722 * s31.b); 
        s32 = (0.2126 * s32.r + 0.7152 * s32.g + 0.0722 * s32.b); 
        s33 = (0.2126 * s33.r + 0.7152 * s33.g + 0.0722 * s33.b);
    }
 
    // Average the color with surrounding samples
    s11 *= filter._11;
    s12 *= filter._12;
    s13 *= filter._13;
    
    s21 *= filter._21;
    s22 *= filter._22;
    s23 *= filter._23;
    
    s31 *= filter._31;
    s32 *= filter._32;
    s33 *= filter._33;
    
    s11 = abs(s11);
    s12 = abs(s12);
    s13 = abs(s13);
    
    s21 = abs(s21);
    s22 = abs(s22);
    s23 = abs(s23);
    
    s31 = abs(s31);
    s32 = abs(s32);
    s33 = abs(s33);
    
    float value = 0;
    
    for (int x = 0; x < 3; x++) {
        for (int y = 0; y < 3; y++) {
            float filterValue = filter[x][y];
            if (filterValue > 0) {
                value += filterValue;
            }
        }
    }
    
    float4 result  = (s11 + s12 + s13 + s21 + s22 + s23 + s31 + s32 + s33) / value;
    return result;
}

// A 3x3 emboss filter kernel
float4 EmbossShaderFunction3x3(VertexShaderOutput input) : COLOR0
{
  float4 s22 = tex2D(SpriteTextureSampler, input.TextureCoordinates); // center
  float4 s11 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / 1024.0f, -1.0f / 768.0f));
  float4 s33 = tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(1.0f / 1024.0f, 1.0f / 768.0f));
 
  s11.rgb = (s11.r + s11.g + s11.b);
  s22.rgb = (s22.r + s22.g + s22.b) * -0.5;
  s33.rgb = (s22.r + s22.g + s22.b) * 0.2;
 
 
    float4 result = (s11 + s22 + s33);
    if (result.r >= 1) {
        return result;
    }
    return float4(0, 0, 0, 1);
    return tex2D(SpriteTextureSampler, input.TextureCoordinates) - saturate(s11 + s22 + s33);
}

// Outputs edges only using a A 3x3 edge filter kernel
float4 OutlinesFunction3x3(VertexShaderOutput input) : COLOR0
{
  float4 lum = float4(0.30, 0.59, 0.11, 1);
 
  // TOP ROW
  float s11 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / 1024.0f, -1.0f / 768.0f)), lum);   // LEFT
  float s12 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -1.0f / 768.0f)), lum);             // MIDDLE
  float s13 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(1.0f / 1024.0f, -1.0f / 768.0f)), lum);    // RIGHT
 
  // MIDDLE ROW
  float s21 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / 1024.0f, 0)), lum);                // LEFT
  // Omit center
  float s23 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / 1024.0f, 0)), lum);                // RIGHT
 
  // LAST ROW
  float s31 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-1.0f / 1024.0f, 1.0f / 768.0f)), lum);    // LEFT
  float s32 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 1.0f / 768.0f)), lum);              // MIDDLE
  float s33 = dot(tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(1.0f / 1024.0f, 1.0f / 768.0f)), lum); // RIGHT
 
  // Filter ... thanks internet <img draggable="false" role="img" class="emoji" alt="🙂" src="https://s0.wp.com/wp-content/mu-plugins/wpcom-smileys/twemoji/2/svg/1f642.svg">
  float t1 = s13 + s33 + (2 * s23) - s11 - (2 * s21) - s31;
  float t2 = s31 + (2 * s32) + s33 - s11 - (2 * s12) - s13;
 
  float4 col;
 
  if (((t1 * t1) + (t2 * t2)) > 0.05) {
  col = float4(0,0,0,1);
  } else {
    col = float4(1,1,1,1);
  }
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(SpriteTextureSampler,input.TextureCoordinates);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};