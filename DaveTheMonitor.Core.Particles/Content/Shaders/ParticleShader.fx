#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

extern const matrix View;
extern const matrix Projection;
extern const matrix World;
extern const float4 FogColor;
extern const float3 CameraPos;
extern const float FogStart;
extern const float FogEnd;
extern const float SkyStart;
extern const float SkyEnd;
extern const texture Tex;
sampler3D TexSampler = sampler_state
{
    Texture = <Tex>;
};

struct VSIn
{
	float4 Position : POSITION0;
};

struct VSOut
{
	float4 Position : SV_POSITION;
    float4 WorldPosition : POSITION1;
    float4 Color : COLOR0;
    float3 TexCoord : TEXCOORD0;
};

void CalcFog(inout float4 color, float4 worldPos)
{
    float d = distance(CameraPos, worldPos.xyz);
    
    float fd = saturate((d - FogStart) / (FogEnd - FogStart));
    color.rgb = lerp(color.rgb, FogColor.rgb, fd * FogColor.a);
    
    float fade = 1 - saturate((d - SkyStart) / (SkyEnd - SkyStart));
    color *= fade;
}

float4x4 Billboard(float3 objPos, float3 cameraPos)
{
    //float4x4 m2;
    //m2[0][0] = -0.998019457;
    //m2[0][1] = 0;
    //m2[0][2] = 0.06290539;
    //m2[0][3] = 0;
    //m2[1][0] = -0.06284021;
    //m2[1][1] = 0.0455127247;
    //m2[1][2] = -0.996985257;
    //m2[1][3] = 0;
    //m2[2][0] = -0.002862996;
    //m2[2][1] = -0.9989638;
    //m2[2][2] = -0.0454225875;
    //m2[2][3] = 0;
    //m2[3][0] = 0.327722639;
    //m2[3][1] = 0;
    //m2[3][2] = -6.35758352;
    //m2[3][3] = 1;
    
    //return m2;
    
    float3 up = float3(0.0, 1.0, 0.0);
    float4x4 m;
    
    float3 d = objPos - cameraPos;
    float l = length(d);
    if (l * l < 0.0001)
    {
        d = float3(0, 0, -1);
    }
    else
    {
        float f = (1.0 / l);
        d.x *= f;
        d.y *= f;
        d.z *= f;
    }
    float3 right = normalize(cross(up, d));
    float3 forward = cross(d, right);
    
    m[0][0] = right.x;
    m[0][1] = right.y;
    m[0][2] = right.z;
    m[0][3] = 0;
    
    m[1][0] = forward.x;
    m[1][1] = forward.y;
    m[1][2] = forward.z;
    m[1][3] = 0;
    
    m[2][0] = d.x;
    m[2][1] = d.y;
    m[2][2] = d.z;
    m[2][3] = 0;
    
    m[3][0] = objPos.x;
    m[3][1] = objPos.y;
    m[3][2] = objPos.z;
    m[3][3] = 1;
    return m;
}

VSOut DefaultVS(in VSIn input, float3 position : POSITION1, float2 size : TEXCOORD0, float4 color : COLOR0, float3 texCoord0 : TEXCOORD1, float3 texCoord1 : TEXCOORD2)
{
    VSOut output;

    float4x4 m = Billboard(position, CameraPos);
    float4 p = input.Position;
    p.x *= size.x * 2;
    p.y *= size.y * 2;
    
    float4 worldPosition = mul(mul(p, World), m);
    float4 viewPosition = mul(worldPosition, View);
    float4 pos = mul(viewPosition, Projection);
    float2 pos2 = float2(-input.Position.xy) + float2(0.5, 0.5);
    float3 texCoord = texCoord0 + ((texCoord1 - texCoord0) * float3(pos2, texCoord0.z));
    
    output.Position = pos;
    output.Color = color;
    output.TexCoord = texCoord;
    output.WorldPosition = worldPosition;

    return output;
}

float4 AlphaTestPS(VSOut input) : COLOR
{
    float4 color = tex3D(TexSampler, input.TexCoord);
    clip(color.a - 0.05);
    color.rgba *= input.Color.rgba;
    
    CalcFog(color, input.WorldPosition);
    
    return color;
}

float4 AlphaTest_NoFogPS(VSOut input) : COLOR
{
    float4 color = tex3D(TexSampler, input.TexCoord);
    clip(color.a - 0.05);
    color.rgba *= input.Color.rgba;
    
    return color;
}

float4 OpaquePS(VSOut input) : COLOR
{
    float4 color = tex3D(TexSampler, input.TexCoord);
    color.rgb *= input.Color.rgb;
    
    CalcFog(color, input.WorldPosition);
    
    return color;
}

float4 Opaque_NoFogPS(VSOut input) : COLOR
{
    float4 color = tex3D(TexSampler, input.TexCoord);
    color.rgb *= input.Color.rgb;
    
    return color;
}

float4 AlphaBlendPS(VSOut input) : COLOR
{
    float4 color = tex3D(TexSampler, input.TexCoord);
    clip(color.a - 0.05);
    color.rgba *= input.Color.rgba;
    
    CalcFog(color, input.WorldPosition);
    
    return color;
}

float4 AlphaBlend_NoFogPS(VSOut input) : COLOR
{
    float4 color = tex3D(TexSampler, input.TexCoord);
    clip(color.a - 0.05);
    color.rgba *= input.Color.rgba;
    
    return color;
}

technique AlphaTest
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL DefaultVS();
		PixelShader = compile PS_SHADERMODEL AlphaTestPS();
	}
};
technique AlphaTest_NoFog
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL DefaultVS();
        PixelShader = compile PS_SHADERMODEL AlphaTest_NoFogPS();
    }
};
technique Opaque
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL DefaultVS();
        PixelShader = compile PS_SHADERMODEL OpaquePS();
    }
};
technique Opaque_NoFog
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL DefaultVS();
        PixelShader = compile PS_SHADERMODEL Opaque_NoFogPS();
    }
};
technique AlphaBlend
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL DefaultVS();
        PixelShader = compile PS_SHADERMODEL AlphaBlendPS();
    }
};
technique AlphaBlend_NoFog
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL DefaultVS();
        PixelShader = compile PS_SHADERMODEL AlphaBlend_NoFogPS();
    }
};