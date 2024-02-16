Shader "Zololgo/PlanetGen | Examples/Standard Planet Ramp Texture" {
	Properties {
		_AtmosphereColor ("Atmosphere Color", Color) = (1,1,1,0)
		_RimColor ("Rim Color", Color) = (0.05,0.11,0.20,1)
		_Ramp ("Color Ramp (vertical)", 2D) = "blue" {}	
		_HeightMap ("HeightMap (RGB)", 2D) = "black" {}		
		_BumpStrength ("Bump strength", Range (0.00, 0.1)) = 0.1
		_Shininess ("Water Shininess", Range (0.01, 1)) = 0.4
		_CloudColor ("Cloud Color", Color) = (1,1,1,1)
		_CloudScale ("Uniform Cloud Scale", Range (0.1, 2)) = 1
		_CloudTex ("Cloud Texture", 2D) = "black" {}
	}
	SubShader { 

		CGPROGRAM
		#pragma surface surf Standard vertex:vert noforwardadd nodirlightmap novertexlights nolightmap nodynlightmap 
		#pragma target 3.0

		struct Input {
            float2 uv_HeightMap;
			half4 localPos;
			half3 localNormal;		
			float3 viewDir;			
        };

		half4 _AtmosphereColor;
		half _BumpStrength, _Shininess;
		half4 _CloudTex_ST, _CloudColor;
		sampler2D _CloudTex;
		half _CloudScale;
		half4 _RimColor;
					
        sampler2D _HeightMap, _Ramp;
        float4 _HeightMap_TexelSize;
        
		void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            v.normal = normalize(v.vertex.xyz);
			o.localPos = v.vertex;
			o.localNormal = v.normal;			
        }
		
		void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float poles = abs(IN.uv_HeightMap.y*2-1);
			float t = tex2D(_HeightMap, IN.uv_HeightMap).x;
			float tx = tex2D (_HeightMap, IN.uv_HeightMap+ float2(_HeightMap_TexelSize.x,0)).x-t;
			float ty = tex2D (_HeightMap, IN.uv_HeightMap+ float2(0,_HeightMap_TexelSize.y)).x-t;

			half corrBumpStrength = lerp(0.1*_BumpStrength*_HeightMap_TexelSize.z,0,poles);
			float3 normal = cross(float3(1, 0, tx * corrBumpStrength),float3(0, 1, ty * corrBumpStrength));
            normal = normalize(normal);

			float4 c = tex2D(_Ramp, half2(0.5,t.x));
			float s = c.a;
			normal = lerp (half3(0.0,0.0,1), normal, s);
			
			fixed4 c0  = tex2D(_CloudTex, IN.localPos.xz *_CloudScale * _CloudTex_ST.xy+_CloudTex_ST.zw);
			fixed4 c1 = tex2D(_CloudTex, IN.localPos.xy *_CloudScale * (IN.localPos.z < 0 ? -_CloudTex_ST.xy : _CloudTex_ST.xy)+_CloudTex_ST.zw);
			fixed4 c2 = tex2D(_CloudTex, IN.localPos.zy *_CloudScale * (IN.localPos.x > 0 ? -_CloudTex_ST.xy : _CloudTex_ST.xy)+_CloudTex_ST.zw);
			
			fixed3 projnormal = saturate(pow(IN.localNormal*1.5, 4));
			half4 cl = lerp(lerp(c0, c1, projnormal.z), c2, projnormal.x) * _CloudColor;
	
			o.Normal = lerp(half3(0,0,1),normal,s);
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));		
			o.Emission = _RimColor.rgb * rim *rim * rim;
			o.Albedo = lerp(lerp(c.rgb*lerp(t,1,0.5),_AtmosphereColor.rgb,_AtmosphereColor.a), cl.rgb, cl.a);
			o.Smoothness = lerp((1-s)*_Shininess,0,cl.a);
		}
		ENDCG

	}
}
