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
        GameObject model;
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
        private bool calibrationflag;
        public bool recordingflag, finishedflag;
        private SMICalibrationVisualizer.VisualisationState previousState, currentState;


        // Use this for initialization
        void Awake()
        {
            initialized = false;
            experimentInfo = new ExperimentInfo();
            rbControllerStream = GetComponent<RBControllerStream>();
            rbHmdStream = GetComponent<RBHmdStream>();
            experimentMarker = gameObject.GetComponent<LSLMarkerStream>();
            sceneFsm = StateMachine<SceneStates>.Initialize(this, SceneStates.None);
            if (sceneFsm!=null)
                Debug.Log("Scene FSM found");

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
                SceneManager.LoadSceneAsync(calibrationScene, LoadSceneMode.Additive);
                Debug.Log("Start Calibration scene");

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
                currentState = SMICalibrationVisualizer.stateOfTheCalibrationView;

                //if calibration state transition
                if (currentState != previousState)
                {
                    Debug.Log("ChangeState...");
                    if (previousState == SMICalibrationVisualizer.VisualisationState.calibration && currentState== SMICalibrationVisualizer.VisualisationState.None) {
                        Debug.Log("from calibrate to none");

                        //wait some time...
                        if ((sceneFsm.State == SceneStates.CalibrateScene) && (Time.time > 3))
                        {

                            sceneFsm.ChangeState(SceneStates.PreScene);
                            // Debug.Log("Unload calibrationScene");

                            //SceneManager.UnloadSceneAsync(calibrationScene);

                            // Debug.Log("Load preExperimentScene");
                            // SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Additive);

                        }
                         }
                    }
                    else
                    {
                       // Debug.Log("Current VisualisationState is" + SMICalibrationVisualizer.stateOfTheCalibrationView);
                   
                    }
                
             
                timeInCalibrationScene = Time.time;

                userInitTime = userInitTime + timeInCalibrationScene;
                previousState = SMICalibrationVisualizer.stateOfTheCalibrationView;
                
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
            SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Additive);
            timeOfEnterRoomScene = Time.time;
        }


        void PreScene_Update()
        {
            if (SteamVR.instance != null)
            {
                if (Time.time - timeOfEnterRoomScene > userInitTime)
                {
                    //Debug.Log(Time.time);
                    InitBaseline();

                    if (((Time.time - timeOfEnterRoomScene) - userInitTime) > baselineDuration)

                        EndBaseline();

                }



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
                    //unload model

                    model = GameObject.Find("Model");
                    if (model != null)
                        Debug.LogWarning("Found model:" + model.name);
                    if (model != null)
                        model.SetActive(showModelInExpScene);

                    Debug.Log("Start Experiment");
                    sceneFsm.ChangeState(SceneStates.ExperimentScene);
                }
            }
        }


        void ExperimentScene_Enter()
        {
            Debug.Log("Load ExperimentScene");
            SceneManager.LoadSceneAsync(experimentScene, LoadSceneMode.Additive);
        }


        void ExperimentScene_Update()
        {
            if (NinjaGame.generatedTrials.Count == 0)
            {
                sceneFsm.ChangeState(SceneStates.PostScene);
            }
        }


        IEnumerator PostScene_Enter() {
            yield return new WaitForSeconds(waitTimeAfterLastTrialSpawn);
        }



        void PostScene_Update()
        {
            Debug.Log("Load post experiment scene");
            if (model != null)
                model.SetActive(true);
            SceneManager.LoadSceneAsync(postExperimentScene, LoadSceneMode.Single);

            if (experimentMarker != null)
                Debug.Log("Should Write Marker: end_experiment_condition");
            experimentMarker.Write("end_experiment_condition");
            //postflag = true;
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