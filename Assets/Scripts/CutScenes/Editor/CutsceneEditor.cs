using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as Cutscene; // Corrected variable name
        if (GUILayout.Button("Add Dialogue Action"))
            cutscene.AddAction(new DialogueAction());
        else if (GUILayout.Button("Add Move Actor Action"))
            cutscene.AddAction(new MoveActorAction());

        base.OnInspectorGUI();
    }
}
