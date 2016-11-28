using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{
    public class TrialsList : MonoBehaviour
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
                //return this;
            }


      }

            //This is our list we want to use to represent our class as an array.
            public  List<Trial> trialsList = new List<Trial>(5);

           

            public void Add(Trial trial)
            {
                //Add a new index position to the end of our list
                trialsList.Add(trial);
            }

            

            public void Remove(int index)
            {
                //Remove an index position from our list at a point in our list array
                trialsList.RemoveAt(index);
            }

        public int Count()
        {
            return this.Count();
        }

        public Trial getElement(TrialsList trialslist, int index)
        {
            return trialslist[index];
        }

        public static Trial PickAndDelete(TrialsList trialsList)
           
        {
            System.Random r = new System.Random();
            int index = r.Next(0, trialsList.Count() - 1);
            //Trial selected = trialsList[index];
            trialsList.Remove(index);

            return selected;
        }
    }
    }

}