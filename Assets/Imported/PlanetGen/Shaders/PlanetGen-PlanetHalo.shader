Shader "Zololgo/PlanetGen | Other/Planet Halo" {

    Properties {
        _Tint ("Tint (RGB)", Color) = (1,1,1,1)
		_Falloff ("Falloff", float) = 1
    }

    SubShader {

		Tags { "Queue" = "Transparent" }
		zwrite off
		cull front
        Blend One One

        Pass {

            CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
			half4 _Tint;
			half _Falloff;

            struct v2f {
                half4 pos : SV_POSITION;
                float rim : TEXCOORD0;
            };

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                half3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.rim = dot(viewDir,-v.normal);
                return o;
            }

			half4 frag (v2f i) : COLOR {
				return smoothstep(0.1, _Falloff, i.rim)*_Tint;
            }

            ENDCG
		}

    }
}
