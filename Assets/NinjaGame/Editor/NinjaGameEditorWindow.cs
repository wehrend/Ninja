using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using Assets.NinjaGame.Scripts;

namespace Assets.NinjaGame.Editor
{
    public class NinjaGameEditorWindow : EditorWindow
    {
        public string[] controller = new string[] { "Vive controller_ Sword", "Hand(s)", };
        public int index = 0;
        public bool groupEnabled;
        public UnityEngine.Object[] prefabs;

        [MenuItem("NinjaGame/NinjaGame Configuration")]
        static void Init()
        {
          
            // Get existing open window or if none, make a new one:
            NinjaGameEditorWindow window = (NinjaGameEditorWindow)EditorWindow.GetWindow(typeof(NinjaGameEditorWindow));
            window.Show();
        }

        void OnGUI()
        {
            FindObjectsOfType();
            GUILayout.Label("Ninja Game Configuration", EditorStyles.boldLabel);
            index = EditorGUILayout.Popup(index, controller);
            GUILayout.Label("Available Fruits and Bombs (Prefabs)", EditorStyles.boldLabel);
            foreach (var prefab in prefabs)
            {
                bool prefabchoosen =true;
                Debug.Log(prefab.name);

                EditorGUILayout.Toggle(prefab.name, prefabchoosen);
            }

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("Ok"))
                DoSomething();
        }

        void FindObjectsOfType()
        {
            if (prefabs.Count() == 0)
                //An array of objects whose class is type or is derived from type.
                prefabs = Resources.FindObjectsOfTypeAll(typeof(MovingRigidbodyPhysics));
        }


       void DoSomething()
       {
            Debug.Log("choosen prefab" + index);
       }
    
    }

}
