using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapGenerator mapGen = (MapGenerator)target;
        GUILayout.Space(5);

        if (GUILayout.Button ("Generate New Path"))
        {
            mapGen.RegenGrid();
        }
    }
}
