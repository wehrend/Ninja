using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR;
using Assets.LSL4Unity.Scripts;
using UnityEngine.Assertions;

namespace Assets.NinjaGame.Scripts
{
    //Big Todo: Make scenes single, not additive !
    //But this involves some interscene communication probs.
    //TODO: Make static / singletone 
    public class ExperimentSceneController : MonoBehaviour
    {
        public double rbStreamDataRate = 90.00;
        public string preExperimentScene = "Empty_room";
        public string experimentScene = "experimentScene";
        public string postExperimentScene;
        public bool showModelInExpScene = false;
        public int waitTimeAfterLastTrialSpawn=7;
        public float userInitTime = 15.00f;
        public float baselineDuration = 180.00f;
        // public const string expMarkerStreamName = "ExperimentMarkerStream";

        //Next todo: using singletones here 
        public static ExperimentInfo experimentInfo;
        public bool preflag, postflag;
        GameObject model;
        RBControllerStream rbControllerStream;
        RBHmdStream rbHmdStream;
        ScoreAndStats texts;
        public static LSLMarkerStream experimentMarker;
        private int deviceIndex;
        private bool triggerPressed;
        private bool initflag, endflag;
        public bool recordingflag, finishedflag;
        // Use this for initialization
        void Awake()
        {
          experimentInfo = new ExperimentInfo();
          rbControllerStream= GetComponent<RBControllerStream>(); 
          rbHmdStream = GetComponent<RBHmdStream>();
          experimentMarker = gameObject.GetComponent<LSLMarkerStream>();
        }

        void Start()
        {
  
            preExperimentScene = "Empty_room";
            experimentScene = "experimentScene";
            postExperimentScene = "Empty_room";
            preflag = false;
            postflag = false;
            recordingflag = false;
            finishedflag = false;
            //Assert.IsNotNull(experimentMarker, "You forgot to assign the reference to a marker stream implementation!");

            //is SteamVR working??
            if (SteamVR.instance != null) {
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
                Debug.Log("Start empty Room scene");
                SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Additive);
            } else {
                Debug.LogError("No instance of SteamVR found!");
            }
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




        // Update is called once per frame
        void Update()
        {

            if (SteamVR.instance != null)
            {
                // We need this solution to get rid of the CameraRig in the MainScene

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
                if (preflag == false && triggerPressed)
                {
                    preflag = true;


                    var emptyRoom = SceneManager.GetActiveScene();
                    //SceneManager.UnloadSceneAsync(emptyRoom);
                    //Debug.Log("Unload Scene");

                    //unload model

                    model = GameObject.Find("Model");
                    if(model !=null)
                        Debug.LogWarning("Found model:" + model.name);
                    if (model != null)
                        model.SetActive(showModelInExpScene);
                    Debug.Log("Start Experiment");
                    SceneManager.LoadSceneAsync(experimentScene, LoadSceneMode.Additive);
                    if (experimentMarker != null)
                    {
                        Debug.Log("Should Write Marker: begin_experiment_condition");
                        experimentMarker.Write("begin_experiment_condition");
                    }
                }
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
                else if (NinjaGame.generatedTrials != null){
   
                    if ((NinjaGame.generatedTrials.Count == 0) && (!postflag)){
                        //invoke after some time waiting for last bubbles
                        Debug.Log("invoking");
                        Invoke("EndExperiment", waitTimeAfterLastTrialSpawn);

                    }

                }
            }
            else
            {
                Debug.LogError("No instance of SteamVR found!");
            }
        }



            void EndExperiment()
            {
                Debug.Log("Load post experiment scene");
                if (model != null)
                    model.SetActive(true);
                SceneManager.LoadSceneAsync(postExperimentScene, LoadSceneMode.Single);

                if (experimentMarker != null)
                    Debug.Log("Should Write Marker: end_experiment_condition");
                experimentMarker.Write("end_experiment_condition");
                postflag = true;
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