using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.NinjaGame.Scripts;

[CustomEditor(typeof(NinjaGame))]
public class NinjaGameEditor : Editor {

    SerializedProperty spawnerDistanceSP;
    SerializedProperty spawnerRangeSP;
    SerializedProperty angleSP;
    int max_angle;

    void OnSceneGUI()
    {
        serializedObject.Update();

        spawnerDistanceSP = serializedObject.FindProperty("spawnerDistance");
        spawnerRangeSP = serializedObject.FindProperty("spawnerRange");
        angleSP = serializedObject.FindProperty("angle");


        max_angle = angleSP.intValue;

        NinjaGame t = target as NinjaGame;
        Debug.Log("Draw circle");
        
        Handles.color = Color.blue;
        //Fixme: angle alignment has to be corrected.
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, 90, spawnerDistanceSP.floatValue-spawnerRangeSP.floatValue/2);
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, 90, spawnerDistanceSP.floatValue+spawnerRangeSP.floatValue/2);
    }
}
