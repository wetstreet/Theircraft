Shader "Custom/BumpShader" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_Bump ("Bump", 2D) = "bump"{}
		_Snow ("Range",Range(0,1)) = 0.2
		_SnowColor("SnowColor",Color) = (1.0,1.0,1.0)
		_SnowDirection("SnowDirection",vector) = (0,1,0)
		_SnowDepth("Depth",Range(0,0.5)) = 0.2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		//#pragma surface surf Standard fullforwardshadows
        #pragma surface surf Lambert vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
	    sampler2D _Bump;
		float4 _SnowColor;
		float4 _SnowDirection;
		float _Snow;
		float _SnowDepth;
		struct Input {
			float2 uv_MainTex;
			float2 uv_Bump;
			float3 worldNormal;
			INTERNAL_DATA
		};
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
			if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) > lerp(1, -1, _Snow))
				o.Albedo = _SnowColor.rgb;
			else 
			    o.Albedo = c.rgb;
		    o.Alpha = c.a;
		}

		void vert(inout appdata_full v)
		{
			float4 snow = mul(UNITY_MATRIX_IT_MV, _SnowDirection);
			if (dot(v.normal, snow.xyz) >= lerp(1,-1,_Snow))
			{
				v.vertex.xyz += (snow.xyz + v.normal)*_SnowDepth*_Snow;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
