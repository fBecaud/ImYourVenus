
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PGSolidPlanet))]
public class PGSolidPlanetInspector : Editor {
	PGSolidPlanet pgSolidPlanet;
	bool autoGenerateToggle = false;
	
    public override void OnInspectorGUI() {

		DrawDefaultInspector ();
		
		GUILayout.Space(10);
		
		autoGenerateToggle = GUILayout.Toggle(autoGenerateToggle, "Auto Generate");
		
		if (GUILayout.Button("Generate")) {
			pgSolidPlanet = target as PGSolidPlanet;
			Undo.RegisterCompleteObjectUndo(pgSolidPlanet, "PG Solid Planet Generate");
			pgSolidPlanet.GeneratePlanet();
		}
		if (GUILayout.Button("Randomize")) {
			pgSolidPlanet = target as PGSolidPlanet;
			Undo.RegisterCompleteObjectUndo(pgSolidPlanet, "PG Solid Planet Randomize");			
			pgSolidPlanet.RandomizePlanet(true);
		}		
		if (GUI.changed) {
			pgSolidPlanet = target as PGSolidPlanet;	
			Undo.RegisterCompleteObjectUndo(pgSolidPlanet, "PG Solid Planet Inspector change");				
			pgSolidPlanet.UpdatePlanetMaterial();
			if (autoGenerateToggle) pgSolidPlanet.GeneratePlanet();
		}
    }

	void OnEnable() {
        Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }
 
    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
    }
 
    private void OnUndoRedoPerformed()
    {
		pgSolidPlanet = target as PGSolidPlanet;			
		pgSolidPlanet.UpdatePlanetMaterial();
		pgSolidPlanet.GeneratePlanet();
    }	
}
