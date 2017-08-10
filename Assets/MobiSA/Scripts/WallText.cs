using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MonsterLove.StateMachine;
using Assets.LSL4Unity.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.MobiSA.Scripts
{
    public class WallText : MonoBehaviour
    {
        public Text wallText;
        public Text instructionsText;

        public GameObject money;
        public GameObject smallMoney;
        public int score;
        public float scoreProgressValue;

        public List<GameObject> moneyClones;
        public Scrollbar healthBar;
        public Scene scene;
        public InstructionLists instructionlists;
        private static ExperimentSceneController expSceneCon;

        //  private NinjaGame.GameInfo scores; 
        [HideInInspector]
        private MobiSACoreEventController ninjaEvent;
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
                instructionlists = expSceneCon.configValues.instructionlists;
                //Debug.LogWarning("["+scene.name +"] Count of loaded instructionlists " + instructionlists.onBaseline.Count());
                //Debug.LogWarning("[" + scene.name + "] instructionlists " + instructionlists.onBaseline.ToString());
            }
            else {
                //Maybe it was loaded as single scene
                Debug.Log("No [ExperimentSceneController] found. Assume it was loaded as single scene");
            }
            expMarker = FindObjectOfType(typeof(LSLMarkerStream)) as LSLMarkerStream;

            Image scoreProgress = GetComponentInChildren<Image>();
            if (scoreProgress != null)
            {
                Debug.Log("Found " + scoreProgress.name);
                scoreProgress.fillAmount = 0.0f;
            }
            money = Resources.Load("Money") as GameObject;
            if (money)
            {
                Debug.Log("Load resource" + money.name);
            }


            ninjaEvent = gameObject.GetComponent<MobiSACoreEventController>();
            if (ninjaEvent == null)
            {
                Debug.LogError("["+this.GetType().Name+"] The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            ninjaEvent.FruitCollision += new MobiSACoreEventHandler(incrementScore);
            ninjaEvent.BombCollision += new MobiSACoreEventHandler(shrinkScore);
            score = 10; //you get a start score of 10 to get an effect for red trials to begin.


        }



        void incrementScore(object sender, MobiSACoreEventArgs eve)
        {
            score = score + 1;
            showMoneyAfterScoreUpdate();
        }

        void shrinkScore(object sender, MobiSACoreEventArgs eve)
        {
            score = score / 2;
            showMoneyAfterScoreUpdate();
        }



        void Update()
        {
           

            for (int i =0; i < moneyClones.Count(); i++) {
               moneyClones[i].transform.Rotate( Vector3.forward,5f);
            }

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
                        }else {
                            showInstructions(instructionlists.preBaseline, "preBaseline");
                        }
                    }
                }
                else if (expSceneCon.sceneFsm.State == ExperimentSceneController.SceneStates.ExperimentScene)
                {
                    // aka experiment scene is loaded
                    if (instructionsText && MobiSACore.generatedTrials != null)
                        showInstructions(instructionlists.onExperiment, "onExperiment");
                
                        instructionsText.text += "\n" + MobiSACore.generatedTrials.Count;
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

        void showInstructions(List<Instruction> instructionList, string name)
        {
            if (instructionList != null)
            {
                if (instructionList.Count() == 0)
                {

                    instructionsText.color = Color.red;
                    instructionsText.fontSize = 100;
                    instructionsText.text = name + ": No instruction(s) available";
                    Debug.Log("No instructions list");
                }
                else
                {
                    //TODO: enable multipart instructions, scrollable
                   // foreach (Instruction inst in instructionList)
                   // {
                    Instruction inst = instructionList.First();
                    instructionsText.text = inst.instruction;
                    instructionsText.color = inst.color;
                    instructionsText.fontSize = inst.fontsize;
                    //Debug.Log("First instruction of " + name + " is " +inst.instruction); 
                    StartCoroutine(WaitForAnyKey());
                   // }

                }
            }
        }

        IEnumerator WaitForAnyKey()
        {
           
            if (expSceneCon.controllerInput != null)
            {
                var touchbutton = expSceneCon.controllerInput.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
                    while (!touchbutton)
                    {
                        yield return null;
                    }
            }
        }

       void showMoneyAfterScoreUpdate()
        {
            Image scoreProgress;
            int mod =  score % 10;

            scoreProgressValue = ((float) mod) /10f;
            scoreProgress = GetComponentInChildren<Image>();
            scoreProgress.fillAmount = scoreProgressValue;
            Debug.Log("ScoreProgress: " + scoreProgressValue);
            var quotient = score / 10;
            if (moneyClones.Count() < quotient)
            {
                //instatiate
                var moneyClone = Instantiate(money, money.transform.position + new Vector3(0f, 0f, (moneyClones.Count() + 1) * -1f), Quaternion.AngleAxis(90, Vector3.right));
                Debug.Log("Instantiated money: "+" " + transform.position);
                moneyClones.Add(moneyClone);
                                
            }
            else if (moneyClones.Count() > quotient) {
                //delete
                var toDelete=moneyClones.Last();
                Debug.Log("Deleted money: " + " " + transform.position);
                Destroy(toDelete);
                moneyClones.Remove(toDelete);

            }
            money.SetActive(true);

          
        }

        void updateFruitCollision(object sender) {
            // if (scoresText && NinjaGame.generatedTrials != null)
                 //scoresText.text = "Counting Trials :\n" + NinjaGame.generatedTrials.Count +"Score:"+ eve.score;
         }

       /*  void updateBombCollision(object sender, NinjaGameEventArgs eve)
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