using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PGSolidPlanet : MonoBehaviour {

	public Material planetMaterial; 
	public enum ResolutionPreset {_256x128, _512x256, _1024x512, _2048x1024, _4096x2048}
	[Header("Surface properties")]
	//
	[Range(0.5f,5f)]
	public float noiseScale = 1.0f;
	Vector2 noiseScaleLimits = new Vector2 (0.5f, 5f);
	//
	[Range(0.4f,0.6f)]
	public float noisePersistence = 0.5f;
	Vector2 noisePersistenceLimits = new Vector2 (0.4f, 0.6f);	

	int octaves = 20;	
	public int seed = 0;
	//[HideInInspector]
	Vector4 scale = Vector4.one;

	[HideInInspector]
	public float randomizedSeed = 0f;
	[Header("Heightmap texture properties")]
	
	public ResolutionPreset resolution = ResolutionPreset._2048x1024;
	public FilterMode filterMode = FilterMode.Trilinear;
	[Range(1,9)]
	public int anisotropicLevel = 5;
	public bool enableMipMaps = true;
		
	RenderTexture renderTex;
	//[HideInInspector]
	Vector2Int texResolution;
	Cubemap cubemap;
	Camera cubeCam;
	Material mat;
	Texture2D equirectangularTex;
	byte[] bytes;

	void OnEnable() {
		RenderPlanet();			
    }
	
	void Update() {
		UpdatePlanetShader();
	}

	void OnValidate(){
		// UpdatePlanetMaterial();
	}
	
	public void GeneratePlanet() {
		Random.InitState(seed);
		randomizedSeed = (float)Random.Range(-9999,9999) / 100f;
		RenderPlanet(true);		
	}

	public void RandomizePlanet() {
		RandomizePlanet(true);
	}

	public void RandomizePlanet(bool generate) {
		seed = Random.Range(-99999,99999);
		// noiseScale = Random.Range(noiseScaleLimits.x,noiseScaleLimits.y);
		// noisePersistence = Random.Range(noisePersistenceLimits.x,noisePersistenceLimits.y);		
		if (generate) GeneratePlanet();
	}

	public void UpdatePlanetMaterial() {
		
		LODGroup lodGroup = GetComponent<LODGroup>();
		if (lodGroup != null) {
			if (GetComponent<MeshFilter>() == null && GetComponent<MeshRenderer>() == null) {
				MeshRenderer dummyRen = gameObject.AddComponent<MeshRenderer>();
			}
			GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
			LOD[] lods = lodGroup.GetLODs();
			for (int i = 0; i<lods.Length; i++) {
				for (int j = 0; j<lods[i].renderers.Length; j++) {
					#if UNITY_EDITOR
					lods[i].renderers[j].sharedMaterial = planetMaterial;
					#else
					lods[i].renderers[j].material = planetMaterial;
					#endif
				}				
			}
		}	
		
		#if UNITY_EDITOR
		GetComponent<MeshRenderer>().sharedMaterial = planetMaterial;
		#else
		GetComponent<MeshRenderer>().material = planetMaterial;
		#endif		
	}
	
	void UpdatePlanetShader() {
		if (planetMaterial) planetMaterial.SetTexture("_HeightMap", equirectangularTex);
	}
	
	void RenderPlanet () {
		RenderPlanet(false);
	}
	void RenderPlanet (bool forceRender) {
		
		UpdateResolutions();
		if (equirectangularTex == null || 
		equirectangularTex.width != texResolution.x || 
		equirectangularTex.height != texResolution.y ||
		(equirectangularTex.mipmapCount == 1 && enableMipMaps) ||
		(equirectangularTex.mipmapCount > 1 && !enableMipMaps) ||
		equirectangularTex.filterMode != filterMode ||
		equirectangularTex.anisoLevel != anisotropicLevel || forceRender) {
		
			ResetPlanetTextures();

			if (cubemap == null) {
				cubemap = new Cubemap (texResolution.y, TextureFormat.RGB24, false);
			}
			if (mat == null) mat = new Material(Shader.Find("Hidden/Zololgo/PlanetGen/CubeToEquirectangular"));

			if (planetMaterial == null) {
				return;
			}		

			UpdatePlanetMaterial();
			Shader tempShader = planetMaterial.shader;
			planetMaterial.shader = Shader.Find("Hidden/Zololgo/PlanetGen/3DNoise");
			planetMaterial.SetFloat("_H", 1.1f-noisePersistence);
			planetMaterial.SetFloat("_Seed", randomizedSeed);
			planetMaterial.SetVector("_Scale", new Vector4(scale.x, scale.y, scale.z, noiseScale));
			planetMaterial.SetFloat("_Iters", octaves);
	
			var tempRot = transform.rotation;
			transform.rotation = Quaternion.identity;
			RenderCubeCam();
			transform.rotation = tempRot;	
			planetMaterial.shader = tempShader;
			Shader.SetGlobalTexture("_Cubemap", cubemap);
			renderTex = RenderTexture.GetTemporary(equirectangularTex.width, equirectangularTex.height, 0, RenderTextureFormat.ARGB32);
			RenderTexture.active = renderTex;
			Graphics.Blit(renderTex, renderTex, mat);
			equirectangularTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(renderTex);
			equirectangularTex.SetPixels(StretchContrast(equirectangularTex.GetPixels(0)),0);
			equirectangularTex.Apply();
		}
	}

	void ResetPlanetTextures() {
		equirectangularTex = new Texture2D(texResolution.x, texResolution.y, TextureFormat.RGB24, enableMipMaps);
		equirectangularTex.wrapMode = TextureWrapMode.Clamp;
		equirectangularTex.filterMode = filterMode;
		equirectangularTex.anisoLevel = anisotropicLevel;			
		cubemap = new Cubemap (texResolution.y, TextureFormat.RGB24, false);
		Resources.UnloadUnusedAssets();
	}
	
	void RenderCubeCam() {
		cubeCam = GetComponent<Camera>();
		if (!cubeCam) {
			cubeCam = gameObject.AddComponent<Camera>();
		}
		cubeCam.nearClipPlane = 0.01f;
		cubeCam.farClipPlane = 1000f;		
		cubeCam.clearFlags = CameraClearFlags.SolidColor;
		cubeCam.backgroundColor = Color.black;
		cubeCam.enabled = false;
		cubeCam.hideFlags = HideFlags.HideInInspector;
		cubeCam.SetReplacementShader(null, "PlanetGen3DNoise");
		cubeCam.RenderToCubemap( cubemap );
		DestroyImmediate(cubeCam);
	}

	void UpdateResolutions() {
		if (resolution == ResolutionPreset._256x128) {
			texResolution = new Vector2Int (256,128);
		}
		else if (resolution == ResolutionPreset._512x256) {
			texResolution = new Vector2Int (512,256);
		}
		else if (resolution == ResolutionPreset._1024x512) {
			texResolution = new Vector2Int (1024,512);
		}
		else if (resolution == ResolutionPreset._2048x1024) {
			texResolution = new Vector2Int (2048,1024);
		}
		else if (resolution == ResolutionPreset._4096x2048) {
			texResolution = new Vector2Int (4096,2048);
		}		
	}
	
	Color[] StretchContrast(Color[] colors) {
		float minC = 1f; 
		float maxC = 0f;
		int i = 0;
		float nv;
		for (i = 0; i < colors.Length; i++) {
			if (colors[i].r < minC) minC = colors[i].r;
			if (colors[i].r > maxC) maxC = colors[i].r;
		}
		float range = maxC - minC;
		for (i = 0; i < colors.Length; i++) {
			nv = (colors[i].r - minC) / range;
			colors[i] = new Color (nv,nv,nv);
		}			
		minC = 1f; 
		maxC = 0f;
		for (i = 0; i < colors.Length; i++) {
			if (colors[i].r < minC) minC = colors[i].r;
			if (colors[i].r > maxC) maxC = colors[i].r;
		}
		range = maxC - minC;
		return colors;
	}
}
