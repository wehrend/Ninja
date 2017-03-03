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
    //Big Todo: Make scenes single, not additive !
    //But this involves some interscene communication probs.
    //TODO: Make static / singletone 

    public class ExperimentSceneController : MonoBehaviour
    {
        public enum SceneStates
        {
            None,
            CalibrateScene,
            PreScene,
            ExperimentScene,
            PostScene
        }
        [Tooltip("DebugHelper SMI")]
        public bool noSMI;
        /// <summary>
        /// Caches for the Camera Prefab  
        /// </summary>
        public GameObject cameraRig;
        public GameObject controllerLeft;
        public GameObject controllerRight;
        private GameObject model, hand;
        ////
        public double rbStreamDataRate = 90.00;
        public string calibrationScene = "BoxRoom";
        public string preExperimentScene = "Empty_room";
        public string experimentScene = "experimentScene";
        public string postExperimentScene;
        public bool showModelInExpScene = false;
        public int waitTimeAfterLastTrialSpawn = 7;
        public float userInitTime = 15.00f;
        public float baselineDuration = 180.00f;
        private float timeOfEnterRoomScene;
        // public const string expMarkerStreamName = "ExperimentMarkerStream";
        public StateMachine<SceneStates> sceneFsm;
        //Next todo: using singletones here 
        public static ExperimentInfo experimentInfo;
        public bool preflag, postflag;
        RBControllerStream rbControllerStream;
        RBHmdStream rbHmdStream;
        ScoreAndStats texts; 

        public bool isSMIvive;
        public static LSLMarkerStream experimentMarker;
        private bool initialized;
        private int deviceIndex;
        private bool triggerPressed;
        private bool initflag,endflag;
        private float timeInCalibrationScene;
        public float startCalibrationTime;
        private bool calibrationflag;
        private bool capturing;
        public bool recordingflag, finishedflag;
       // private SMICalibrationVisualizer calibViz;
        private SMICalibrationVisualizer.VisualisationState previousState, currentState;

        //capturing Scene
        private GameObject camCap;
        private static CaptureScene capScene;
        

        // Use this for initialization
        void Awake()
        {
            initialized = false;
            /// <summary>
            /// Caches the Camer Prefabs, we use Child element, cause naming varys
            /// </summary>
            cameraRig = GameObject.Find("Camera (eye)").transform.parent.gameObject;
            controllerLeft = GameObject.Find("Controller (left)");
            controllerRight = GameObject.Find("Controller (right)");
            ////
            experimentInfo = new ExperimentInfo();
            rbControllerStream = GetComponent<RBControllerStream>();
            rbHmdStream = GetComponent<RBHmdStream>();
            experimentMarker = gameObject.GetComponent<LSLMarkerStream>();
            sceneFsm = StateMachine<SceneStates>.Initialize(this, SceneStates.None);
            if (sceneFsm!=null)
                Debug.Log("Scene FSM found");
            // calibViz = GameObject.FindObjectOfType(typeof(SMICalibrationVisualizer)) as SMICalibrationVisualizer;
            // Debug.Log("Found: "+calibViz.ToString());
            
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(cameraRig);

        }



        void OnGUI()
        {
            if (!initialized)
            {
                Debug.Log("InitScene");

                preExperimentScene = "Empty_room";
                experimentScene = "experimentScene";
                postExperimentScene = "Empty_room";
                preflag = false;
                postflag = false;
                calibrationflag = false;
                recordingflag = false;
                finishedflag = false;
                //Assert.IsNotNull(experimentMarker, "You forgot to assign the reference to a marker stream implementation!");

              
                //is SteamVR working??
                if (SteamVR.instance != null)
                {
                    //Get Controller index

                    deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                    if (deviceIndex == -1)
                        Debug.LogError("Please switch controller on!");

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
                    initialized = true;
                    if (isSMIvive)
                    {
                            Debug.Log("Load calibrate scene");
                            sceneFsm.ChangeState(SceneStates.CalibrateScene, StateTransition.Overwrite);
                            
                    }
                    else
                    {
                        Debug.Log("No SMI Vive available, load room scene");
                        sceneFsm.ChangeState(SceneStates.PreScene);
                    }

                }
                else
                {
                    Debug.LogError("No instance of SteamVR found!");
                    initialized = true;

                }
            }
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

                //            Debug.Log(SMICalibrationVisualizer.Instance.ToString());
                
                  /*  if ((Input.GetKeyDown(KeyCode.Alpha3)) || (Input.GetKeyDown(KeyCode.Alpha5)))
                    {
                        startCalibrationTime = Time.time;
                        Debug.Log("Accept key at time: " + startCalibrationTime);
                    }*/
                    
                
                
                    //wait some time...
                    if ((sceneFsm.State == SceneStates.CalibrateScene) && (Time.time - startCalibrationTime > 30))
                    {
                    //the Calibration scene rig translated by 0.704 from origin
                    sceneFsm.ChangeState(SceneStates.PreScene);
                        Debug.Log("Unload calibrationScene");

                        //SceneManager.UnloadSceneAsync(calibrationScene);

                        // Debug.Log("Load preExperimentScene");
                        // SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Additive);

                    }
            }
        }


        void PreScene_Enter()
        {

            if (sceneFsm.LastState == SceneStates.CalibrateScene)
            {
                Debug.Log("Unload calibrationScene");
                SceneManager.UnloadSceneAsync(calibrationScene);
            }
            Debug.Log("Load preExperimentScene");
            SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Single);
            DisableSMIScreen();
            timeOfEnterRoomScene = Time.time;
            CheckDeactivates();

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
            Debug.Log("Load ExperimentScene");
            SceneManager.LoadSceneAsync(experimentScene, LoadSceneMode.Single);
            ///no controllers, but hands 
            ChangeAppeareance(false, true);
            ActivateAndStartCapturing();
            CheckDeactivates();
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
        }


        IEnumerator PostScene_Enter() {
            yield return new WaitForSeconds(waitTimeAfterLastTrialSpawn);
            Debug.Log("Load post experiment scene");
            //hands, no controllers
            ChangeAppeareance(true, false);
            SceneManager.LoadSceneAsync(postExperimentScene, LoadSceneMode.Single);
           
           // CheckDeactivates();
            if (experimentMarker != null)
                Debug.Log("Should Write Marker: end_experiment_condition");
            experimentMarker.Write("end_experiment_condition");
            Debug.Log("End Application!");
            Application.Quit();
        }

        void CheckDeactivates()
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
        }

        void ChangeAppeareance(bool showModel, bool showHands)
        {
            //can be optimized later on....
            var modell = controllerLeft.transform.FindChild("Model").gameObject;
            if (modell != null)
            {
                Debug.LogWarning("Found model:" + modell.name);
                modell.SetActive(showModel);
            }
            var modelr = controllerRight.transform.FindChild("Model").gameObject;
            if (modelr != null)
            {
                Debug.LogWarning("Found model:" + modelr.name);
                modelr.SetActive(showModel);
            }
            var handl = controllerLeft.transform.FindChild("HandCursor_edited").gameObject;
            if (handl != null)
            {
                Debug.LogWarning("Found model:" + handl.name);
                modell.SetActive(showHands);
            }
            var handr = controllerRight.transform.FindChild("HandCursor_edited").gameObject;
            if (handr != null)
            {
                Debug.LogWarning("Found model:" + handr.name);
                handr.SetActive(showHands);
            }

        }



       void ActivateAndStartCapturing()
        {

            camCap = GameObject.Find("CameraCapture");
            if (camCap)
            {
                camCap.SetActive(true);
                Debug.Log("Activate campCap" + camCap.ToString());
                capScene = camCap.GetComponent<CaptureScene>();
                if (capScene)
                {
                    capScene.enabled = true;
                    capScene.StartCapture();
                }

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