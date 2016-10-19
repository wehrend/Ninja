using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.NinjaGame.Scripts;

[CustomEditor(typeof(NinjaGame))]
public class NinjaGameEditor : Editor {

    SerializedProperty spawnerDistanceSP;
    SerializedProperty spawnerRangeSP;
    SerializedProperty velocitySP;
    SerializedProperty velocityRangeSP;
    SerializedProperty angleSP;
    int max_angle;


    void OnEnable()
    {
        spawnerDistanceSP = serializedObject.FindProperty("spawnerDistance");
        spawnerRangeSP = serializedObject.FindProperty("spawnerRange");
        // velocitySP = serializedObject.FindProperty("velocityAvg");
        // velocityRangeSP = serializedObject.FindProperty("velocityRange");
        angleSP = serializedObject.FindProperty("angle");
    }
   

    void OnSceneGUI()
    {
        serializedObject.Update();

        max_angle = angleSP.intValue;

        //NinjaGame t = target as NinjaGame;
        Debug.Log("Draw circle");
        
        Handles.color = Color.white;
        //Draw the spawner area 
        //inner boundary
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, max_angle/2 , spawnerDistanceSP.floatValue-spawnerRangeSP.floatValue/2);
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, -max_angle/2, spawnerDistanceSP.floatValue - spawnerRangeSP.floatValue / 2);
        //outer boundary 
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, max_angle/2, spawnerDistanceSP.floatValue + spawnerRangeSP.floatValue / 2);
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, -max_angle/2, spawnerDistanceSP.floatValue+spawnerRangeSP.floatValue/2);
    }
}
