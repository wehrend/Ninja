using UnityEngine;
using System.Collections;
using VRTK;

namespace Assets.NinjaGame.Scripts
{
    public class GameController : MonoBehaviour
    {

        public int health;
        public int score;
        public void HappenWhenSwordIsGrabbed(GameObject grabbedObject)
        {
            Debug.Log("Object has been grabbed");
        }

        // Use this for initialization
        void Start()
        {
            health = 1000;
            score = 0;
        }

        public void issueDamage(int damage)
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

        public int getHealth()
        {
            return health;
        }

    }
}
