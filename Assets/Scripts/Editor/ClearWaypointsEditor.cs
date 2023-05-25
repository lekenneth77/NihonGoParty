using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ClearWaypoints))]
public class ClearWaypointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ClearWaypoints clearer = (ClearWaypoints)target;
        if (GUILayout.Button("Clear All Waypoints"))
        {
            clearer.removeAll();
        }
    }
}
