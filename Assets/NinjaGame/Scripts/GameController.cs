using UnityEngine;
using System.Collections;
using VRTK;

namespace Assets.NinjaGame.Scripts
{
    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// we need the light for game over setting
        /// </summary>
        public Light light; 
        public int health;
        public int score;
        public bool gamePlaying;
        public NinjaGameEventController con;
        public NinjaGameEventArgs eve;



        # region event system
        //////////////////////////////////
        // Event System
        void Start()
        {

            health = 1000;
            score = 0;
            
            con = GetComponent<NinjaGameEventController>();
            light = FindObjectOfType<Light>();
            light.enabled = true;
            if (con== null)
            {
                Debug.LogError("The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            GetComponent<NinjaGameEventController>().CollisionWithFruit += new NinjaGameEventHandler(fruitCollision);
            GetComponent<NinjaGameEventController>().CollisionWithBomb += new NinjaGameEventHandler(bombCollision);
            GetComponent<NinjaGameEventController>().UpdateScore += new NinjaGameEventHandler(updateScore);
            GetComponent<NinjaGameEventController>().UpdateHealth += new NinjaGameEventHandler(updateHealth);
            GetComponent<NinjaGameEventController>().StartGame += new NinjaGameEventHandler(StartGame);
            GetComponent<NinjaGameEventController>().GameOver += new NinjaGameEventHandler(GameOver);
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
            score = eve.totalscore;

        }

        void updateHealth(object sender, NinjaGameEventArgs eve)
        {

            health = eve.health;
        }
        void StartGame(object sender, NinjaGameEventArgs eve)
        {
            Debug.Log("Start Game");
        }

        void GameOver(object sender, NinjaGameEventArgs eve)
        {
            Debug.Log("GameOver");
            light.enabled = false;
        }

        void Update()
        {
            if (health < 3)
                GetComponent<NinjaGameEventController>().TriggerGameOver(eve);

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
