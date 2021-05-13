Shader "Custom/TextureArrayShader"
{
    Properties
    {
        _Array ("Texture", 2DArray) = "" {}
        _Slice ("Slice", Range(0, 460)) = 0
        _TileX ("Tile X", Float) = 1
        _TileY ("Tile Y", Float) = 1
        _Cutoff ("Cut Off", Range(0, 1)) = 0.5
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

            #pragma enable_d3d11_debug_symbols

            #include "Common.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                half4 color : COLOR;
            };

            TEXTURE2D_ARRAY(_Array);  SAMPLER(sampler_Array);
            float _Slice;
            float _TileX;
            float _TileY;
            float _Cutoff;
            
            sampler2D skyLightTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv.xy = v.uv.xy * float2(_TileX, _TileY);
                o.uv.z = v.vertex.w;
                o.uv.w = v.uv.z;
                o.color = v.color;

                o.worldNormal = TransformObjectToWorldNormal(v.normal);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = SAMPLE_TEXTURE2D_ARRAY(_Array, sampler_Array, i.uv.xy, i.uv.z) * i.color;

                // skylight
                float skylight = i.uv.w;
                float skyLightAtten = pow(skylight, 2 + 5 * skylight) + 0.002;
                col.rgb *= saturate(skyLightAtten);

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
