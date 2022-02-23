Shader "Custom/BreakParticleShader"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _Cutoff ("Cut Off", Range(0, 1)) = 0.5
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Cull [_Culling]

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #pragma enable_d3d11_debug_symbols

            #include "Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"        

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D_ARRAY(_Array);  SAMPLER(sampler_Array);
            sampler2D _MainTex;
            float _Slice;
            float _Cutoff;
            half4 _Color;
            float _SkyLight;
            float _BlockLight;

            sampler2D _DayLightTexture;
            sampler2D _NightLightTexture;
            float _DayNight01;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Cutoff);

                half4 dayLight = tex2D(_DayLightTexture, half2(_BlockLight, 1 - _SkyLight));
                half4 nightLight = tex2D(_NightLightTexture, half2(_BlockLight, 1 - _SkyLight));

                col.rgb *= lerp(dayLight.rgb, nightLight.rgb, saturate(_DayNight01));

                // color tint
                col.rgb *= LinearToGamma22(_Color.rgb);

                return col;
            }

            ENDHLSL
        }
    }
}
