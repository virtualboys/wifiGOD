Shader "Custom/flatShader" {

	Properties {
		_difColor ("Diffuse Color", Color)  = (1, 1, 1, 1)
		_specColor ("Specular Color", Color)  = (1, 1, 1, 1)
		_ambColor ("Ambient Color", Color)  = (1, 1, 1, 1)
		_shininess ("Shininess", Range (0,200)) = 50
		_kD ("Diffuse Coefficient", Range (0,1)) = 0
		_kS ("Specular Coefficient", Range (0,1)) = 0
		_kA ("Ambient Coefficient", Range (0,1)) = .2
	}
	
    SubShader {
    
    	Tags { "RenderType" = "Opaque" }
    	
		CGPROGRAM
		#pragma surface surf FlatBlinnPhong
		
		half4 _difColor;
		half4 _specColor;
		half4 _ambColor;
		half _shininess;
		half _kD;
		half _kS;
		half _kA;

		half4 LightingFlatBlinnPhong (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
		
			half3 h = normalize (lightDir + viewDir);

			half3 diff = _kD * _difColor.rgb * max (0, dot (s.Normal, lightDir));

			float nh = max (0, dot (s.Normal, h));
			float3 spec = _kS * _specColor.rgb * pow (nh, 48.0);
			
			half3 amb = _kA * _ambColor.rgb;
			
			half4 c;
			c.rgb = _LightColor0.rgb * (atten * 2) * (amb + s.Albedo * diff + spec);
			c.a = s.Alpha;
			return c;
		}

		struct Input {
		  float2 uv_MainTex;
		};
		
		sampler2D _MainTex;
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		}
		
		ENDCG
		
		}
		
		Fallback "Diffuse"
		
}
