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
        public TrialsList trialsConfig;
        private int curIndex;
        public string dataDirectory;
        int trialname;
        //Experiment properties
        int angle;
        int numberOfParallelSpawns;
        float pausetime;
        /// <summary>
        /// Trials properties 
        /// </summary>
        int sizeOfTrials;
        int sizeOfGeneratedTrials;
        string[] trialsname;
        int[] instances;
        Color[] color;
        float[] velocity;
        float[] distance;
        float[] scale;
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
            trialname = 1;
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
                Debug.Log("Loaded Config");
                ReinitTrials();
            }


            if (trialsConfig != null)
            {
                // ReinitTrials();
                NinjaGame.generatedTrials = trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);

            } else {
                Debug.LogWarning("Static GameInfo instance not found, create one!");
                //ReinitTrials();
                trialsConfig = new TrialsList();
                NinjaGame.generatedTrials= trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);

            }
        }

        private void ReinitTrials()
        {
            sizeOfTrials = 0;
            if (trialsConfig != null)
                ///Experiment
                angle = trialsConfig.experiment.maximumAngle;
                numberOfParallelSpawns = trialsConfig.experiment.parallelSpawns;
                pausetime = trialsConfig.experiment.pausetime;
            ///Trials
            Debug.Log(trialsConfig.listOfTrials.Count); 
            sizeOfTrials = trialsConfig.listOfTrials.Count;
            trialsname = new string[sizeOfTrials];
            instances = new int[sizeOfTrials];
            color = new Color[sizeOfTrials];
            velocity = new float[sizeOfTrials];
            distance = new float[sizeOfTrials];
            scale = new float[sizeOfTrials];

            trialsnameToSelect = new string[sizeOfTrials];
            for (int i = 0; i < sizeOfTrials; i++)
            {
                ///Trials
                trialsnameToSelect[i] = trialsConfig.listOfTrials[i].trial;
                instances[i] = trialsConfig.listOfTrials[i].instances;
                trialsname[i] = trialsConfig.listOfTrials[i].trial;
                color[i] = trialsConfig.listOfTrials[i].color;
                scale[i] = trialsConfig.listOfTrials[i].scale;
                velocity[i] = trialsConfig.listOfTrials[i].velocity;
                distance[i] = trialsConfig.listOfTrials[i].distance;
                sizeOfGeneratedTrials =+ instances[i];

            }
        }

        private void SaveTrials()
        {  
            //Experiment
            trialsConfig.experiment.maximumAngle = angle;
            trialsConfig.experiment.parallelSpawns = numberOfParallelSpawns;
            trialsConfig.experiment.pausetime = pausetime; 
            //Trials
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



        public void OnGUI() {
            EditorGUILayout.LabelField("Experiment properties:");
            angle=EditorGUILayout.IntField("Maximum Angle", angle);
            numberOfParallelSpawns = EditorGUILayout.IntField("Parallel Spawns", numberOfParallelSpawns);
            pausetime = EditorGUILayout.FloatField("Pausetime", pausetime);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Trials properties:");
            EditorGUILayout.LabelField("Size of exemplaric Trials:\t" + trialsConfig.listOfTrials.Count);
            //EditorGUILayout.Space();
            //LabelField("Size of Generated Trials:\t" + trialsConfig.generatedTrials.Count);
            EditorGUILayout.LabelField("Select trial to edit:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                trialsConfig.listOfTrials.Add(new Trial("New Trial "+trialname, 1, Color.white, 0.5f, 5.0f, 10.0f));
                trialname++;
                ReinitTrials();
            }
            curIndex = EditorGUILayout.Popup(curIndex, trialsname);

            if (GUILayout.Button("-"))
            {
                //hacky trick
                Debug.Log(curIndex+","+ trialsConfig.listOfTrials.Capacity);
                trialsConfig.listOfTrials.RemoveAt(curIndex);

                ReinitTrials();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            //hacky trick #2
            if (curIndex < 0)
                curIndex = 1;

            trialsname[curIndex]= EditorGUILayout.TextField("Trial", trialsname[curIndex]);
            instances[curIndex] = EditorGUILayout.IntField("Instance", instances[curIndex]);
            color[curIndex] = EditorGUILayout.ColorField("Color", color[curIndex]);
            velocity[curIndex] = EditorGUILayout.FloatField("Velocity", velocity[curIndex]);
            distance[curIndex] = EditorGUILayout.FloatField("Distance", distance[curIndex]);
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
                if (NinjaGame.trialsConfig != null)
                {
                    NinjaGame.trialsConfig = new TrialsList();
                    NinjaGame.trialsConfig.experiment.maximumAngle = angle;
                    NinjaGame.trialsConfig.experiment.parallelSpawns = numberOfParallelSpawns;
                    NinjaGame.trialsConfig.experiment.pausetime = pausetime;

                    NinjaGame.generatedTrials = trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);
                }
                else
                {
                    Debug.LogError("Static GameInfo instance not found, create one!");
                    NinjaGame.trialsConfig = new TrialsList();
                    NinjaGame.trialsConfig.experiment.maximumAngle = angle;
                    NinjaGame.trialsConfig.experiment.parallelSpawns = numberOfParallelSpawns;
                    NinjaGame.trialsConfig.experiment.pausetime = pausetime;
                    NinjaGame.generatedTrials=trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);
                }
            }
            if (GUILayout.Button("Save JSON Config As"))
            {
                SaveTrials();
                var trialsConfigPath = EditorUtility.SaveFilePanel("Save trials config file (JSON)", saveTrialsConfig, "trialslist", "json");
                ConfigUtil.SaveAsJson<TrialsList>(new FileInfo(trialsConfigPath), trialsConfig);
            }
            EditorGUILayout.EndHorizontal();

        }

    }

}