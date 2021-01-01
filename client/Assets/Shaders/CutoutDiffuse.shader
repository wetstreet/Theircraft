Shader "Custom/CutoutDiffuse"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _CutOff ("Cut Off", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode"="UniversalForward" "Queue"="AlphaTest" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            // #pragma multi_compile_fwdbase
            // #pragma multi_compile_fog

            #include "Common.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float _CutOff;
            half4 _Color;
            CBUFFER_END

            sampler2D _MainTex;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 texcoord     : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                // float3 worldNormal  : TEXCOORD1;
                // float3 worldPos     : TEXCOORD2;
                // SHADOW_COORDS(3)
                // UNITY_FOG_COORDS(4)
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;

                // output.worldNormal = TransformObjectToWorldNormal(v.normal);
                // output.worldPos = mul(unity_ObjectToWorld, v.vertex);

                // TRANSFER_SHADOW(o);
                // UNITY_TRANSFER_FOG(o, o.pos);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = tex2D(_MainTex, input.uv) * _Color;
                clip(color.a - _CutOff);

                return color;

                // half3 worldNormal = normalize(i.worldNormal);
                // half3 lightDir = normalize(UnityWorldSpaceLightDir(i.pos));

                // UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

                // float diff = saturate(dot(worldNormal, lightDir));
                // fixed3 diffuse = color.rgb;

                // fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * color.rgb;
                // float3 c = diffuse;

                // UNITY_APPLY_FOG(i.fogCoord, c);

                // return half4(c, color.a);
            }
            ENDHLSL
        }

    }

}