Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color Tint", Color) = (0.24706, 0.46275, 0.89412, 1)
        _AlphaBoost ("Alpha Boost", Range(0, 0.5)) = 0
        _Angle("旋转角度", Range(-180, 180)) = 0
        _WaterTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 0.70588
        _Speed ("Speed", Range(10, 20)) = 15
    }
    SubShader
    {
        Tags { "LightMode"="UniversalForward" "Queue"="Transparent" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM

            // #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Common.hlsl"

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                half4 light : TEXCOORD3;
                float vertexDistance : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            float _AlphaBoost;
            float4 _WaterTex_ST;
            float _Speed;
            float _Angle;
            float _Alpha;
            CBUFFER_END

            sampler2D _WaterTex;

            sampler2D _DayLightTexture;
            sampler2D _NightLightTexture;
            float _DayNight01;

            float2 Rotate(float2 input)
            {
                
                //uv值的区间是（0，1），所以中心点就是（0.5，0.5）
                float center = float2(0.5, 0.5);
                //将uv坐标移到中心
                float2 uv = input.xy - center;
                //输入的_Angle值为了方便理解设定为-360-360.  经过转换后的angle值为（-pi * 2） - （pi*2）区间
                float angle = _Angle * (3.14 * 2 / 360);
                //矩阵旋转
                uv = float2(uv.x * cos(angle) - uv.y * sin(angle),
                    uv.x * sin(angle) + uv.y * cos(angle));
                
                //还原uv坐标
                uv += float2(0.5, 0.5);
                return uv;
            }

            v2f vert (a2v v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _WaterTex);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                float blockLight = 0;
                float skylight = 1;
                half4 dayLight = tex2Dlod(_DayLightTexture, half4(blockLight, 1 - skylight, 0, 0));
                half4 nightLight = tex2Dlod(_NightLightTexture, half4(blockLight, 1 - skylight, 0, 0));
                o.light = lerp(dayLight, nightLight, saturate(_DayNight01));

                o.vertexDistance = length((mul(UNITY_MATRIX_MV, v.vertex).xyz));

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 uv = Rotate(i.uv);
                uv.y /= 32;
                int index = (_Time.y * _Speed) % 32;
                uv.y += index / 32.f;

                // sample the texture
                half4 col = 0;
                col.rgb = tex2D(_WaterTex, uv).a * _Color.rgb;
                col.rgb *= i.light.rgb;
                col.a = _Alpha;

                col = linear_fog(col, i.vertexDistance);

                return col;
            }

            ENDHLSL
        }
    }
}
