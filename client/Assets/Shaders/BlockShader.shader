Shader "Custom/BlockShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma enable_d3d11_debug_symbols

            #include "Common.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                half4 color : COLOR;
                float vertexDistance : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Cutoff;

            half4 _Color;

            sampler2D _DayLightTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = v.texcoord1;

                o.color = v.color;
                o.worldNormal = TransformObjectToWorldNormal(v.normal);

                o.vertexDistance = length(mul(UNITY_MATRIX_MV, v.positionOS).xyz);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv.xy);

                clip(col.a - _Cutoff);

                // float skylight = i.uv.z;
                // col.rgb *= GetSkyLight(skylight);
                
                float3 lightDir = float3(-0.44029, 0.69466, -0.56855);

                float nl = max(dot(i.worldNormal, lightDir), 0.2);

                col.rgb *= nl;
                col.rgb *= i.color.rgb;

                col = linear_fog(col, i.vertexDistance);

                return col;
            }

            ENDHLSL
        }
    }
}
