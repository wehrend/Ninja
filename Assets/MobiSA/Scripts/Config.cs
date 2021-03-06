﻿using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.IO;
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
        // if path empty , its uses the Project ordner in editor mode and the directory of exe in default mode
        // else it creates the  given directory, 

        //Todo: on standalone build copy the defaultconfig, replace the name with the standalone build path,in the json generation
        public String capturesStorePath;
        public String environmentFilePath;
        public List<String> listOfBlocknames;
    }
    //Advanced setup
    [Serializable]
    public class Advanced
    {
        public Boolean displaySmiGazeCursor;
        public Boolean useAudio;
        public Boolean useForceFeedback;

        public Advanced() {
            this.displaySmiGazeCursor = false;
            this.useAudio = true;
            this.useForceFeedback = true;
        }
        
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

        public InstructionLists() {
            this.permanentWallTextonTop = new Instruction();
            this.permanentWallTextonTop.instruction = "Join VR and have some fun";
            this.permanentWallTextonTop.fontsize = 100;
            this.permanentWallTextonTop.color = Color.white;

            this.preBaseline = new List<Instruction>();
            var prebaseInst = new Instruction("Waiting for Baseline", Color.white, 100);
            this.preBaseline.Add(prebaseInst);
            this.onBaseline = new List<Instruction>();
            var onbaseInst = new Instruction("Baseline Recording.\nPlease follow the instructions of lab assistant.", Color.red, 100);
            this.onBaseline.Add(onbaseInst);
            this.postBaseline = new List<Instruction>();
            var postbaseInst = new Instruction("You have a start score of one coin. Don't lose it!\n\n\nPlease press the trigger button, to start the experiment...", Color.white, 100);
            this.postBaseline.Add(postbaseInst);
            this.postExperiment = new List<Instruction>();
            this.onExperiment = new List<Instruction>();
            var onexpInst = new Instruction("Experiment running.", Color.green, 80);
            this.onExperiment.Add(onexpInst);
            var postexpInst = new Instruction("Ok. Experiment is finished. Thanks for being part of it!", Color.blue, 100);
            this.postExperiment.Add(postexpInst);
        }
    }

    [Serializable]
    public class Instruction
    {
        public String instruction;
        public int fontsize;
        public Color color;
        private Color white;

        public Instruction() {
        }

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
        public int baselineDuration;
        public int animationDuration;
        public String instructionsFilePath;
        public bool userinfoScore;
        public bool userinfoBlock;        
        public bool userinfoObjects;
  
        public bool userPostExperimentInfo;

        public Experiment() {
            this.baselineDuration = 30;
            this.animationDuration = 0;
            this.userinfoScore = false;
            this.userinfoBlock = true;
            this.userinfoObjects = false;
            this.userPostExperimentInfo = false;
        }
    }

    [Serializable]
    public class Block
    {
        //TODO adding instructions
        public int maximumAngle;
        public int parallelSpawns;
        public float pausetime;
        public float pausetimeTimingJitter;
        public float distractorDestroyDistance;
        public String name;
        public List<Trial> listOfTrials;
        public int blockPausetime;
        public List<Trial> generatedTrials = new List<Trial>();
        public int trialsMax;
       

        public Block(string name, List<Trial> trialslist, int pausetime) {
            this.name = name;
            this.listOfTrials = trialslist;
            this.blockPausetime = pausetime;
            this.maximumAngle = 260;
            this.parallelSpawns = 2;
            this.pausetime = 5.0f;
            this.pausetimeTimingJitter = 0.25f;
            this.distractorDestroyDistance = 1.0f;
        }


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

        public static Trial PickAndDelete(Block curBlock)
        {
            //System.Random rand;
            Trial selected;
            int index;
            int minVal = 0;
            System.Random rand = new System.Random();

            if (curBlock.generatedTrials.Count != 0)
            {
                index = rand.Next(minVal, curBlock.generatedTrials.Count - 1);
                //Debug.Log("index: " + index + "ListCount: " + (trialsList.Count - 1));
                selected = curBlock.generatedTrials[index];
                curBlock.generatedTrials.RemoveAt(index);
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
        public Advanced advanced = new Advanced();
        public InstructionLists instructionlists = new InstructionLists();
        public Experiment experiment = new Experiment();

        public List<Block> blocks = new List<Block>() {
            new Block("first Level", firstTrialsSet(), 3),
            new Block("second Level", secondTrialsSet(), 5),
           //new Block("third Level", new List<Trial>(), 5)
        };


        public static List<Trial> firstTrialsSet()
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

        public static List<Trial> secondTrialsSet()
        {
            List<Trial> trialsList = new List<Trial>();
            Trial target = new Trial();
            target.instances = 15;
            target.trial = "Target";
            target.color = Color.blue;
            target.heigth = 1.4f;
            target.scaleAvg = 0.6f;
            target.scaleVar = 0.1f;
            target.velocityAvg = 4.0f;
            target.velocityVar = 0.5f;
            target.distanceAvg = 8.0f;
            target.distanceVar = 2.0f;
            trialsList.Add(target);
            Trial distract = new Trial();
            distract.instances = 15;
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
       
    }
}
