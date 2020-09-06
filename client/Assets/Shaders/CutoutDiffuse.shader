Shader "Custom/CutoutDiffuse"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _CutOff ("Cut Off", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode"="ForwardBase" "Queue"="AlphaTest" }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float _CutOff;
            fixed4 _Color;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
                UNITY_FOG_COORDS(4)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 color = tex2D(_MainTex, i.uv) * _Color;
                clip(color.a - _CutOff);

                half3 worldNormal = normalize(i.worldNormal);
                half3 lightDir = normalize(UnityWorldSpaceLightDir(i.pos));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

                float diff = saturate(dot(worldNormal, lightDir));
                fixed3 diffuse = color.rgb * diff;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * color.rgb;
                float3 c = diffuse * atten + ambient;

                UNITY_APPLY_FOG(i.fogCoord, c);

                return fixed4(c, color.a);
            }
            ENDCG
        }

    }

}