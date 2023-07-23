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
        GUILayout.Box("Builders");
        if (GUILayout.Button("Build Blank Space"))
        {
            Selection.activeGameObject = builder.BuildBlankSpace();
        }
        if (GUILayout.Button("Build Katakana Space"))
        {
            Selection.activeGameObject = builder.BuildMinigame(0);
        }
        if (GUILayout.Button("Build Grammar Space")) 
        {
            Selection.activeGameObject = builder.BuildMinigame(1);
        }
        if (GUILayout.Button("Build Kanji Space"))
        {
            Selection.activeGameObject = builder.BuildMinigame(2);
        }
        if (GUILayout.Button("Build Vocabulary Space"))
        {
            Selection.activeGameObject = builder.BuildMinigame(3);
        }
        if (GUILayout.Button("Build Duel Space"))
        {
            Selection.activeGameObject = builder.BuildMinigame(4);
        }
        if (GUILayout.Button("Build Multiplayer Space"))
        {
            Selection.activeGameObject = builder.BuildMinigame(5);
        }
        if (GUILayout.Button("Build Crossroad"))
        {
            Selection.activeGameObject = builder.BuildCrossroad();
        }
        if (GUILayout.Button("Build Finish Line"))
        {
            Selection.activeGameObject = builder.BuildFinishLine();
        }
        GUILayout.Box("Tools");
        if (GUILayout.Button("Draw Lines"))
        {
            builder.DrawLines();
        }
        if (GUILayout.Button("Rename WPs"))
        {
            builder.RenameWPs();
        }
        if (GUILayout.Button("Change Selected To"))
        {
            builder.ChangeSelectedTo(Selection.activeGameObject);
        }


        GUILayout.Box("Don't forget to update Board Controller's waypoints in the Script Object!");
    }
    
}
