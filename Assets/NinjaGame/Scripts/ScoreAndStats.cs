using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using System.Collections;

namespace Assets.NinjaGame.Scripts
{

    public class ScoreAndStats : MonoBehaviour
    {


        public Text instructionText;
        public Text scoresText;
        public Scrollbar healthBar;
        private int health;
        private GameController gameController;
        public NinjaGameEventController ninjaGameEvent;
        public NinjaGameEventArgs eve;
        private int score;
        // Use this for initialization
        void Start()
        {
            ninjaGameEvent = GetComponent<NinjaGameEventController>();
            if (ninjaGameEvent != null)
            {
                ninjaGameEvent.GameOver += new NinjaGameEventHandler(gameOver);
            }
            gameController = FindObjectOfType(typeof(GameController)) as GameController;
        }

        /*

        void UpdateHealth(object sender, NinjaGameEventArgs eve)
        {
            scoresText.text = "Score:\n" + score.ToString() + "\n Health:\n" + health.ToString();
            if (health < 5)
            {
                score = 0;
                instructionText.color = Color.red;
                instructionText.text = "Game Over!";
            }
        }

        void UpdateScore(object sender, NinjaGameEventArgs eve)
        {
            scoresText.text = "Score:\n" + score.ToString() + "\n Health:\n" + health.ToString();
        }*/

        // Update is called once per frame
        void Update()
        {
            if (gameController != null)
            {
                instructionText.text = "Swing the Sword.\nCatch the blue spheres, and avoid the black bombs";
                score = gameController.getScores();
                health = gameController.getHealth();
                scoresText.text = "Score:\n" + score.ToString();//+ "\n Health:\n" + health.ToString();
                healthBar.size = (float) health / 1000f;
	                            
            }
            else
            {
                Debug.LogError("GameController not found!");
            }

        }

        void gameOver(object sender, NinjaGameEventArgs eve)
        {
            health = 0;
            instructionText.color = Color.red;
            instructionText.text = "Game Over!";
            scoresText.text = "Score:\n" + score.ToString(); //+ "\n Health:\n" + health.ToString();
        }
    }
}