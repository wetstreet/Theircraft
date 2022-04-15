Shader "Custom/TextureArrayShader"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _Array ("Texture", 2DArray) = "" {}
        _Slice ("Slice", Range(0, 460)) = 0
        _TileX ("Tile X", Float) = 1
        _TileY ("Tile Y", Float) = 1
        _Cutoff ("Cut Off", Range(0, 1)) = 0.5
        // _DayLightTexture ("_DayLightTexture", 2D) = "white" {}
        // _NightLightTexture ("_NightLightTexture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull [_Culling]

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ DEBUG_AO

            #pragma enable_d3d11_debug_symbols

            #include "Common.hlsl"

            struct appdata
            {
                float3 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 worldNormal : TEXCOORD1;
                half4 color : COLOR;
                half4 light : TEXCOORD2;
                float vertexDistance : TEXCOORD3;
            };

            sampler2D _MainTex;
            TEXTURE2D_ARRAY(_Array);  SAMPLER(sampler_Array);
            float _Slice;
            float _TileX;
            float _TileY;
            float _Cutoff;

            sampler2D _DayLightTexture;
            sampler2D _NightLightTexture;
            float _DayNight01;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv.xy = v.uv.xy * float2(_TileX, _TileY);
                o.color = v.color;

                o.worldNormal.xyz = TransformObjectToWorldNormal(v.normal);

                // skylight
                float skylight = v.uv1.x;
                half blockLight = v.uv1.y;

                o.vertexDistance = length((mul(UNITY_MATRIX_MV, float4(v.vertex, 1)).xyz));

                half4 dayLight = tex2Dlod(_DayLightTexture, half4(blockLight, 1 - skylight, 0, 0));
                half4 nightLight = tex2Dlod(_NightLightTexture, half4(blockLight, 1 - skylight, 0, 0));

                o.light = lerp(dayLight, nightLight, saturate(_DayNight01));

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = tex2D(_MainTex, i.uv.xy);

                clip(col.a - _Cutoff);

                col.rgb *= i.color.rgb;

            #if DEBUG_AO
                return half4(i.light.rgb, 1);
            #endif

                col.rgb *= i.light.rgb;

                col.rgb *= GetNL(i.worldNormal);

                col = linear_fog(col, i.vertexDistance);

                return col;
            }

            ENDHLSL
        }
    }
}
