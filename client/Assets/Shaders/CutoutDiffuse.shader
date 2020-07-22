Shader "Custom/CutoutDiffuse"
{
    Properties
    {
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

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _CutOff;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                clip(color.a - _CutOff);

                half3 worldNormal = normalize(i.worldNormal);
                half3 lightDir = normalize(UnityWorldSpaceLightDir(i.pos));

                float diffuse = saturate(dot(worldNormal, lightDir));

                return fixed4(color.rgb * diffuse, color.a);
            }
            ENDCG
        }

    }

}