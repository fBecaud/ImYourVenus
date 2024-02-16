using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlanetMaker : MonoBehaviour
{
	public Transform planetPrefab;
	public Shader planetShader;

    void Start()
    {
		if (planetPrefab != null) {
			Transform newPlanet = Instantiate(planetPrefab);
			newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
			newPlanet.GetComponent<PGSolidPlanet>().planetMaterial = new Material(planetShader);		
			newPlanet.GetComponent<PGSolidPlanet>().RandomizePlanet();
		}
    }

}
