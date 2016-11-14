using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Assets.NinjaGame.Scripts;

[CustomEditor(typeof(NinjaGame))]
public class NinjaGameEditor : Editor {

    SerializedProperty objectsSP;
    SerializedProperty probabilitiesSP;
    SerializedProperty colorsSP;
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
        //Selection.activeGameObject = GameObject.Find("Application"); 
    }

  /*  public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        objectsSP =  serializedObject.FindProperty("objects");
        probabilitiesSP = serializedObject.FindProperty("probabilites");
        colorsSP = serializedObject.FindProperty("colors");
        Show(objectsSP, probabilitiesSP, colorsSP);
        serializedObject.ApplyModifiedProperties();
    }

    public static void Show(SerializedProperty objects, SerializedProperty probabilities, SerializedProperty colors)
    {
        EditorGUILayout.LabelField("Objects probability");
        if (objectsSP != null)
            Debug.LogError("Size of:" + objects.arraySize);
        for (int i = 0; i < objects.arraySize; i++)
        {
            EditorGUILayout.LabelField("Object");
            EditorGUILayout.PropertyField(objects.GetArrayElementAtIndex(i), true);
            EditorGUILayout.LabelField("Probability");
            EditorGUILayout.PropertyField(probabilities.GetArrayElementAtIndex(i), true);
            EditorGUILayout.LabelField("Color");
            EditorGUILayout.PropertyField(colors.GetArrayElementAtIndex(i), true);
        }
    }
    */
    void OnSceneGUI()
    {
        
        serializedObject.Update();
      
        max_angle = angleSP.intValue;

        //NinjaGame t = target as NinjaGame;
        
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
