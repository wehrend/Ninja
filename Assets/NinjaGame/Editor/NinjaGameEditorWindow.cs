using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Assets.NinjaGame.Scripts;

namespace Assets.NinjaGame.Editor
{
    public class NinjaGameEditorWindow : EditorWindow
    {
        public string[] controller = new string[] { "Vive controller / Sword", "Hand(s)", };
        public int index = 0;
        public bool groupEnabled;
        public List<UnityEngine.Object> resourcePrefabs = new List<UnityEngine.Object>();

        [MenuItem("NinjaGame/NinjaGame Configuration")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            NinjaGameEditorWindow window = (NinjaGameEditorWindow)EditorWindow.GetWindow(typeof(NinjaGameEditorWindow));
            window.Show();
        }

        void OnGUI()
        {
            PrefabSearch();
            GUILayout.Label("Ninja Game Configuration", EditorStyles.boldLabel);
            index = EditorGUILayout.Popup(index, controller);
            GUILayout.Label("Available Fruits and Bombs (Prefabs)", EditorStyles.boldLabel);
            foreach (var prefab in resourcePrefabs)
            {
               EditorGUILayout.Toggle(prefab.name,true);
            }

            if (GUILayout.Button("Ok"))
                DoSomething();
            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
    
            EditorGUILayout.EndToggleGroup();
        }
        /// <summary>
        /// ToDo: make to PrefabSearch(Type) or something 
        /// </summary>
        void PrefabSearch()
        {
            if (resourcePrefabs.Count == 0)
            {
                var allResources = Resources.LoadAll("Prefabs", typeof(MovingRigidbodyPhysics));
                foreach (var res in allResources)
                {
                    resourcePrefabs.Add(res);

                }
            }
        }

       void DoSomething()
       {
            Debug.Log("choosen prefab" + index);
       }
    
    }

}
