using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Assets.NinjaGame.Scripts
{
    public class ScoreAndStats : MonoBehaviour
    {
        public Text instructionText;
        public Text scoresText;
        public Scrollbar healthBar;
        public Scene scene;
      //  private NinjaGame.GameInfo scores; 
        [HideInInspector]
        private NinjaGameEventController ninjaGameEvent;
        [HideInInspector]
        public NinjaGameEventArgs eve;
        // Use this for initialization
        void Start()
        {
            scene = SceneManager.GetActiveScene();
            // if scene is "Empty_room"...
            //healthBar = GetComponent<Scrollbar>();
            ninjaGameEvent = FindObjectOfType(typeof(NinjaGameEventController)) as NinjaGameEventController;
           
            if (ninjaGameEvent == null)
            {
                Debug.LogError("The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            ninjaGameEvent.GameOver += new NinjaGameEventHandler(gameOver);
           
        }

        void StuffForBaseline()
        {
            if (instructionText)
                instructionText.text= "Baseline Recording. Please follow the instructions of lab assistant.";
            scoresText = GetComponent<Text>();
            if (scoresText)
                scoresText.text="0 Trials";
        }

        void Update()
        {
            if (scene.name.Contains("Empty_room"))
                StuffForBaseline();
            else
            {
                if (scoresText && NinjaGame.generatedTrials != null)
                    scoresText.text = "Counting Trials :\n" + NinjaGame.generatedTrials.Count;
                //healthBar.size = (float) NinjaGame.game.health / 1000f;
            }

            if ((NinjaGame.generatedTrials != null) && (NinjaGame.generatedTrials.Count == 0))
            {
                if (instructionText)
                {
                    instructionText.color = Color.blue;
                    instructionText.text = "Ok. Experiment is finished. Thanks for being part of it";
                }
            }
        }

        void gameOver(object sender, NinjaGameEventArgs eve)
        {
            instructionText.color = Color.red;
            instructionText.text = "Game Over!";
        }
    }
}