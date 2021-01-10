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
            // make fog work
            #pragma multi_compile_fog

            #include "Common.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
            };

            TEXTURE2D_ARRAY(_Array);  SAMPLER(sampler_Array);
            float _Slice;
            float _TileX;
            float _TileY;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv.xy = v.uv * float2(_TileX, _TileY);
                o.uv.z = v.vertex.w;
                o.color = v.color;
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = SAMPLE_TEXTURE2D_ARRAY(_Array, sampler_Array, i.uv.xy, i.uv.z) * i.color;
                clip(col.a - _Cutoff);
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDHLSL
        }
    }
}
