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



        # region event system
        //////////////////////////////////
        // Event System
        void Start()
        {

            health = 1000;
            score = 0;
            if (GetComponent<NinjaGameEventController>() == null)
            {
                Debug.LogError("The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            GetComponent<NinjaGameEventController>().CollisionWithFruit += new NinjaGameEventHandler(fruitCollision);
            GetComponent<NinjaGameEventController>().CollisionWithBomb  += new NinjaGameEventHandler(bombCollision);
            GetComponent<NinjaGameEventController>().UpdateScore    += new NinjaGameEventHandler(updateScore);
            GetComponent<NinjaGameEventController>().UpdateHealth   += new NinjaGameEventHandler(updateHealth);


        }

        void fruitCollision(object sender, NinjaGameEventArgs eve)
        {
            score += eve.score;
        }

        void bombCollision(object sender, NinjaGameEventArgs eve)
        {
            health -= eve.damage;
        }

        void updateScore(object sender, NinjaGameEventArgs eve)
        {
            score = eve.score;
        }

        void updateHealth(object sender, NinjaGameEventArgs eve)
        {
            health = eve.health;
        }

        #endregion
        /////////////////////////////////////////////
        // classic system
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
