Shader "Hidden/Zololgo/PlanetGen/CubeToEquirectangular"{

	Properties {}

	Subshader {

		Fog {Mode Off}
		Lighting Off
		ZTest Always Cull Off ZWrite Off

		Pass {
            CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			//#pragma glsl
			#pragma fragmentoption ARB_precision_hint_nicest
			#pragma exclude_renderers xbox360

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			samplerCUBE _Cubemap;
			v2f vert (appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv =  v.texcoord.xy;
				return o;
			}

			float4 frag (v2f i) : COLOR {
				float pi = 3.141592653589793;
				float theta = (1-i.uv.y) * pi;
				float phi = (1-(i.uv.x)) * 2 * pi;
				float4 lookup;
				lookup.x = sin(theta) * cos(phi);
				lookup.z = sin(theta) * sin(phi);;
				lookup.y = cos(theta);
				lookup.w = 0.0;
				//float4 c = texCUBElod (_Cubemap, normalize(lookup));
				float4 c = texCUBE (_Cubemap, normalize(lookup));
				return c;
			}
            ENDCG
		}
	}
}
