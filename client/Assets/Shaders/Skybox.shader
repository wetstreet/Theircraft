// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Skybox" {
    Properties {
        _Height1 ("Height 1", Float) = 1000
        _Height2 ("Height 2", Float) = 2000
    }

    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Common.hlsl"

            half4 _SkyTopColor;
            half4 _SkyBottomColor;

            float _SkyHeight;
            float _SkyTransition;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                return lerp(_SkyBottomColor, _SkyTopColor, smoothstep(_SkyHeight, _SkyHeight + _SkyTransition, i.worldPos.y));
                // float val = saturate(i.uv.y);
                // return lerp(_Color2, _Color, pow(val, 0.5f));
                // return pow(i.uv.y, 0.5f);
            }

            ENDHLSL
        }
    }
}
