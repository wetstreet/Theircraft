Shader "Custom/HighlightShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Lightness("Lightness", Range(1,2)) = 1
	}

	SubShader{
		Pass{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct v2f {
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};
			sampler2D _MainTex;
			float4 _Color;
			float _Lightness;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 o = tex2D(_MainTex,IN.uv) * _Lightness * _Color;
				return o;
			}

			ENDCG
		}
	}
	FallBack "Unlit/Texture"
}
