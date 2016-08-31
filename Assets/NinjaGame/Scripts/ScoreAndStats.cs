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
        private int score;
        // Use this for initialization
        void Start()
        {
            gameController = FindObjectOfType(typeof(GameController)) as GameController;
        }

        // Update is called once per frame
        void Update()
        {
            if (gameController != null)
            {
                instructionText.text = "Swing the Sword.\nCatch the blue spheres, and avoid the black bombs";
                score = gameController.getScores();
                health = gameController.getHealth();
                scoresText.text = "Score:\n" + score.ToString()+ "\n Health:\n"+health.ToString();
               // healthBar.size = health / 10;
                if (health < 5)
                {
                    score = 0;
                    instructionText.color = Color.red;
                    instructionText.text = "Game Over!";
                }
            }
            else
            {
                Debug.LogError("GameController not found!");
            }


        }
    }
}