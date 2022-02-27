#ifndef COMMON_INCLUDED
#define COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"


half4 _SkyLightColor;
half3 GetSkyLight(half skylight)
{
    float skyLightAtten = pow(skylight, 2 + 5 * skylight) + 0.002;
    return saturate(skyLightAtten) * _SkyLightColor.rgb;
}


float _FogStart;
float _FogEnd;
float4 _FogColor;

float4 linear_fog(float4 inColor, float vertexDistance, float fogStart, float fogEnd)
{
    if (vertexDistance <= fogStart)
    {
        return inColor;
    }

    float fogValue = vertexDistance < fogEnd ? smoothstep(fogStart, fogEnd, vertexDistance) : 1.0;
    return float4(lerp(inColor.rgb, _FogColor.rgb, fogValue * _FogColor.a), inColor.a);
}

float4 linear_fog(float4 inColor, float vertexDistance)
{
    return linear_fog(inColor, vertexDistance, _FogStart, _FogEnd);
}


#endif // COMMON_INCLUDED