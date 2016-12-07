using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.VREF.Scripts;

namespace Assets.NinjaGame.Scripts
{

    public class TrialsListEditorWindow : EditorWindow
    {
        SerializedObject so;
        public TrialsList trialsConfig;
        private int curIndex;
        public string dataDirectory;

        int angle;
        /// <summary>
        /// 
        /// </summary>
        int sizeOfTrials;
        string[] trialsname;
        int[] instances;
        Color[] color;
        float[] velocity;
        float[] distance;
        float[] scale;
        int[] numberOfParallelSpawns;
        string[] trialsnameToSelect;

        string expectedTrialsConfig;
        string saveTrialsConfig;

        [MenuItem("NinjaGame / Editing Trials List ")]
        public static void Init()
        {
            var window = (TrialsListEditorWindow)EditorWindow.GetWindow(typeof(TrialsListEditorWindow));
            window.Show();

        }

        public void OnEnable()
        {
            dataDirectory = Application.dataPath + "/NinjaGame/Config/";
            expectedTrialsConfig = dataDirectory + "trialslist.json";
            saveTrialsConfig = dataDirectory + "trialslist.json";

            if (trialsConfig == null)
            {
                trialsConfig = ScriptableObject.CreateInstance(typeof(TrialsList)) as TrialsList;
                //load default trials config

                trialsConfig = ConfigUtil.LoadConfig<TrialsList>(new FileInfo(expectedTrialsConfig), true, () =>
                {
                    Debug.LogError("Something is wrong with the AppConfig. Was not found and I was not able to create one!");
                });

                ReinitTrials();
            }


            trialsname[0] = "All Trials";
            ReinitTrials();
            if (NinjaGame.game != null)
            {
                NinjaGame.game.setListOfTrials(trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials));
                Debug.Log("Assigned Trials-List with size " + NinjaGame.game.trialsList.Count);
            } else {
                Debug.LogWarning("Static GameInfo instance not found, create one!");
                NinjaGame.game = new NinjaGame.GameInfo();
                NinjaGame.game.setListOfTrials(trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials));
                Debug.Log("Assigned Trials-List with size "+ NinjaGame.game.trialsList.Count );
            
            }
        }

        private void ReinitTrials()
        {
            angle = trialsConfig.maximumAngle;

            sizeOfTrials = trialsConfig.listOfTrials.Count;
            trialsname = new string[sizeOfTrials];
            instances = new int[sizeOfTrials];
            color = new Color[sizeOfTrials];
            velocity = new float[sizeOfTrials];
            distance = new float[sizeOfTrials];
            scale = new float[sizeOfTrials];
            numberOfParallelSpawns = new int[sizeOfTrials];
            trialsnameToSelect = new string[sizeOfTrials];

            for (int i = 0; i < sizeOfTrials; i++)
            {
                trialsConfig.maximumAngle = angle;
                trialsnameToSelect[i] = trialsConfig.listOfTrials[i].trial;
                instances[i] = trialsConfig.listOfTrials[i].instances;
                trialsname[i] = trialsConfig.listOfTrials[i].trial;
                color[i] = trialsConfig.listOfTrials[i].color;
                scale[i] = trialsConfig.listOfTrials[i].scale;
                velocity[i] = trialsConfig.listOfTrials[i].velocity;
                distance[i] = trialsConfig.listOfTrials[i].distance;
                numberOfParallelSpawns[i] = trialsConfig.listOfTrials[i].numberOfSpawners;

            }
        }

        private void SaveTrials()
        {
            for (int i = 0; i < sizeOfTrials; i++)
            {
               
                trialsConfig.listOfTrials[i].trial = trialsnameToSelect[i];
                trialsConfig.listOfTrials[i].trial = trialsname[i];
                trialsConfig.listOfTrials[i].instances = instances[i];
                trialsConfig.listOfTrials[i].color = color[i];
                trialsConfig.listOfTrials[i].scale = scale[i];
                trialsConfig.listOfTrials[i].velocity = velocity[i];
                trialsConfig.listOfTrials[i].distance = distance[i];
            }
        }



        public void Update()
        {
            Repaint();
        }

        public void OnSceneGUI()
        {
            //set Application game object active 
            
            Handles.color = Color.white;
            //Draw the spawner Arc
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, angle / 2, distance[curIndex]);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, angle / 2, distance[curIndex]);
            
            var prefab = Resources.Load("BasicPrefab", typeof(MovingRigidbodyPhysics)) as MovingRigidbodyPhysics;
            prefab.name = trialsname[curIndex];
            prefab.color = color[curIndex];
            prefab.velocity = velocity[curIndex];
            prefab.distance = distance[curIndex];//velocity;
            //prefab.transform.localScale = scale[currentTrialIndex] * Vector3.one

        }


        public void OnGUI() {
            EditorGUILayout.IntField("Maximum Angle",angle);
            EditorGUILayout.LabelField("Size of Trials:\t"+sizeOfTrials);
            EditorGUILayout.LabelField("Select trial to edit:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                trialsConfig.listOfTrials.Add(new Trial( "New Trial",1, Color.white, 0.5f, 5.0f, 10.0f, 1));
                ReinitTrials();
            }
            curIndex = EditorGUILayout.Popup(curIndex, trialsname);

            if (GUILayout.Button("-"))
            {
                trialsConfig.listOfTrials.RemoveAt(curIndex);
                ReinitTrials();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            trialsname[curIndex]= EditorGUILayout.TextField("Trial",trialsname[curIndex]);
            instances[curIndex] = EditorGUILayout.IntField("Instance", instances[curIndex]);
            color[curIndex] = EditorGUILayout.ColorField("Color",color[curIndex]);
            velocity[curIndex] = EditorGUILayout.FloatField("Velocity",velocity[curIndex]);
            distance[curIndex] = EditorGUILayout.FloatField("Distance",distance[curIndex]);
            numberOfParallelSpawns[curIndex] =EditorGUILayout.IntField("Parallel Spawns",numberOfParallelSpawns[curIndex]);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load JSON Config"))
            {
                var trialsConfigPath = EditorUtility.OpenFilePanel("Open trials config file (JSON)", expectedTrialsConfig, "json");
                trialsConfig = ConfigUtil.LoadConfig<TrialsList>(new FileInfo(trialsConfigPath), true, () =>
                  {
                      Debug.LogError("Something is wrong with the AppConfig. Was not found and I was not able to create one!");
                  });
                ReinitTrials();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply"))
            {
                if (NinjaGame.game != null)
                {
                    NinjaGame.game = new NinjaGame.GameInfo();
                    NinjaGame.game.setMaximumAngle(angle);
                    NinjaGame.game.setListOfTrials(trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials));
                }
                else
                {
                    Debug.LogError("Static GameInfo instance not found, create one!");
                    NinjaGame.game = new NinjaGame.GameInfo();
                    NinjaGame.game.setMaximumAngle(angle);
                    NinjaGame.game.setListOfTrials(trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials));
                }
            }
            if (GUILayout.Button("Save JSON Config As"))
            {
                SaveTrials();
                var trialsConfigPath = EditorUtility.SaveFilePanel("Save trials config file (JSON)",saveTrialsConfig, "trialslist", "json");
                ConfigUtil.SaveAsJson<TrialsList>(new FileInfo(trialsConfigPath), trialsConfig);
            }
            EditorGUILayout.EndHorizontal();

            }

    }
}

