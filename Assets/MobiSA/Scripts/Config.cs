using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


namespace Assets.MobiSA.Scripts
{
    #region Data classes

    /// <summary>
    /// Scheme of Config Version Number 1
    /// </summary>

    //Setup class
    [Serializable]
    public class Setup
    {
        //some hack to make it downwardscompatible
        public int? configVersionNumber;
        public Boolean releaseMode;
        public String videoFilePath;
        public String environmentFilePath;
        public List<String> listOfBlocknames; 
        public Boolean useForceFeedback;
    }

    //Instructions class
    [Serializable]
    public class InstructionLists
    {
        // and make this fallback / defaul        
        public Instruction permanentWallTextonTop;
        public List<Instruction> preBaseline;
        public List<Instruction> onBaseline;
        public List<Instruction> postBaseline;
        public List<Instruction> preExperiment;
        public List<Instruction> onExperiment;
        public List<Instruction> postExperiment;
     }
    [Serializable]
    public class Instruction
    {
        public String instruction;
        public int fontsize;
        public Color color;
        private Color white;

        public Instruction() { }

        public Instruction(string instruction, Color color, int fontsize)
        {
            this.instruction = instruction;
            this.color = color;
            this.fontsize = fontsize;
        }
    }


    //Experiment class
    [Serializable]
    public class Experiment
    {
        public int maximumAngle;
        public int parallelSpawns;
        public float pausetime;
        public float pausetimeTimingJitter;
        public int animationDuration;
        public String instructionsFilePath;
    }

    [Serializable]
    public class Block
    {
        //TODO adding instructions
        public String name;
        public List<Trial> listOfTrials;
        public int blockPausetime;
        public List<Trial> generatedTrials = new List<Trial>();
        public int trialsMax;

        public List<Trial> GenerateTrialsList(List<Trial> exemplaricBaseTrials)
        {
            List<Trial> trials = new List<Trial>();

            foreach (Trial e in exemplaricBaseTrials)
            {
                for (int i = 0; i < e.instances; i++)
                    trials.Add(e);

            }
            return trials;
        }
    }

    //Trial class
    [Serializable]
    public class Trial
    {
        
        public int instances;
        public string trial;
        //public object prefab;
        public Color color;
        public float heigth;
        public float scaleAvg;
        public float scaleVar;
        public float velocityAvg; //average
        public float velocityVar;
        public float distanceAvg;
        public float distanceVar;

        #endregion

        #region trialadminstration
        //Constructor for test purposes
        /*   public Trial( string t, int i, Color c, float s, float v, float d)
           {
               //Trial Target or distract
               this.trial = t;
               this.instances = i;
               this.color = c;
               this.scaleAvg = s;
               this.velocityAvg = v;
               this.distanceAvg = d;
           }*/

        public static Trial PickAndDelete(List<Trial> trialsList)
        {
            Trial selected;
            int index;
            int minVal = 0;
            System.Random r = new System.Random();
            if (trialsList.Count != 0)
            {
                index = r.Next(minVal, trialsList.Count - 1);
                //Debug.Log("index: " + index + "ListCount: " + (trialsList.Count - 1));
                selected = trialsList[index];
                trialsList.RemoveAt(index);
            }
            else
            {
                selected = null;
                Debug.Log("No Object");
            }

            return selected;
        }
    }
    #endregion

    public class Config : ScriptableObject
    {
        public Setup setup = new Setup();
        public InstructionLists instructionlists = new InstructionLists();
        public Experiment experiment = new Experiment();

        public List<Block> blocks = new List<Block>();
        private static Config configAsset;



        /* public void setListOfTrials(List<Trial> trials)
         {
             this.listOfTrials = trials;
         }*/

        #region editorstuff
#if UNITY_EDITOR
        [MenuItem("MoBI SA/EditConfigAsset")]
        public static void Create() {

            string assetPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/DefaultConfig.asset");

            //Generate default config asset
            configAsset = CreateInstance<Config>();
            //Default setup

            Setup setup = new Setup();
            setup.releaseMode = false;
            setup.videoFilePath = "C:\\Users\\sven\\Ninja2\\Assets\\NinjaCaptureStreams";
            setup.useForceFeedback = false;
            configAsset.setup = setup;
            //default instructions
            InstructionLists instructionLists = new InstructionLists();
            instructionLists.permanentWallTextonTop = new Instruction();
            instructionLists.permanentWallTextonTop.instruction = "Join VR and have some fun";
            instructionLists.permanentWallTextonTop.fontsize = 100;
            instructionLists.permanentWallTextonTop.color = Color.white;

            instructionLists.preBaseline = new List<Instruction>();
            var prebaseInst = new Instruction();
            prebaseInst.instruction = "Waiting for Baseline";
            prebaseInst.fontsize = 100;
            prebaseInst.color = Color.white;
            instructionLists.preBaseline.Add(prebaseInst);
            instructionLists.onBaseline = new List<Instruction>();
            var onbaseInst = new Instruction();
            onbaseInst = new Instruction();
            onbaseInst.instruction = "Baseline Recording.\\nPlease follow the instructions of lab assistant.";
            onbaseInst.fontsize = 100;
            onbaseInst.color = Color.red;
            instructionLists.onBaseline.Add(onbaseInst);
            instructionLists.postBaseline = new List<Instruction>();
            var postbaseInst = new Instruction();
            postbaseInst.instruction = "You have a start score of one coin. Don't lose it!\n\n\nPlease press the trigger button, to start the experiment...";
            postbaseInst.fontsize = 100;
            postbaseInst.color = Color.white;
            instructionLists.postBaseline.Add(postbaseInst);
            instructionLists.postExperiment = new List<Instruction>();
            instructionLists.onExperiment = new List<Instruction>();
            var onexpInst = new Instruction();
            onexpInst.instruction = "Experiment running...";
            onexpInst.fontsize = 80;
            onexpInst.color = Color.green;
            instructionLists.onExperiment.Add(onexpInst);
            configAsset.instructionlists = instructionLists;
            var postexpInst = new Instruction();
            postexpInst.instruction = "Ok. Experiment is finished. Thanks for being part of it";
            postexpInst.fontsize = 100;
            postexpInst.color = Color.blue;
            instructionLists.postExperiment.Add(postexpInst);
            configAsset.instructionlists = instructionLists;

            //default experiment
            configAsset.experiment.maximumAngle = 260;
            configAsset.experiment.parallelSpawns = 3;
            configAsset.experiment.pausetime = 5.0f;
            configAsset.experiment.pausetimeTimingJitter = 0.25f;
            configAsset.experiment.animationDuration = 0;

            //default blocks with default trials 
            List<Block> blocks = new List<Block>(3);
            Block block1 = new Block();
            block1.name = "first Level";
            block1.blockPausetime = 3;
            block1.listOfTrials = firstTrialsSet();
            blocks.Add(block1);
            Block block2 = new Block();
            block2.name = "second Level";
            block2.blockPausetime = 5;
            block2.listOfTrials = secondTrialsSet();
            blocks.Add(block2);
            Block block3 = new Block();
            block3.name = "third Level";
            block3.blockPausetime = 5;
            //block3.listOfTrials = thirdTrialsSet();
            blocks.Add(block3);
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
            string json = JsonUtility.ToJson(configAsset);
            string path = "Assets/DefaultConfig.txt";
            Debug.Log("Export to " + path);
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.Write(json);
            writer.Close();
            writer.Dispose(); 
        }

        public static List<Trial> firstTrialsSet() {
            List<Trial> trialsList = new List<Trial>();
            Trial target = new Trial();
            target.instances = 25;
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
#endif
#endregion
    }

}
