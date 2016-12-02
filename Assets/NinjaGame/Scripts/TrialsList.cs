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
        public int numberOfSpawners;

        //Constructor for test purposes
        public Trial(int i, string t, Color c, float s, float v, float d, int n)
        {
            //Trial Target or distract
            this.instances = i;
            this.trial = t;
            //this.prefab = p;
            this.color = c;
            this.scale = s;
            this.velocity = v;
            this.distance = d;
            this.numberOfSpawners = n;
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

    public class TrialsList : ScriptableObject
    {
        //needs to  be rewritten later on

        //private List<Trial> generatedTrails;
        public  List<Trial> testTrial = new List<Trial>();
        public List<Trial> listOfTrials = new List<Trial>();

        public List<Trial> buildTestTrial()
        {
            
            testTrial.Add(new Trial(50,"Distract", Color.red, 0.3f, 4.0f, 3.8f, 1));
            testTrial.Add(new Trial(50,"Target", Color.green, 0.4f, 5.0f, 8.0f, 1));
            testTrial.Add(new Trial(5,"Distract2", Color.red, 0.3f, 4.0f, 9.0f, 1));
            testTrial.Add(new Trial(5,"Target2", Color.green, 0.4f, 5.0f, 8.0f, 2));
            testTrial.Add(new Trial(5,"Distract3", Color.red, 0.5f, 3.0f, 6.5f, 2));
            testTrial.Add(new Trial(5,"Target3", Color.green, 0.6f, 2.5f, 8.0f, 1));
            testTrial.Add(new Trial(3,"Distract", Color.red, 0.4f, 5.0f, 5.0f, 1));
            testTrial.Add(new Trial(3,"Other", Color.blue, 0.6f, 2.5f, 8.0f, 1));
            testTrial.Add(new Trial(3,"Other2", Color.black, 0.4f, 5.0f, 5.0f, 1));
            return testTrial;
        }


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
