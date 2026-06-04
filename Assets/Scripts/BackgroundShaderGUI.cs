using UnityEditor;
using UnityEngine;
using System;
using static UnityEditor.MaterialProperty;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public class BackgroundShaderGUI : ShaderGUI
{
    MaterialEditor m_MaterialEditor;
    MaterialProperty[] _MATERIALS_PROPERTY_BACKGROUNDSHADER = new MaterialProperty[3];
    private readonly string[] _SHADER_PROPERTY_BACKGROUNDSHADER = new string[3]
    {
        "_Texture_random",
        "_Texture_size",
        "_USETEXTURERANDOMTILE",
    };

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        /*
        FindProperties(properties);
        this.m_MaterialEditor = materialEditor;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.richText = true;

        DrawTextureRandomTile(style);
        */
        base.OnGUI(materialEditor, properties);
    }

    #region Utility Method
    public void FindProperties(MaterialProperty[] properties)
    {
        for (int i = 0; i < _MATERIALS_PROPERTY_BACKGROUNDSHADER.Length; i++)
        {
            this._MATERIALS_PROPERTY_BACKGROUNDSHADER[i] = FindProperty(_SHADER_PROPERTY_BACKGROUNDSHADER[i], properties);
        }
    }
    private float GetValueFromIndex(int valueIndexToGet)
    {
        float valueToReturn = this._MATERIALS_PROPERTY_BACKGROUNDSHADER[valueIndexToGet].floatValue;
        return valueToReturn;
    }
    private void DrawShaderPropertyOnGUI(int index)
    {
        this.m_MaterialEditor.ShaderProperty(_MATERIALS_PROPERTY_BACKGROUNDSHADER[index], _MATERIALS_PROPERTY_BACKGROUNDSHADER[index].displayName);
    }
    
    private void DrawInitiateBoxLableField(string sectionName, GUIStyle style)
    {
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Helpbox");
        EditorGUILayout.LabelField("<b><size=14>" + sectionName + "</size></b>", style);
    }
    private void DrawTextureRandomTile(GUIStyle style)
    {
        DrawInitiateBoxLableField("Draw Texture Random Tile", style);

        int featureValue = 2;
        float featureShaderValue = GetValueFromIndex(featureValue);

        bool useTextureRandomTillingFeature = featureValue > 0;

        DrawShaderPropertyOnGUI(featureValue);

        if (useTextureRandomTillingFeature)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Feature 1 : Vertex Color Debugging", EditorStyles.boldLabel);
            DrawShaderPropertyOnGUI(0);
            DrawShaderPropertyOnGUI(1);
        }
    }
    #endregion
}
