// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Skybox" {
    Properties {
        _Height1 ("Height 1", Float) = 1000
        _Height2 ("Height 2", Float) = 2000
    }

    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off
        ZTest Always

        Pass {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Common.hlsl"

            half4 _SkyColor;
            float _SkyFogEnd;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float vertexDistance : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.vertexDistance = length((mul(UNITY_MATRIX_MV, v.vertex).xyz));

                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                return linear_fog(_SkyColor, i.vertexDistance, 0, _SkyFogEnd);
            }

            ENDHLSL
        }
    }
}
