
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class PlanetGenCreateNew : Editor {
	[MenuItem ("Tools/Zololgo/PlanetGen/Create New Solid Planet", false, 10)]
	static void CreateNewSolidPlanet () {

		Transform newPlanet = Instantiate(AssetDatabase.LoadAssetAtPath<Transform>("Assets/Zololgo/PlanetGen/Prefabs/Solid Planet.prefab"));
		newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
		Selection.activeObject = newPlanet;
		newPlanet.GetComponent<PGSolidPlanet>().planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
		newPlanet.GetComponent<PGSolidPlanet>().planetMaterial.hideFlags = HideFlags.HideAndDontSave;
		newPlanet.GetComponent<PGSolidPlanet>().GeneratePlanet();
		Undo.RegisterCreatedObjectUndo(newPlanet.gameObject, "Create new Solid Planet");
    }
	[MenuItem ("Tools/Zololgo/PlanetGen/Create New Solid Planet LOD Group", false, 10)]
	static void CreateNewSolidPlanetLODGroup () {

		Transform newPlanet = Instantiate(AssetDatabase.LoadAssetAtPath<Transform>("Assets/Zololgo/PlanetGen/Prefabs/Solid Planet LOD Group.prefab"));
		newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
		Selection.activeObject = newPlanet;
		newPlanet.GetComponent<PGSolidPlanet>().planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
		newPlanet.GetComponent<PGSolidPlanet>().planetMaterial.hideFlags = HideFlags.HideAndDontSave;		
		newPlanet.GetComponent<PGSolidPlanet>().GeneratePlanet();		
		Undo.RegisterCreatedObjectUndo(newPlanet.gameObject, "Create new Solid Planet LOD Group");
    }	
	[MenuItem ("Tools/Zololgo/PlanetGen/Create New Halo", false, 10)]
	static void CreateNewPlanetHalo () {

		Transform newPlanet = Instantiate(AssetDatabase.LoadAssetAtPath<Transform>("Assets/Zololgo/PlanetGen/Prefabs/Planet Halo.prefab"));
		newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
		Selection.activeObject = newPlanet;
		newPlanet.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Other/Planet Halo"));
		newPlanet.GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.HideAndDontSave;		 	
		Undo.RegisterCreatedObjectUndo(newPlanet.gameObject, "Create new Planet Halo");
    }		
}

