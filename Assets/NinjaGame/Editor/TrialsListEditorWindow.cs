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
        private int currentTrialIndex;
        public string dataDirectory;

        /// <summary>
        /// 
        /// </summary>
        int sizeOfTrials;
        string[] trialsname;
        int[] instances;
        Color[] color;
        float[] velocity;
        float[] distance;
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
                NinjaGame.game = new NinjaGame.GameInfo();
                NinjaGame.game.trialsList = trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);
            } else {
                Debug.LogError("Static GameInfo instance not found, create one!");
                NinjaGame.game = new NinjaGame.GameInfo();
                NinjaGame.game.trialsList = trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);
            }
        }

        private void ReinitTrials()
        {
            sizeOfTrials = trialsConfig.listOfTrials.Count;
            trialsname = new string[sizeOfTrials];
            instances = new int[sizeOfTrials];
            color = new Color[sizeOfTrials];
            velocity = new float[sizeOfTrials];
            distance = new float[sizeOfTrials];
            numberOfParallelSpawns = new int[sizeOfTrials];
            trialsnameToSelect = new string[sizeOfTrials];

            for (int i = 0; i < sizeOfTrials; i++)
            {
                trialsname[i] = trialsConfig.listOfTrials[i].trial;
                trialsnameToSelect[i] = trialsConfig.listOfTrials[i].trial;
                instances[i] = trialsConfig.listOfTrials[i].instances;
                color[i] = trialsConfig.listOfTrials[i].color;
                velocity[i] = trialsConfig.listOfTrials[i].velocity;
                distance[i] = trialsConfig.listOfTrials[i].distance;
            }
        }

        private void SaveTrials()
        {
            for (int i = 0; i < sizeOfTrials; i++)
            {
                trialsConfig.listOfTrials[i].trial = trialsname[i];
                trialsConfig.listOfTrials[i].trial = trialsnameToSelect[i];
                trialsConfig.listOfTrials[i].instances = instances[i];
                trialsConfig.listOfTrials[i].color = color[i];
                trialsConfig.listOfTrials[i].velocity = velocity[i];
                trialsConfig.listOfTrials[i].distance = distance[i];
            }
        }



        public void Update()
        {
            Repaint();
        }



        public void OnGUI() {
            EditorGUILayout.LabelField("Size of Trials:\t"+sizeOfTrials);
            EditorGUILayout.LabelField("Select trial to edit:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                trialsConfig.listOfTrials.Add(new Trial(1, "New Trial", Color.white, 0.5f, 5.0f, 10.0f, 1));
                ReinitTrials();
            }
            currentTrialIndex = EditorGUILayout.Popup(currentTrialIndex, trialsname);

            if (GUILayout.Button("-"))
            {
                trialsConfig.listOfTrials.RemoveAt(currentTrialIndex);
                ReinitTrials();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            trialsname[currentTrialIndex]= EditorGUILayout.TextField("Trial",trialsname[currentTrialIndex]);
            instances[currentTrialIndex] = EditorGUILayout.IntField("Instance",instances[currentTrialIndex]);
            color[currentTrialIndex] = EditorGUILayout.ColorField("Color",color[currentTrialIndex]);
            velocity[currentTrialIndex] = EditorGUILayout.FloatField("Velocity",velocity[currentTrialIndex]);
            distance[currentTrialIndex] = EditorGUILayout.FloatField("Distance",distance[currentTrialIndex]);
            numberOfParallelSpawns[currentTrialIndex] =EditorGUILayout.IntField("Parallel Spawns",numberOfParallelSpawns[currentTrialIndex]);
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
                TrialsListEditorWindow.Destroy(this);
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

