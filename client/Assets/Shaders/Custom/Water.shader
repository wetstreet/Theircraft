Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _AlphaBoost ("Alpha Boost", Range(0, 0.5)) = 0
        _Angle("旋转角度", Range(-180, 180)) = 0
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Range(10, 20)) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                SHADOW_COORDS(4)
            };

            fixed4 _Color;
            float _AlphaBoost;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            int _Index;

            float _Angle;
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

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = Rotate(i.uv);
                uv.y /= 32;
                int index = (_Time.y * _Speed) % 32;
                uv.y += index / 32.f;

                // sample the texture
                fixed4 col = tex2D(_MainTex, uv) * _Color;

                col.a += _AlphaBoost;

                half3 worldNormal = normalize(i.worldNormal);
                half3 lightDir = normalize(UnityWorldSpaceLightDir(i.vertex));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

                float diff = saturate(dot(worldNormal, lightDir));
                fixed3 diffuse = col.rgb * diff;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * col.rgb;
                float3 c = diffuse * atten + ambient;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, c);
                return fixed4(c, _Color.a);
            }
            ENDCG
        }
    }
}
