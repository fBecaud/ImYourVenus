
Shader "Hidden/Zololgo/PlanetGen/3DNoise" {
	Properties {}
	
	SubShader {
		Tags { "RenderType" = "PlanetGen3DNoise"}
		cull front
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma fragmentoption ARB_precision_hint_nicest
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float3 vpos : TEXCOORD1;
			};

			float _Seed, _Iters, _H;
			float4 mod289(float4 x){return x - floor(x * (1.0 / 289.0)) * 289.0;}
			float4 perm(float4 x){return mod289(((x * 34.0) + 1.0) * x);}
			float4 _Scale;
			
			float noise(float3 _p, float3 sc, float sd) {
				float3 p = normalize(_p) * sc + float3(sd,sd,sd);
				float3 a = floor(p);
				float3 d = p - a;
				d = d * d * (3.0 - 2.0 * d);
				float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				float4 k1 = perm(b.xyxy);
				float4 k2 = perm(k1.xyxy + b.zzww);
				float4 c = k2 + a.zzzz;
				float4 k3 = perm(c);
				float4 k4 = perm(c + 1.0);
				float4 o1 = frac(k3 * (1.0 / 41.0));
				float4 o2 = frac(k4 * (1.0 / 41.0));
				float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				float final = o4.y * d.y + o4.x * (1.0 - d.y);
				return final;
			}

			v2f vert (appdata_full v) { 
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex);
				o.vpos = v.vertex.xyz;
				return o;
			}

			float4 frag( v2f i ) : COLOR {
			    float G = exp2(-_H);	// persistence
			    float3 f = _Scale.xyz*_Scale.w;	// freq
				float maxA = 0.0;	// max amp
			    float a = 1;		// amp
			    float t = 0;
			    for( int j=0; j<_Iters; j++ ) {
			        t += a*noise(i.vpos, f, _Seed);
					maxA += a;
			        f *= 2;
			        a *= G;
			    }
				t /= maxA;
				return float4(t,t,t,1.0);
			}
			ENDCG
		}
	}
}
