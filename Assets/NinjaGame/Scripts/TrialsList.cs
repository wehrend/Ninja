using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Assets.NinjaGame.Scripts
{
    [Serializable]
    public class Trial
    {
        
        public int instances;
        public string trial;
        //public object prefab;
        public Color color;
        public float scale;
        public float velocity;
        public float distance;

        //Constructor for test purposes
        public Trial( string t, int i, Color c, float s, float v, float d)
        {
            //Trial Target or distract
            this.trial = t;
            this.instances = i;
            this.color = c;
            this.scale = s;
            this.velocity = v;
            this.distance = d;
        }

        public static Trial PickAndDelete(List<Trial> trialsList)
        {
            System.Random r = new System.Random();
            int index = r.Next(0, trialsList.Count - 1);
            Trial selected = trialsList[index];
            trialsList.RemoveAt(index);

            return selected;
        }
    }

    [Serializable]
    public class Experiment
    {
        //Experiment
        public int maximumAngle;
        public int parallelSpawns;
        public float pausetime;
    }

    public class TrialsList : ScriptableObject
    {

 
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
                for (int i = 0; i < e.instances - 1; i++)
                    trials.Add(e);
                }
            return trials;
        }

            
    }

}
