using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapBuilder builder = (MapBuilder)target;
        if (GUILayout.Button("Build Blank Space"))
        {
            Selection.activeGameObject = builder.BuildBlankSpace();
        }
        if (GUILayout.Button("Build Crossroad"))
        {
            builder.BuildCrossroad();
        }
        if (GUILayout.Button("Build Finish Line"))
        {
            builder.BuildFinishLine();
        }
        if (GUILayout.Button("Draw Lines"))
        {
            builder.DrawLines();
        }
        if (GUILayout.Button("Rename WPs"))
        {
            builder.RenameWPs();
        }

        GUILayout.Box("Don't forget to update Board Controller's waypoints in the Script Object!");
    }
    
}
