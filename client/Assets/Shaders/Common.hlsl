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


#endif // COMMON_INCLUDED