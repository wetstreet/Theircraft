// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RimlightShader" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Degree("Degree", Range(0,0.5)) = 0.07
		_OutlineColor("OutlineColor", color) = (0,0,0,1)
	}
		SubShader{
			Pass {
				Cull Front
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				struct v2f {
					float4 vertex:POSITION;
				};

				float _Degree;
				half4 _OutlineColor;

				v2f vert(appdata_full v)
				{
					v2f o;
					float4 v_vertex = mul(UNITY_MATRIX_MV, v.vertex);
					float3 v_normal = mul(UNITY_MATRIX_IT_MV, v.normal);
					v_vertex.xyz += normalize(v_normal) * _Degree;
					o.vertex = mul(UNITY_MATRIX_P, v_vertex);
					return o;
				}

				half4 frag(v2f IN) :COLOR
				{
					return _OutlineColor;
				}
				ENDCG
		   }

    	Pass{
	            Cull Back
	            CGPROGRAM

	            #pragma vertex vert
	            #pragma fragment frag
	            #include "UnityCG.cginc"

	            struct v2f {
		        	float4 vertex:POSITION;
					float4 uv : TEXCOORD0;
            	};
				sampler2D _MainTex;

				v2f vert(appdata_full v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					return o;
				}

				half4 frag(v2f IN):COLOR
				{
					half4 c = tex2D(_MainTex,IN.uv);
					return c;
				}

            	ENDCG
		   }
		}
    	FallBack "Diffuse"
}
