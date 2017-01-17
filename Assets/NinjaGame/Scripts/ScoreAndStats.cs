using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.LSL4Unity.Scripts;
using System.Collections;

namespace Assets.NinjaGame.Scripts
{
    public class ScoreAndStats : MonoBehaviour
    {
        public Text instructionText;
        public Text scoresText;
        public Scrollbar healthBar;
        public Scene scene;
        public float userInitTime=15.00f;
        public float baselineDuration=180.00f;

      //  private NinjaGame.GameInfo scores; 
        [HideInInspector]
        private NinjaGameEventController ninjaGameEvent;
        [HideInInspector]
        public NinjaGameEventArgs eve;
        private LSLMarkerStream expMarker;
        private bool initflag, endflag;
        // Use this for initialization

        void Start()
        {
            scene = SceneManager.GetActiveScene();
            //healthBar = GetComponent<Scrollbar>();
            ninjaGameEvent = FindObjectOfType(typeof(NinjaGameEventController)) as NinjaGameEventController;
           
            if (ninjaGameEvent == null)
            {
                Debug.LogError("The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            ninjaGameEvent.FruitCollision += new NinjaGameEventHandler(updateFruitCollision);
            ninjaGameEvent.BombCollision += new NinjaGameEventHandler(updateBombCollision);
            ninjaGameEvent.GameOver += new NinjaGameEventHandler(gameOver);

            expMarker = FindObjectOfType(typeof(LSLMarkerStream)) as LSLMarkerStream;
            instructionText = GetComponent<Text>();
            scoresText = GetComponent<Text>();
        }



        void InitBaseline()
        {

            if (!initflag)
            {
                Debug.Log("Init baseline...");

                if (scoresText)
                {
                    Debug.Log("scores text");
                    scoresText.color = Color.red;
                    scoresText.text = "Baseline Recording.\nPlease follow the instructions of lab assistant.";
                }
                if (expMarker != null)
                {
                    expMarker.Write("begin_baseline_condition");
                    Debug.Log("begin_baseline_condition");
                }
                initflag = true;
            }

        }

        void EndBaseline() { 

            if (!endflag)
            {

                Debug.Log("End baseline...");
                if (expMarker != null)
                {
                    expMarker.Write("end_baseline_condition");
                    Debug.Log("end_baseline_condition");
                }
             
                endflag = true;
            }
        }


        void Update()
        {
            // aka if no experiment scene loaded 
            if (NinjaGame.generatedTrials == null)
            {
                if (Time.time > userInitTime)
                {
                    //Debug.Log(Time.time);
                    InitBaseline();

                    if ((Time.time - userInitTime) > baselineDuration)

                        EndBaseline();

                }
            }
            else
            {
                // aka experiment scene is loaded
                if (scoresText && NinjaGame.generatedTrials != null)
                    scoresText.text = "Counting Trials :\n" + NinjaGame.generatedTrials.Count;
                //healthBar.size = (float) NinjaGame.game.health / 1000f;

                if (NinjaGame.generatedTrials.Count == 0)
                {
                    if (scoresText)
                    {
                        scoresText.color = Color.blue;
                        scoresText.text = "Ok. Experiment is finished. Thanks for being part of it";
                    }
                }
            }
        }

        void updateFruitCollision(object sender, NinjaGameEventArgs eve) {
            if (scoresText && NinjaGame.generatedTrials != null)
                scoresText.text = "Counting Trials :\n" + NinjaGame.generatedTrials.Count +"Score:"+ eve.score;
        }

        void updateBombCollision(object sender, NinjaGameEventArgs eve)
        {
            if (scoresText && NinjaGame.generatedTrials != null)
            {
                eve.score = eve.score / 2;
                scoresText.text = "Counting Trials :\n" + NinjaGame.generatedTrials.Count + "Score:" + eve.score;
            }
        }


        void gameOver(object sender, NinjaGameEventArgs eve)
        {
            instructionText.color = Color.red;
            instructionText.text = "Game Over!";
        }
    }
}