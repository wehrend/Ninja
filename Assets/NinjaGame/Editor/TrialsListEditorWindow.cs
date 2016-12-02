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

        [MenuItem("NinjaGame / Editing Trials List ")]
        public static void Init()
        {
            var window = (TrialsListEditorWindow)EditorWindow.GetWindow(typeof(TrialsListEditorWindow));
            window.Show();

        }

        public void OnEnable()
        {
            dataDirectory = Application.dataPath + "/NinjaGame/Config/";
        }


        public void OnGUI() {
            if (trialsConfig == null)
            {
                trialsConfig = ScriptableObject.CreateInstance(typeof(TrialsList)) as TrialsList;
                trialsConfig.buildTestTrial();
            }
            string expectedTrialsConfig = dataDirectory + "trialslist.json";
            string saveTrialsConfig = dataDirectory + "trialslist.json";

            var sizeOfTrials = trialsConfig.testTrial.Count;
            string[] trialsname = new string[sizeOfTrials];
            int[] instances = new int[sizeOfTrials];
            Color[] color = new Color[sizeOfTrials];
            float[] velocity = new float[sizeOfTrials];
            float[] distance = new float[sizeOfTrials];
            int[] numberOfParallelSpawns = new int[sizeOfTrials];
            string[] trialsnameToSelect = new string[sizeOfTrials];

            trialsname[0] = "All Trials";
            for (int i = 0; i < sizeOfTrials; i++)
            {
                trialsname[i] = trialsConfig.testTrial[i].trial;
                trialsnameToSelect[i] = trialsConfig.testTrial[i].trial;
                instances[i] = trialsConfig.testTrial[i].instances;
                color[i] = trialsConfig.testTrial[i].color;
                velocity[i] = trialsConfig.testTrial[i].velocity;
                distance[i] = trialsConfig.testTrial[i].distance;

            }
            EditorGUILayout.LabelField("Select trial to edit:");
            currentTrialIndex = EditorGUILayout.Popup(currentTrialIndex, trialsname);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(trialsname[currentTrialIndex]);
            instances[currentTrialIndex] = EditorGUILayout.IntField(instances[currentTrialIndex]);
            color[currentTrialIndex] = EditorGUILayout.ColorField(color[currentTrialIndex]);
            velocity[currentTrialIndex] = EditorGUILayout.FloatField(velocity[currentTrialIndex]);
            EditorGUILayout.FloatField(distance[currentTrialIndex]);
            EditorGUILayout.IntField(numberOfParallelSpawns[currentTrialIndex]);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load JSON Config"))
                trialsConfig=ConfigUtil.LoadConfig<TrialsList>(new FileInfo(expectedTrialsConfig), true, () =>
                {
                    Debug.LogError("Something is wrong with the AppConfig. Was not found and I was not able to create one!");
                });
            EditorGUILayout.Space();
            if (GUILayout.Button("Save JSON Config"))
                ConfigUtil.SaveAsJson<TrialsList>(new FileInfo(saveTrialsConfig), trialsConfig);
            EditorGUILayout.EndHorizontal();

            //so.ApplyModifiedProperties();
            }
        }
    }

