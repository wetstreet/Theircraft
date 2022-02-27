Shader "Custom/Cloud"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue="="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Tags{ "LightMode" = "SRPDefaultUnlit" }
            ZWrite On
            ColorMask 0
        }

        Pass
        {
            Tags{ "LightMode" = "UniversalForward" }
            ZTest LEqual
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma enable_d3d11_debug_symbols

            #include "Common.hlsl"

            struct appdata
            {
                float3 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float vertexDistance : TEXCOORD2;
            };

            half4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);

                o.vertexDistance = length((mul(UNITY_MATRIX_MV, float4(v.vertex, 1)).xyz));

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = _Color;
                col.rgb *= _SkyLightColor.rgb;

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

                col = linear_fog(col, i.vertexDistance);
                
                return col;
            }
            ENDHLSL
        }
    }
}
