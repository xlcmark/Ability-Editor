using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Fow_WallGenerator))]
public class WallGeneratorEditor : Editor
{
    private Fow_WallGenerator wg;
    private void OnEnable()
    {
        wg = (Fow_WallGenerator)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        if (GUILayout.Button("CreateWall"))
        {
            wg.CreateWall();
        }
    }
}
