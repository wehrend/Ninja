using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using System.Collections;

namespace Assets.NinjaGame.Scripts
{
    [RequireComponent(typeof(NinjaGame.Info))]
    public class ScoreAndStats : MonoBehaviour
    {
        public SerializedObject so;
        public Text instructionText;
        public Text scoresText;
        public Scrollbar healthBar;
        //private GameController gameController;
        private NinjaGameEventController ninjaGameEvent;
        public NinjaGameEventArgs eve;
        // Use this for initialization
        void Start()
        {
            
            var totalscore = so.FindProperty("totalscore").intValue;
            var health = so.FindProperty("health").floatValue;
            ninjaGameEvent = FindObjectOfType(typeof(NinjaGameEventController)) as NinjaGameEventController;
           // ninjaGameEvent = GetComponent<NinjaGameEventController>();
            if (ninjaGameEvent != null)
            {
                ninjaGameEvent.GameOver += new NinjaGameEventHandler(gameOver);
                
            }
            //gameController = FindObjectOfType(typeof(GameController)) as GameController;
        }

        // Update is called once per frame
        void Update()
        {
           // so.Update();
            var totalscore = so.FindProperty("totalscore").intValue;
            var health = so.FindProperty("health").floatValue;
            instructionText.text = "Swing the Sword.\nCatch the blue spheres, and avoid the black bombs";
                scoresText.text = "Score:\n" + totalscore.ToString();//+ "\n Health:\n" + health.ToString();
                healthBar.size = (float) health / 1000f;
	                            

        }

        void gameOver(object sender, NinjaGameEventArgs eve)
        {
            info.health = 0;
            instructionText.color = Color.red;
            instructionText.text = "Game Over!";
            scoresText.text = "Score:\n" + info.totalscore.ToString(); //+ "\n Health:\n" + health.ToString();
        }
    }
}