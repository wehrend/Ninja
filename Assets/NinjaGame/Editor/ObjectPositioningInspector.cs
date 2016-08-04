using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Assets.NinjaGame.EditorExtensions
{

    [CustomEditor(typeof(ObjectPositioningInspector))]
    public class ObjectPositioningInspector : Editor
    {

        List<Transform> objectPositions;
        Transform t;

        private void OnSceneGUI()
        {
            objectPositions = new List<Transform>();
            PositionHandle position = target as PositionHandle;
            Handles.color = Color.white;
            EditorGUI.BeginChangeCheck();
      
            Vector3 lookTarget = Handles.PositionHandle( position.lookTarget, Quaternion.identity);
            Handles.BeginGUI();
            GUILayout.TextField("00:00.00", 6);
            GUILayout.TextField("000", 3);
            Handles.EndGUI();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
                position.lookTarget = lookTarget;
                position.Update();

                t.position = position.lookTarget;
                //objectPositions.Add(t);

                 
            }
        }

    }

}