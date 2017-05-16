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
        public Config trialsConfig;
        private int curIndex;
        public string dataDirectory;
        int trialname;
        //Experiment properties
        int angle;
        int numberOfParallelSpawns;
        float pausetime;
        float pausetimeTimingJitter;
        /// <summary>
        /// Trials properties 
        /// </summary>
        int sizeOfTrials;
        int sizeOfGeneratedTrials;
        string[] trialsname;
        int[] instances;
        Color[] color;
        float[] velocityAvg;
        float[] velocityVar;
        float[] distanceAvg;
        float[] distanceVar;
        float[] scaleAvg;
        float[] scaleVar;
        float[] heigth;
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
                trialsConfig = ScriptableObject.CreateInstance(typeof(Config)) as Config;
                //load default trials config

                trialsConfig = ConfigUtil.LoadConfig<Config>(new FileInfo(expectedTrialsConfig), true, () =>
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
                trialsConfig = new Config();
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
                pausetimeTimingJitter = trialsConfig.experiment.pausetimeTimingJitter;
            ///Trials
            Debug.Log(trialsConfig.listOfTrials.Count); 
            sizeOfTrials = trialsConfig.listOfTrials.Count;
            trialsname = new string[sizeOfTrials];
            instances = new int[sizeOfTrials];
            color = new Color[sizeOfTrials];
            heigth = new float[sizeOfTrials];
            velocityAvg = new float[sizeOfTrials];
            velocityVar = new float[sizeOfTrials];
            distanceAvg = new float[sizeOfTrials];
            distanceVar = new float[sizeOfTrials];
            scaleAvg = new float[sizeOfTrials];
            scaleVar = new float[sizeOfTrials];

            trialsnameToSelect = new string[sizeOfTrials];
            for (int i = 0; i < sizeOfTrials; i++)
            {
                ///Trials
                trialsnameToSelect[i] = trialsConfig.listOfTrials[i].trial;
                instances[i] = trialsConfig.listOfTrials[i].instances;
                trialsname[i] = trialsConfig.listOfTrials[i].trial;
                color[i] = trialsConfig.listOfTrials[i].color;
                heigth[i] = trialsConfig.listOfTrials[i].heigth;
                scaleAvg[i] = trialsConfig.listOfTrials[i].scaleAvg;
                scaleVar[i] = trialsConfig.listOfTrials[i].scaleVar;
                velocityAvg[i] = trialsConfig.listOfTrials[i].velocityAvg;
                velocityVar[i] = trialsConfig.listOfTrials[i].velocityVar;
                distanceAvg[i] = trialsConfig.listOfTrials[i].distanceAvg;
                distanceVar[i] = trialsConfig.listOfTrials[i].distanceVar;
                sizeOfGeneratedTrials =+ instances[i];

            }
        }

        private void SaveTrials()
        {  
            //Experiment
            trialsConfig.experiment.maximumAngle = angle;
            trialsConfig.experiment.parallelSpawns = numberOfParallelSpawns;
            trialsConfig.experiment.pausetime = pausetime;
            trialsConfig.experiment.pausetimeTimingJitter = pausetimeTimingJitter;
            //Trials
            for (int i = 0; i < sizeOfTrials; i++)
            {
                trialsConfig.listOfTrials[i].trial = trialsnameToSelect[i];
                trialsConfig.listOfTrials[i].trial = trialsname[i];
                trialsConfig.listOfTrials[i].instances = instances[i];
                trialsConfig.listOfTrials[i].color = color[i];
                trialsConfig.listOfTrials[i].heigth = heigth[i];
                trialsConfig.listOfTrials[i].scaleAvg = scaleAvg[i];
                trialsConfig.listOfTrials[i].scaleAvg = scaleVar[i];
                trialsConfig.listOfTrials[i].velocityAvg = velocityAvg[i];
                trialsConfig.listOfTrials[i].velocityVar = velocityVar[i];
                trialsConfig.listOfTrials[i].distanceAvg = distanceAvg[i];
                trialsConfig.listOfTrials[i].distanceVar = distanceVar[i];
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
            pausetimeTimingJitter = EditorGUILayout.FloatField("Pausetime (Jitter)", pausetimeTimingJitter);


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Trials properties:");
            EditorGUILayout.LabelField("Size of exemplaric Trials:\t" + trialsConfig.listOfTrials.Count);
            //EditorGUILayout.Space();
            //LabelField("Size of Generated Trials:\t" + trialsConfig.generatedTrials.Count);
            EditorGUILayout.LabelField("Select trial to edit:");
            EditorGUILayout.BeginHorizontal();
           /* if (GUILayout.Button("+"))
            {
                trialsConfig.listOfTrials.Add(new Trial("New Trial "+trialname, 1, Color.white, 0.5f, 5.0f, 10.0f));
                trialname++;
                ReinitTrials();
            }*/
            curIndex = EditorGUILayout.Popup(curIndex, trialsname);

            /*if (GUILayout.Button("-"))
            {
                //hacky trick
                Debug.Log(curIndex+","+ trialsConfig.listOfTrials.Capacity);
                trialsConfig.listOfTrials.RemoveAt(curIndex);

                ReinitTrials();
            }*/

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            //hacky trick #2
            if (curIndex < 0)
                curIndex = 1;

            trialsname[curIndex]= EditorGUILayout.TextField("Trial", trialsname[curIndex]);
            instances[curIndex] = EditorGUILayout.IntField("Instance", instances[curIndex]);
            color[curIndex] = EditorGUILayout.ColorField("Color", color[curIndex]);
            heigth[curIndex] = EditorGUILayout.FloatField("Heigth", heigth[curIndex]);
            EditorGUILayout.BeginHorizontal();
            scaleAvg[curIndex] = EditorGUILayout.FloatField("Scale (Avg)", scaleAvg[curIndex]);
            scaleVar[curIndex] = EditorGUILayout.FloatField("Scale (Var)", scaleVar[curIndex]);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            velocityAvg[curIndex] = EditorGUILayout.FloatField("Velocity (Avg)", velocityAvg[curIndex]);
            velocityVar[curIndex] = EditorGUILayout.FloatField("Velocity (Var)", velocityVar[curIndex]);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            distanceAvg[curIndex] = EditorGUILayout.FloatField("Distance (Avg)", distanceAvg[curIndex]);
            distanceVar[curIndex] = EditorGUILayout.FloatField("Distance (Var)", distanceVar[curIndex]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Load JSON Config"))
            {
                var trialsConfigPath = EditorUtility.OpenFilePanel("Open trials config file (JSON)", expectedTrialsConfig, "json");
                trialsConfig = ConfigUtil.LoadConfig<Config>(new FileInfo(trialsConfigPath), true, () =>
                  {
                      Debug.LogError("Something is wrong with the AppConfig. Was not found and I was not able to create one!");
                  });
                ReinitTrials();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply"))
            {
                if (NinjaGame.config != null)
                {
                    NinjaGame.config = new Config();
                    NinjaGame.config.experiment.maximumAngle = angle;
                    NinjaGame.config.experiment.parallelSpawns = numberOfParallelSpawns;
                    NinjaGame.config.experiment.pausetime = pausetime;
                    NinjaGame.config.experiment.pausetimeTimingJitter = pausetimeTimingJitter;

                    NinjaGame.generatedTrials = trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);
                }
                else
                {
                    Debug.LogError("Static GameInfo instance not found, create one!");
                    NinjaGame.config = new Config();
                    NinjaGame.config.experiment.maximumAngle = angle;
                    NinjaGame.config.experiment.parallelSpawns = numberOfParallelSpawns;
                    NinjaGame.config.experiment.pausetime = pausetime;
                    NinjaGame.config.experiment.pausetimeTimingJitter = pausetimeTimingJitter;
                    NinjaGame.generatedTrials=trialsConfig.GenerateTrialsList(trialsConfig.listOfTrials);
                }
            }
            if (GUILayout.Button("Save JSON Config As"))
            {
                SaveTrials();
                var trialsConfigPath = EditorUtility.SaveFilePanel("Save trials config file (JSON)", saveTrialsConfig, "trialslist", "json");
                ConfigUtil.SaveAsJson<Config>(new FileInfo(trialsConfigPath), trialsConfig);
            }
            EditorGUILayout.EndHorizontal();

        }

    }

}