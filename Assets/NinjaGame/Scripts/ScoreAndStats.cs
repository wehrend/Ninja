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
        private static ExperimentSceneController expSceneCon;

      //  private NinjaGame.GameInfo scores; 
        [HideInInspector]
        private NinjaGameEventController ninjaGameEvent;
        [HideInInspector]
        public NinjaGameEventArgs eve;
        private LSLMarkerStream expMarker;
        private bool initflag, recordingflag, endflag;
        // Use this for initialization

        void Start()
        {
            scene = SceneManager.GetActiveScene();
            recordingflag = false;
            //healthBar = GetComponent<Scrollbar>();
            //ninjaGameEvent = FindObjectOfType(typeof(NinjaGameEventController)) as NinjaGameEventController;
           
          /*  if (ninjaGameEvent == null)
            {
                Debug.LogError("The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }*/
           // ninjaGameEvent.FruitCollision += new NinjaGameEventHandler(updateFruitCollision);
           // ninjaGameEvent.BombCollision += new NinjaGameEventHandler(updateBombCollision);
           // ninjaGameEvent.GameOver += new NinjaGameEventHandler(gameOver);
            var gos_expScene = SceneManager.GetSceneByName("MainScene").GetRootGameObjects();
            Debug.Log("GameObjects:"+gos_expScene.Length);
            foreach (var go in gos_expScene)
                {
                if (go.gameObject.GetComponent<ExperimentSceneController>())
                {
                    expSceneCon = go.gameObject.GetComponent<ExperimentSceneController>();
                    //Debug.Log("Found" + expSceneCon);
                }
            }
               
            expMarker = FindObjectOfType(typeof(LSLMarkerStream)) as LSLMarkerStream;
        }



        void Update()
        {

            // aka if no experiment scene loaded 
            if (NinjaGame.generatedTrials == null)
            {

                // Debug.Log("Found expscenecontroller");
                if (expSceneCon && (scoresText))
                {
                    if (expSceneCon.recordingflag)
                    {
                        scoresText.color = Color.red;
                        scoresText.text = "Baseline Recording.\nPlease follow the instructions of lab assistant.";
                    }
                    else //back top normal (whit, not text)
                    {
                        scoresText.color = Color.white;
                        scoresText.text = "";
                    }
                }
            }
            else if (NinjaGame.generatedTrials != null)
            {
                // aka experiment scene is loaded
                if (scoresText && NinjaGame.generatedTrials != null)
                    scoresText.text = "Counting Trials :\n" + NinjaGame.generatedTrials.Count;
                //healthBar.size = (float) NinjaGame.game.health / 1000f;

                if (scoresText && !expSceneCon)
                {
                    scoresText.color = Color.blue;
                    scoresText.text = "Ok. Experiment is finished. Thanks for being part of it";
                }
            }
       }
        

       /* void updateFruitCollision(object sender, NinjaGameEventArgs eve) {
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
        }*/
    }
}