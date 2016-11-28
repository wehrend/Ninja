using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{
    [Serializable]
    public class Trial
    {
        public string trial;
        //public object prefab;
        public Color color;
        public float scale;
        public float velocity;
        public float distance;
        public int numberOfSpawners;

        //Constructor for test purposes
        public Trial(string t, Color c, float s, float v, float d, int n)
        {
            //Trial Target or distract
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

    public class TrialsList :ScriptableObject
    { 
        //This is our list we want to use to represent our class
        private List<Trial> trialsList = new List<Trial>();

        TrialsList( List<Trial> list)
        {
            this.trialsList = list;
        }

            
    }

    }
