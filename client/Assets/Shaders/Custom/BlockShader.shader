Shader "Custom/BlockShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
        _LUT ("LUT", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _LUT;

            float _Cutoff;
            float _Pow;
            float _Min;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                //逐顶点光照
				// fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// fixed3 worldLight = normalize(WorldSpaceLightDir(v.vertex));
                // o.light = saturate(dot(worldNormal, worldLight));

                o.color = v.color.rgb;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return 1;
                float3 light = tex2D(_LUT, i.color.xy).rgb;
                // return fixed4(light, 1);
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Cutoff);
                return fixed4(col.rgb * light, col.a);
            }
            ENDCG
        }
    }
}
