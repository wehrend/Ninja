using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.NinjaGame.Scripts;

namespace Assets.NinjaGame.EditorExtensions
{
    [CustomEditor(typeof(MovingRigidbody))]
    public class MovingRigidbodyDataInspector : Editor
    {


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startPosition"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startRotation"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startTimes"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("velocities"), true);
            serializedObject.ApplyModifiedProperties();


        }
    }
}