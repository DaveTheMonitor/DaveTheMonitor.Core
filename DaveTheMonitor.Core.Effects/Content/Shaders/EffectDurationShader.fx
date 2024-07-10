#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

extern const texture2D BackgroundTexture;
extern const texture2D IconTexture;
extern const float Progress;
extern const float2 ScreenSize;

sampler2D BackgroundTextureSampler = sampler_state
{
	Texture = <BackgroundTexture>;
};

sampler2D IconTextureSampler = sampler_state
{
    Texture = <IconTexture>;
};

struct VSIn
{
    float4 Position : SV_POSITION;
    float2 TexCoord0 : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
};

struct VSOut
{
	float4 Position : SV_POSITION;
    float2 TexCoord0 : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
};

VSOut MainVS(VSIn input)
{
    VSOut output;
    float2 pos = input.Position.xy / ScreenSize;
    pos -= float2(0.5, 0.5);
    pos.x *= 2;
    pos.y *= -2;
    output.Position = float4(pos, 0, 1);
    output.TexCoord0 = input.TexCoord0;
    output.TexCoord1 = input.TexCoord1;
    return output;
}

float4 MainPS(VSOut input) : COLOR
{
    float4 bgColor = tex2D(BackgroundTextureSampler, input.TexCoord0);
    float4 iconColor = tex2D(IconTextureSampler, input.TexCoord1);
    float4 color = iconColor.a > bgColor.a ? iconColor : bgColor;
    color.rgb = 0;
    color.a *= 0.75;
	return color;

}

technique SpriteDrawing
{
	pass P0
	{
        VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};