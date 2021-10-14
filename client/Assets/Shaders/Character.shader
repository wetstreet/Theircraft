Shader "Custom/Character"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1, 1, 1, 1)
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
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Cutoff;
            float _Pow;
            float _Min;

            half4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                o.color = v.color.rgb;

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Cutoff);
                return half4(col.rgb * _Color.rgb, col.a);
            }

            ENDHLSL
        }
    }
}
