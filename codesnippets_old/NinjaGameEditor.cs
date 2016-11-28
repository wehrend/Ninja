using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.NinjaGame.Scripts;

[CustomEditor(typeof(NinjaGame))]
[CanEditMultipleObjects]
public class NinjaGameEditor : Editor {


    SerializedProperty spawnerDistanceSP;
    SerializedProperty spawnerRangeSP;
    SerializedProperty velocitySP;
    SerializedProperty velocityRangeSP;
    SerializedProperty angleSP;
    SerializedObject so;
    int max_angle;


    void OnEnable()
    {
       // spawnerDistanceSP = serializedObject.FindProperty("spawnerDistance");
       // spawnerRangeSP = serializedObject.FindProperty("spawnerRange");
        //angleSP = serializedObject.FindProperty("angle");
    }

    public override void OnInspectorGUI()
    {
        so.Update();
        var property = so.GetIterator();
        Debug.LogError(property);
        var next = property.NextVisible(true);
        if (next)
            do
            {
                //GUI.color = cachedGuiColor;
                this.ShowProperty(property);
            } while (property.NextVisible(false));
        so.ApplyModifiedProperties();
    }

    protected void ShowProperty(SerializedProperty property)
    {
        EditorGUILayout.PropertyField(property, property.isExpanded);
    }



    void OnSceneGUI()
    {
     /*   so.Update();

        max_angle = angleSP.intValue;

        //NinjaGame t = target as NinjaGame;
        
        Handles.color = Color.white;
        //Draw the spawner area 
        //inner boundary
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, max_angle/2 , spawnerDistanceSP.floatValue-spawnerRangeSP.floatValue/2);
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, -max_angle/2, spawnerDistanceSP.floatValue - spawnerRangeSP.floatValue / 2);
        //outer boundary 
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, max_angle/2, spawnerDistanceSP.floatValue + spawnerRangeSP.floatValue / 2);
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, -max_angle/2, spawnerDistanceSP.floatValue+spawnerRangeSP.floatValue/2);*/
    }
}

