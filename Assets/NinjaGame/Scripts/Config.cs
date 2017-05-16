using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Assets.NinjaGame.Scripts
{
    #region Data classes

    //Setup class
   /* [Serializable]
    public class Setup
    {
        public float timeUntilBaseline;
        public float calibrationSceneTime;
        public float baselineDuration;

    }*/

    //Experiment class
    [Serializable]
    public class Experiment
    {
        public int maximumAngle;
        public int parallelSpawns;
        public float pausetime;
        public float pausetimeTimingJitter;
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
        //public Setup setup = new Setup();
 
        public Experiment experiment = new Experiment();
        //exemplaric Trials
        public List<Trial> listOfTrials = new List<Trial>();
       // public List<Trial> generatedTrials = new List<Trial>();

       /* public void setListOfTrials(List<Trial> trials)
        {
            this.listOfTrials = trials;
        }*/
        public List<Trial> GenerateTrialsList( List<Trial> exemplaricBaseTrials )
        {
            List<Trial> trials= new List<Trial>();

            foreach (Trial e in exemplaricBaseTrials)
                {
                for (int i = 0; i < e.instances; i++)
                    trials.Add(e);
                }
            return trials;
        }

            
    }

}
