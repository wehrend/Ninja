using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MonsterLove.StateMachine;
using Assets.LSL4Unity.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.NinjaGame.Scripts
{
    public class WallText : MonoBehaviour
    {
        public Text wallText;
        public Text instructionsText;
        public Scrollbar healthBar;
        public Scene scene;
        public InstructionLists instructionlists;
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

            /* Scene mainScene = SceneManager.GetSceneByName("MainScene");
             if (mainScene != null)
             {
                 var gos_expScene = mainScene.GetRootGameObjects();
                 Debug.Log("GameObjects:" + gos_expScene.Length);
                 foreach (var go in gos_expScene)
                 {
                     if (go.gameObject.GetComponent<ExperimentSceneController>())
                     {
                         expSceneCon = go.gameObject.GetComponent<ExperimentSceneController>();
                         //Debug.Log("Found" + expSceneCon);
                     }
                 }
             }
             else
             {
                 Debug.LogWarning("MainScene not found!");
             }*/
           
            var expGO = GameObject.Find("[ExperimentSceneController]");
            if (expGO)
            {
                expSceneCon = expGO.GetComponent<ExperimentSceneController>();
            }
            else {
                //Maybe it was loaded as single scene
                Debug.Log("No [ExperimentSceneController] found. Assume it was loaded as single scene");
            }
            expMarker = FindObjectOfType(typeof(LSLMarkerStream)) as LSLMarkerStream;

           
        }



        void Update()
        {
            if (expSceneCon)
            {

                // // aka if no experiment scene loaded 
                if (expSceneCon.sceneFsm.State == ExperimentSceneController.SceneStates.PreScene)
                {

                    // Debug.Log("Found expscenecontroller");
                    if (instructionsText)
                    {
                        if (expSceneCon.recordingflag)
                        {
                            showInstructions(instructionlists.onBaseline, "onBaseline");
                            // "Baseline Recording.\nPlease follow the instructions of lab assistant.";
                        }
                        else if((!expSceneCon.recordingflag) && (expSceneCon.initflag) ) //back top normal (whit, not text)
                        {
                            showInstructions(instructionlists.postBaseline, "postBaseline");
                            // "Now please press the trigger button, to start the experiment...";
                        }
                    }
                }
                else if (expSceneCon.sceneFsm.State == ExperimentSceneController.SceneStates.ExperimentScene)
                {
                    // aka experiment scene is loaded
                    if (instructionsText && NinjaGame.generatedTrials != null)
                        showInstructions(instructionlists.onExperiment, "onExperiment");
                
                        instructionsText.text += "\n" + NinjaGame.generatedTrials.Count;
                    //healthBar.size = (float) NinjaGame.game.health / 1000f;
                }
            
                else if (expSceneCon.sceneFsm.State == ExperimentSceneController.SceneStates.PostScene)
                {
                    if ((instructionsText) && (!expSceneCon.waitflag)) 
                    {
                        showInstructions(instructionlists.postExperiment, "postExperiment");
                        //"Ok. Experiment is finished. Thanks for being part of it";
                    }
                }
           }
       }

        void showInstructions(List<Instruction> instructionList,string name)
        {
            if ( instructionList != null && instructionList.Any())
            {
               // Debug.LogWarning("Instructions list"+instructionList.Count());
                foreach (Instruction inst in instructionList)
                {

                    instructionsText.text = inst.instruction;
                    instructionsText.color = inst.color;
                    //StartCoroutine(WaitForAnyKey());
                  }
                } else{
                instructionsText.color = Color.red;
                instructionsText.text = name +": No instruction(s) available";
                Debug.Log("No instructions list");
            }
        }

        IEnumerator WaitForAnyKey()
        {
            while (!Input.anyKey) {
                yield return null;
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