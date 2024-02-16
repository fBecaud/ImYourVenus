using UnityEditor;
using UnityEngine;
using System;

class PlanetGenSolidPlanetStandardShaderGUI : ShaderGUI {
	MaterialEditor editor;
	MaterialProperty[] properties;
	Material material;
	
    override public void OnGUI (MaterialEditor editor, MaterialProperty[] properties) {
		this.editor = editor;
		this.properties = properties;
		this.material = editor.target as Material;
		//
		// Atmosphere properties
		GUILayout.Label("Atmosphere properties", EditorStyles.boldLabel);
		editor.ColorProperty(GetProp("_AtmosphereColor"), "Atmosphere Color");	
		editor.ColorProperty(GetProp("_RimColor"), "Rim Color");	
		// Surface parameters
		GUILayout.Label("Surface parameters", EditorStyles.boldLabel);
		editor.TexturePropertySingleLine(new GUIContent("Heightmap",""), GetProp("_HeightMap"));
		editor.RangeProperty(GetProp("_BumpStrength"), "Bump strength");	
		// Water settings
		GUILayout.Label("Water settings", EditorStyles.boldLabel);
		editor.RangeProperty(GetProp("_SeaLevel"), "Sea level");
		editor.ColorProperty(GetProp("_SeaColor"), "Sea Color");	
		editor.RangeProperty(GetProp("_Shininess"), "Shininess");
		// Land Colors
		GUILayout.Label("Land settings", EditorStyles.boldLabel);	
		editor.RangeProperty(GetProp("_ColorBlend"), "Color Blending");				
		editor.ColorProperty(GetProp("_LandColor"), "Land Base Color");	
		editor.RangeProperty(GetProp("_HillLevel"), "Hill Level");	
		editor.ColorProperty(GetProp("_HillColor"), "Hill Color");	
		editor.RangeProperty(GetProp("_MountainLevel"), "Mountain Level");	
		editor.ColorProperty(GetProp("_MountainColor"), "Mountain Color");				
		// Cloud Settings
		GUILayout.Label("Cloud settings", EditorStyles.boldLabel);		
		editor.ColorProperty(GetProp("_CloudColor"), "Cloud Color");	
		editor.TexturePropertySingleLine(new GUIContent("Cloud Texture",""), GetProp("_CloudTex"));			
		editor.RangeProperty(GetProp("_CloudScale"), "Uniform Scale");		
		TextureUVOffsetProperty(GetProp("_CloudTex"), "Offset"); 		
	}

	MaterialProperty GetProp (String propName) {
		return FindProperty(propName, properties);
	}	

	Vector4 TextureUVOffsetProperty(MaterialProperty uvTransformProperty, String secondText) {
		Rect position = EditorGUILayout.GetControlRect(true, 32.0f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
		Vector4 uvTransform = uvTransformProperty.textureScaleAndOffset;
		Vector2 value2 = new Vector2(uvTransform.z, uvTransform.w);
		float num = EditorGUIUtility.labelWidth;
		float x = position.x + num;
		float x2 = position.x;
		Rect totalPosition = new Rect(x2, position.y, EditorGUIUtility.labelWidth, 16.0f);
		Rect position2 = new Rect(x, position.y, position.width - EditorGUIUtility.labelWidth, 16.0f);
		EditorGUI.PrefixLabel(totalPosition, new GUIContent(secondText));
		value2 = EditorGUI.Vector2Field(position2, GUIContent.none, value2);
		Vector4 newUVTransform = new Vector4(1f, 1f, value2.x, value2.y);
		uvTransformProperty.textureScaleAndOffset = newUVTransform;
		return newUVTransform;
	}	
}