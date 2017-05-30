Shader "Custom/ExtendedStandardShader" {
	Properties {
		_Color ("Color", Color) = (1,0,0,1)
		_MainTex ("Albedo (RGB) or AlphaMap", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		//_AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5
		_Collision("Collision", Vector) = (0.0,0.0,0.0,0.0)
	}
	SubShader {
		Tags { "RenderType"="Cutout" }
		LOD 200
		
		// Render Front side 

		Cull Back

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		float4 _AlphaCutoff;
		float4 _Collision;
		float4 _StartTime;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 position = IN.uv_MainTex; // - float2(0, 0.5)); //+ float2(_Collision.x, _Collision.y);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, position) * _Color;
			//o.Albedo = c.rgb;
			float3 reflectionDir = reflect(-IN.viewDir, IN.worldNormal)*0.25f;
			o.Albedo = c*DotClamped(IN.viewDir,reflectionDir);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			if (_Collision.w > 0.2f) {
				clip(reflectionDir - _AlphaCutoff*0.5);
			}
		}
		ENDCG

		//Render Back side
		/*Cull Front

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;

		};

		half _Glossiness;
		half _Metallic;
		half _AlphaCutoff;
		float4 _Collision;
		float4 _StartTime;
		float animTime;

		fixed4 _Color;

		void vert(inout appdata_full v) {
			v.normal *= -1;
		}

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
			UNITY_INSTANCING_CBUFFER_END

			void surf(Input IN, inout SurfaceOutputStandard o) {
			float2 position = (IN.uv_MainTex - float2(0.5, 0.5)) - float2(_Collision.x, _Collision.y);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, position) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			if (_Collision.w > 0.2f) {
				clip(c.a - _AlphaCutoff);
			}
		}	
		ENDCG*/
		
	}
	FallBack "Diffuse"
}
