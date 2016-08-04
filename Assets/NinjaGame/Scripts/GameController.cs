﻿using UnityEngine;
using System.Collections;

namespace Assets.NinjaGame.Scripts
{
    public class GameController : MonoBehaviour
    {

        public float health;
        public int score;

        public void HappenWhenSwordIsGrabbed(GameObject grabbedObject)
        {
            Debug.Log("Object has been grabbed");
        }

        // Use this for initialization
        void Start()
        {
            health = 1000f;
            score = 0;
        }

        public void issueDamage(float damage)
        {
            health -= damage;
        }


        public void issueBoni(int bonusPoints)
        {
            score += bonusPoints;
        }

        public int getScores()
        {
            return score;
        }

        public float getHealth()
        {
            return health;
        }

    }
}
