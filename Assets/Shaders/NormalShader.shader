// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NormalShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	    _NormalMap("NormalMap",2D) = "white"{}
		_BumpScale ("BumpScale", Range(0,1)) = 0.5
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(0.8,256)) = 10
	}
	SubShader{

			Pass{
				Tags{"LightMode" = "ForwardBase"}

				CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #include "Lighting.cginc"

                fixed4 _Color;
		        sampler2D _MainTex;
				float4 _MainTex_ST;
		        sampler2D _NormalMap;
				float4 _NormalMap_ST;
				float _BumpScale;
				float4 _Specular;
				float _Gloss;

				struct a2v {
					float4 vertex:POSITION;
					float3 normal:NORMAL;
					float4 tangent:TANGENT;
					float4 texcoord:TEXCOORD0;

				};
				struct v2f {
					float4 pos:SV_POSITION;
					float4 uv:TEXCOORD0;
					float3 lightDir:TEXCOORD1;
					float3 viewDir:TEXCOORD2;
				};
				
				v2f vert(a2v v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw; //保证矩阵转换后 tiling  offset 正确
					o.uv.zw = v.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw; 
					float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)); //副切线
					float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal); //构造切线空间转换矩阵
					o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;  //把光照方向转换到切线空间
					o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;  //把视角方向转换到切线空间
					return o;
				}

				fixed4 frag(v2f i):SV_Target{
					fixed3 tangentLightDir = normalize(i.lightDir);
					fixed3 tangentViewDir = normalize(i.viewDir);
					fixed4 packedNormal = tex2D(_NormalMap, i.uv.zw);//法线贴图像素采样
					fixed3 tangentNormal = UnpackNormal(packedNormal);//反映射获取法线
					tangentNormal.xy *= _BumpScale;
					tangentNormal.z = sqrt(1 - saturate(dot(tangentNormal.xy, tangentNormal.xy))); //向量叉乘获取互相垂直的第三条向量 
					fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
					fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));
					fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
					fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(tangentNormal, halfDir)), _Gloss);
					return fixed4(ambient + diffuse + specular, 1.0);
				}

			ENDCG
	   }
		
	}
	FallBack "Diffuse"
}
