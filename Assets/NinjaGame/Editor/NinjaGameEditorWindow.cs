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
        public List<String> controller; //= new string[] { "Vive controller_ Sword", "Hand(s)", };
        public List<String> levels;
        public int index =0;
        public int controllablesIndex=0;
        public int levelsIndex = 0;
        public bool groupEnabled;
        public Object[] prefabs;

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
            levels =FindAvailableLevels();

            bool prefabchoosen =true;
            GUILayout.Label("Ninja Game Configuration", EditorStyles.boldLabel);
            GUILayout.Label("Available controller(s)", EditorStyles.boldLabel);
            index= EditorGUILayout.Popup(index, controller.ToArray());


            GUILayout.Label("Choose Level", EditorStyles.boldLabel);
            levelsIndex = EditorGUILayout.Popup(levelsIndex, levels.ToArray());
            GUILayout.Label("Available Fruits and Bombs (Prefabs)", EditorStyles.boldLabel);
            foreach (var prefab in prefabs)
            {

               //Debug.Log(prefab.name);

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
        List<String> FindControllers()
        {
            Object[] controllableObjects;
            List<String> controlls = new List<String>();

            controllableObjects = Resources.FindObjectsOfTypeAll(typeof(VRTK_InteractableObject));
            foreach (var controllables in controllableObjects)
            {
                //Debug.Log("Controllables:"+controllables.name);
                controlls.Add(controllables.name);
            }

            return controlls;
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
        List<String> FindAvailableLevels()
        {
            Object[] levelObjects;
            List<String> levelsList = new List<String>();
            levelObjects = Resources.FindObjectsOfTypeAll(typeof(Level) );

            foreach ( var level in levelObjects)
            {
                //var levelPrefab= PrefabUtility.FindPrefabRoot(level);

                //Debug.Log("Level:"+ level.name );
                levelsList.Add(level.name);

            }
            return levelsList;
        }


       void DoSomething()
       {
           //Activate choosen level and controllers
           Debug.Log("Choosen Controller(s): " +  controller[controllablesIndex]);
           GameObject.Find(controller[controllablesIndex]).active =true;
           Debug.Log("Choosen Level: " +  levels[levelsIndex]);
           GameObject.Find(controller[controllablesIndex]).active=true;


       }
    
    }

}
