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

            #include "Common.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                half4 color : COLOR;
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
                o.uv.zw = v.texcoord.zw;

                o.color = v.color;
                o.worldNormal = TransformObjectToWorldNormal(v.normal);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv.xy);

                i.uv.w = 1.f;
                // skylight
                float skylight = i.uv.w;
                // col.rgb *= GetSkyLight(skylight);

                if (i.worldNormal.y == 1)
                {
                    col.rgb *= 1;
                }
                else if (i.worldNormal.z == 1 || i.worldNormal.z == -1)
                {
                    col.rgb *= 0.6;
                }
                else if (i.worldNormal.x == 1 || i.worldNormal.x == -1)
                {
                    col.rgb *= 0.3;
                }
                else if (i.worldNormal.y == -1)
                {
                    col.rgb *= 0.2;
                }

                clip(col.a - _Cutoff);

                return col;
            }

            ENDHLSL
        }
    }
}
