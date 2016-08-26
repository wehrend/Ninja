using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.Collections;
using System.Collections.Generic;
using Assets.NinjaGame.Scripts;
using VRTK;

namespace Assets.NinjaGame.Editor
{
    public class NinjaGameEditorWindow : EditorWindow
    {
        public string[] controller; //= new string[] { "Vive controller_ Sword", "Hand(s)", };
        public int index = 0;
        public bool groupEnabled;
        public Object[] prefabs;
        public Object[] levels;
        [MenuItem("NinjaGame/NinjaGame Configuration")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            NinjaGameEditorWindow window = (NinjaGameEditorWindow)EditorWindow.GetWindow(typeof(NinjaGameEditorWindow));
            window.Show();
        }



        void OnGUI()
        {
            controller=FindControllers();


            FindAvailablePrefabsOfType();
            FindAvailableLevels();

            bool prefabchoosen =true;
            GUILayout.Label("Ninja Game Configuration", EditorStyles.boldLabel);
            index = EditorGUILayout.Popup(index, controller);

            //index = EditorGUILayout.Popup(index, level);
            GUILayout.Label("Available Fruits and Bombs (Prefabs)", EditorStyles.boldLabel);
            foreach (var prefab in prefabs)
            {

                Debug.Log(prefab.name);

                EditorGUILayout.Toggle(prefab.name, prefabchoosen);
            }


            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("Ok"))
                DoSomething();
        }



        /*
        * We want find everything what is controllable / interactable via the vive controller
        * This may be direct (Controller_Hand,SteamVR_Model) or indirect (Sword,Lightsaber)
        */
        string[] FindControllers()
        {
            Object[] controllableObjects;
            List<String> controlls = new List<String>();

            controllableObjects = Resources.FindObjectsOfTypeAll(typeof(VRTK_InteractableObject));
            foreach (var controllables in controllableObjects)
            {
                //Debug.Log("Controllables:"+controllables.name);
                controlls.Add(controllables.name);
            }
            String[] results = new String[controlls.Count];
            return results= controlls.ToArray();
        }

        /*
        * Also we want to find any prefabs which are moving rigidbodys
        */

        void FindAvailablePrefabsOfType()
        {
           //An array of objects whose class is type or is derived from type
           prefabs= Resources.FindObjectsOfTypeAll(typeof(MovingRigidbodyPhysics));

        }


        /*
        * Lastly, we want find any levels (gameobjects childing or otherwise involving FruitsAndBombsSpawner )
        */
        void FindAvailableLevels()
        {
            levels = Resources.FindObjectsOfTypeAll(typeof(FruitAndBombSpawner));
            foreach (var level in levels)
            {
                Debug.Log("Controllables:"+level.name);
                //level.Add(level.name);
            }
        }


       void DoSomething()
       {
            Debug.Log("choosen prefab" + index);
       }
    
    }

}
