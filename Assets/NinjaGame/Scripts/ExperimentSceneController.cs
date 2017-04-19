using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MonsterLove.StateMachine;
using Valve.VR;
using Assets.LSL4Unity.Scripts;
using UnityEngine.Assertions;
using SMI;
using VRCapture;

namespace Assets.NinjaGame.Scripts
{
    //TODO: Make static / singletone 

    public class ExperimentSceneController : MonoBehaviour
    {
        public enum SceneStates
        {
            //None,
            MenuScene,
            CalibrateScene,
            PreScene,
            ExperimentScene,
            WaitForPostScene,
            PostScene//,
           // Pause
        }
        //[Tooltip("DebugHelper SMI")]
        //public bool noSMI;

        /// <summary>
        /// Caches for the Camera Prefab  
        /// </summary>
        public GameObject player;
        public GameObject vrCapture;
        private GameObject cameraRig;
        public GameObject controllerOne;
        public GameObject controllerTwo;
        private GameObject model, hand;
        ////
        public double rbStreamDataRate = 90.00;
        public string calibrationScene;
        public string preExperimentScene;
        public string experimentScene;
        public string postExperimentScene;
        public int waitTimeAfterLastTrialSpawn = 7;
        public float userInitTime = 15.00f;
        public float calibrationTimeSlot = 30.0f;
        public float baselineDuration = 180.00f;
        private float timeOfEnterRoomScene;
        private float timeInCalibrationScene;
        public float startCalibrationTime;
        // public const string expMarkerStreamName = "ExperimentMarkerStream";
        public StateMachine<SceneStates> sceneFsm;
        //Next todo: using singletones here 
        public static ExperimentInfo experimentInfo;
        [HideInInspector]
        public bool preflag, postflag;
        RBControllerStream rbControllerStream;
        RBHmdStream rbHmdStream;
        ScoreAndStats texts;
        public GameObject pauseScreen; 

        public static LSLMarkerStream experimentMarker;
        //private bool initialized;
        public bool startGame=false;
        private int deviceIndex;
        private bool triggerPressed;
        [HideInInspector]
        public bool initflag,endflag;
        [HideInInspector]
        private bool calibrationflag;
        private bool capturing;
        [HideInInspector]
        public bool recordingflag, finishedflag, waitflag;
       // private SMICalibrationVisualizer calibViz;
        private SMICalibrationVisualizer.VisualisationState previousState, currentState;

        //capturing Scene
        private GameObject camCap;
        private static CaptureScene capScene;
        private GameObject gazeCursor;


        // Use this for initialization
        void Awake()
        {

            /// <summary>
            /// Caches the Camer Prefabs
            /// </summary>
            /// 
            //player = GameObject.Find("Player (with Capturing)").gameObject; //ViveCamera_WithEyetracking

            // now we have the root object, on  which DontDestroyOnLoad() works
            Debug.Log(player.name);
            //not the very best practice, but for the moment
            var cameraRig=player.transform.FindChild("SteamVRObjects");
            controllerOne = cameraRig.transform.Find("Hand1").gameObject;
            controllerTwo = cameraRig.transform.Find("Hand2").gameObject;
            vrCapture = GameObject.Find("VRCapture").gameObject;

            ////
            experimentInfo = new ExperimentInfo();
            rbControllerStream = GetComponent<RBControllerStream>();
            rbHmdStream = GetComponent<RBHmdStream>();
            experimentMarker = gameObject.GetComponent<LSLMarkerStream>();
            sceneFsm = StateMachine<SceneStates>.Initialize(this, SceneStates.MenuScene);
            if (sceneFsm!=null)
                Debug.Log("Scene FSM found");
            // calibViz = GameObject.FindObjectOfType(typeof(SMICalibrationVisualizer)) as SMICalibrationVisualizer;
            // Debug.Log("Found: "+calibViz.ToString());

            //experiment scene controller
            DontDestroyOnLoad(this.gameObject);
            //Whole CameraRig
            DontDestroyOnLoad(player.gameObject);
            DontDestroyOnLoad(vrCapture);
    }



        void MenuScene_Enter()
        {

            Debug.Log("MenuScene enter");
            calibrationScene = "BoxRoom";
            preExperimentScene = "Empty_room";
            experimentScene = "experimentScene";
            postExperimentScene = "Empty_room";
            Debug.Log("InitScene");
            preflag = false;
            postflag = false;
            calibrationflag = false;
            recordingflag = false;
            finishedflag = false;
            waitflag = true;
            //Assert.IsNotNull(experimentMarker, "You forgot to assign the reference to a marker stream implementation!");



            if (rbControllerStream != null)
            {
                rbControllerStream.SetDataRate(rbStreamDataRate);
                if (rbControllerStream.GetDataRate() == rbStreamDataRate)
                    Debug.Log("Set LSL data rate for RB Controller set to " + rbStreamDataRate + "Hz.");
            }
            if (rbHmdStream != null)
            {
                rbHmdStream.SetDataRate(rbStreamDataRate);
                if (rbHmdStream.GetDataRate() == rbStreamDataRate)
                    Debug.Log("Set LSL data rate for RB Hmd set to " + rbStreamDataRate + "Hz.");
            }
            if (SteamVR.instance != null)
            {
                //Get Controller index

                deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                if (deviceIndex == -1)
                    Debug.LogError("Please switch controller on!");


            }
            else
            {
                Debug.LogError("No instance of SteamVR found!");

            }
        }

        void MenuScene_Update()
        {
            if (SteamVR.instance != null)
            {

                if (startGame)
                {
                    Debug.Log("Load calibrate scene");
                    sceneFsm.ChangeState(SceneStates.CalibrateScene, StateTransition.Overwrite);
                }
            }
          //  else
          //  {
          //      Debug.LogError("No instance of SteamVR found!");
          //  }

        }


        void CalibrateScene_Enter()
        {
            SceneManager.LoadSceneAsync(calibrationScene, LoadSceneMode.Single);
            Debug.Log("Start Calibration scene");

        }


        ///search for the loading screen to deactivate if not in calib mode
        void  DisableSMIScreen()
        {
            var loadScreen = GameObject.Find("SMILoadingScreen");

            if (loadScreen)
                loadScreen.SetActive(false);
        }

        void InitBaseline()
        {

            if (!initflag)
            {

                Debug.Log("Init baseline...");
                recordingflag = true;

                if (experimentMarker != null)
                {
                    experimentMarker.Write("begin_baseline_condition");
                    Debug.Log("begin_baseline_condition");
                }
                initflag = true;
            }

        }

        void EndBaseline()
        {

            if (!endflag)
            {

                Debug.Log("End baseline...");
                if (experimentMarker != null)
                {
                    experimentMarker.Write("end_baseline_condition");
                    Debug.Log("end_baseline_condition");
                }

                endflag = true;
                recordingflag = false;
            }
        }

        void CalibrateScene_Update()
        {
            if (SteamVR.instance != null)
            {

                // if ((Input.GetKeyDown(KeyCode.Alpha3)) || (Input.GetKeyDown(KeyCode.Alpha5)))
                // {
                //     startCalibrationTime = Time.time;
                //     Debug.Log("Accept key at time: " + startCalibrationTime);
                // }


                //wait some time...
                if ((sceneFsm.State == SceneStates.CalibrateScene))
                    {
                        if (SMIGazeController.GazeModel.connectionRoutineDone)
                        {
                            Debug.Log("SMI ConnectionRoutine done.");
                            if (SMIGazeController.GazeModel.ErrorID == 1) //no error 
                            {
                                Debug.Log("SMI Vive");
                            //Give timeslot for calibration

                                if (Time.time > calibrationTimeSlot)
                                {
                                    LoadPreScene();
                                }
                              //  else {
                              //      Debug.Log(Time.time + "<" + calibrationTimeSlot);
                              //  }
                            }
                            else if (SMIGazeController.GazeModel.ErrorID == 506)
                            {
                                Debug.LogError("No SMI Vive connected! (ErrorID: 506)");
                                //Direct Scene Load 
                                LoadPreScene();
                            }
                            else {
                                Debug.LogError("SMI Vive Error " + SMIGazeController.GazeModel.ErrorID);
                                //Direct Scene Load 
                                LoadPreScene();
                            }

                        }

                    }
             }
        }


        void LoadPreScene()
        {
            gazeCursor = GameObject.Find("Example_GazeCursor");

            //GazeCursor
            if (gazeCursor != null)
                DontDestroyOnLoad(gazeCursor.gameObject);
            SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Single);
            Debug.Log("Load Scene preExperimentScene");
            sceneFsm.ChangeState(SceneStates.PreScene);
            //SceneManager.UnloadSceneAsync(calibrationScene);
        }



        void PreScene_Enter()
        {

            if (sceneFsm.LastState == SceneStates.CalibrateScene)
            {
                Debug.Log("Unload calibrationScene");
                SceneManager.UnloadSceneAsync(calibrationScene);
            }
            //Debug.Log("Load preExperimentScene");
            //SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Single);
            DisableSMIScreen();
            timeOfEnterRoomScene = Time.time;
            //CheckDeactivates();

        }


        void PreScene_Update()
        {
           
                if (Time.time - timeOfEnterRoomScene > userInitTime)
                {
                    //Debug.Log(Time.time);
                    InitBaseline();

                    if (((Time.time - timeOfEnterRoomScene) - userInitTime) > baselineDuration)

                        EndBaseline();

                }
                


            if (SteamVR.instance != null)
            {
                //Debug.Log("deviceIndex: " + deviceIndex);
                if (deviceIndex == -1)
                {
                    //check again
                    deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                }
                else
                {
                    triggerPressed = SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
                }
                //Debug.Log("trigger status: " + triggerPressed);
                if (triggerPressed)
                {
                    Debug.Log("Start Experiment");
                    sceneFsm.ChangeState(SceneStates.ExperimentScene);
                }
            }
        }


        void ExperimentScene_Enter()
        {
            //explicit needed(?)
            ExperimentSceneController.experimentInfo.triggerPressed = false;

            Debug.Log("Load ExperimentScene");
            SceneManager.LoadSceneAsync(experimentScene, LoadSceneMode.Single);
            //pauseScreen =GameObject.FindGameObjectWithTag("PauseScreen").gameObject;
            //if (pauseScreen!=null)
            //    pauseScreen.SetActive(false);

            ActivateAndStartCapturing();
            //CheckDeactivates();
            DisableSMIScreen();

            
        }


        void ExperimentScene_Update()
        {
            if ((NinjaGame.generatedTrials != null) && (  NinjaGame.generatedTrials.Count == 0))
            {
                sceneFsm.ChangeState(SceneStates.PostScene);
                if(capScene)
                    capScene.FinishCapture();
            }

            //freeze the game 
            if (Input.GetKey(KeyCode.Space))
            {
                if (pauseScreen != null)
                {
                    if (Time.timeScale == 1.00)
                    {
                        Time.timeScale = 0.0f;
                        experimentMarker.Write("Pause_begin");
                        pauseScreen.SetActive(true);
                    }
                    else
                    {
                        pauseScreen.SetActive(false);
                        experimentMarker.Write("Pause_end");
                        Time.timeScale = 1.0f;
                    }
                }
                

                
            }
        }



        IEnumerator PostScene_Enter() {
            Debug.Log("PostScene_Enter");
            yield return new WaitForSeconds(waitTimeAfterLastTrialSpawn);
            PostScene();
        }


        void PostScene() {
            waitflag = false;
            Debug.Log("Load post experiment scene");
            SceneManager.LoadSceneAsync(postExperimentScene, LoadSceneMode.Single);
            
           // CheckDeactivates();
            if (experimentMarker != null)
                Debug.Log("Should Write Marker: end_experiment_condition");
            experimentMarker.Write("end_experiment_condition");
            Debug.Log("End Application!");
           // Application.Quit();
        }

        /*void CheckDeactivates()
        { 
            if (noSMI)
            {
                var smi = GameObject.Find("SMIEyeTracker");

                var script =smi.GetComponent<SMIEyetracking>();
                if (script)
                {
                    script.enabled = false;
                    smi.SetActive(false);
                    Debug.Log("Deactivate smi: script.isActiveAndEnabled=" + script.isActiveAndEnabled);
                }
                else
                {
                    Debug.Log("No SMIEyetracking script found.");
                }


            }
        }*/



       void ActivateAndStartCapturing()
        {

            
            if (vrCapture)
            {
                vrCapture.SetActive(true);
                Debug.Log("Activate vrCapture" + vrCapture.ToString());
                capScene = vrCapture.GetComponent<CaptureScene>();
                if (capScene)
                {
                    capScene.enabled = true;
                    capScene.StartCapture();
                }
                else
                {
                    Debug.Log("Capture Scene not found!");
                }
            }
            else {
                Debug.Log("VRCamera not found");
            }
        }
        

    }



    [Serializable]
    public class ExperimentInfo : ScriptableObject
    {
        public int score;
        public int totalscore;
        public int damage;
        public int health;
        public bool triggerPressed;
        /* public List<Trial> ListOfTrials;

         public void setListOfTrials(List<Trial> trials)
         {
             game.ListOfTrials = trials;
         }*/
    }

}