
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Assets.MobiSA.Scripts;

    public class ConfigEditor {
        private static Config configAsset;


    [MenuItem("MoBI SA/EditConfigAsset")]
        public static void Create() {

            string assetPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/DefaultConfig.asset");

            //Generate default config asset
            configAsset = CreateInstance<Config>();
            //Default setup

            Setup setup = new Setup();

            //default instructions
            InstructionLists instructionLists = new InstructionLists();


            //default experiment
            Experiment experiment = new Experiment();
            //default blocks with default trials 
            List<Block> blocks = new List<Block>(3){
                new Block("first Level", firstTrialsSet(), 3),
                new Block("second Level", secondTrialsSet(), 5),
                new Block("third Level", new List<Trial>(), 5)
                };

            configAsset.blocks = blocks;
            if (configAsset != null)
            {
                Debug.Log("AssetPathName: " + assetPathName);
                AssetDatabase.CreateAsset(configAsset, assetPathName);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("MoBI SA/Export(default) Config to JSON")]
        public static void ExportToJSON() {
            string json = JsonUtility.ToJson(configAsset, true);
            string path = "Assets/DefaultConfig.txt";
            Debug.Log("Export to " + path);
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.Write(json);
            writer.Close();
            writer.Dispose();
        }


    public static List<Trial> firstTrialsSet()
    {
        List<Trial> trialsList = new List<Trial>();
        Trial target = new Trial();
        target.instances = 25;
        target.trial = "Target";
        target.color = Color.green;
        target.heigth = 1.4f;
        target.scaleAvg = 0.6f;
        target.scaleVar = 0.1f;
        target.velocityAvg = 4.0f;
        target.velocityVar = 0.5f;
        target.distanceAvg = 8.0f;
        target.distanceVar = 2.0f;
        trialsList.Add(target);
        Trial distract = new Trial();
        distract.instances = 25;
        distract.trial = "Distract";
        distract.color = Color.red;
        distract.heigth = 1.6f;
        distract.scaleAvg = 0.3f;
        distract.scaleVar = 0.01f;
        distract.velocityAvg = 5.0f;
        distract.velocityVar = 0.5f;
        distract.distanceAvg = 9.0f;
        distract.distanceVar = 3.0f;
        trialsList.Add(distract);

        return trialsList;
    }

    public static List<Trial> secondTrialsSet()
    {
        List<Trial> trialsList = new List<Trial>();
        Trial target = new Trial();
        target.instances = 5;
        target.trial = "Target";
        target.color = Color.green;
        target.heigth = 1.4f;
        target.scaleAvg = 0.6f;
        target.scaleVar = 0.1f;
        target.velocityAvg = 2.0f;
        target.velocityVar = 0.5f;
        target.distanceAvg = 8.0f;
        target.distanceVar = 2.0f;
        trialsList.Add(target);
        Trial distract = new Trial();
        distract.instances = 5;
        distract.trial = "Distract";
        distract.color = Color.red;
        distract.heigth = 1.6f;
        distract.scaleAvg = 0.3f;
        distract.scaleVar = 0.01f;
        distract.velocityAvg = 3.0f;
        distract.velocityVar = 0.5f;
        distract.distanceAvg = 9.0f;
        distract.distanceVar = 3.0f;
        trialsList.Add(distract);

        return trialsList;
    }
}


