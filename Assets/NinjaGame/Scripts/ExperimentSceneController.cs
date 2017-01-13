using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        //Next todo: using singletones here 
        public static ExperimentInfo experimentInfo;
        bool flag;
        GameObject model;
        RBControllerStream rbControllerStream;
        RBHmdStream rbHmdStream;
        public static LSLMarkerStream experimentMarker;

        
        // Use this for initialization
        void Awake()
        {
          experimentInfo = new ExperimentInfo();
          rbControllerStream= GetComponent<RBControllerStream>();
          rbHmdStream = GetComponent<RBHmdStream>();
          experimentMarker = new LSLMarkerStream();
          if (experimentMarker)
            experimentMarker.lslStreamName = "ExperimentMarkerStream";
        }

        void Start()
        {
            preExperimentScene = "Empty_room";
            experimentScene = "experimentScene";
            postExperimentScene = "Empty_room";

            //Assert.IsNotNull(experimentMarker, "You forgot to assign the reference to a marker stream implementation!");

            //is SteamVR working??
            if (SteamVR.instance != null) {

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
                Debug.Log("Start empty Room scene with baseline");
                SceneManager.LoadSceneAsync(preExperimentScene, LoadSceneMode.Additive);
                flag = false;
            } else {
                Debug.LogError("No instance of SteamVR found!");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (SteamVR.instance != null)
            {
                // We need this solution to get rid of the CameraRig in the MainScene
                int deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                //Debug.Log("deviceIndex: " + deviceIndex);
                bool triggerPressed = SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
                //Debug.Log("trigger status: " + triggerPressed);
                if (flag == false && triggerPressed)
                {
                    flag = true;

                   
                    var emptyRoom = SceneManager.GetActiveScene();
                    //SceneManager.UnloadSceneAsync(emptyRoom);
                    //Debug.Log("Unload Scene");

                    //unload model

                    model = GameObject.Find("Model");
                    Debug.LogWarning("Found model:" + model.name);
                    if (model != null)
                        model.SetActive(showModelInExpScene);
                    Debug.Log("Start Experiment");
                    SceneManager.LoadSceneAsync(experimentScene, LoadSceneMode.Additive);
                    if(experimentMarker!=null)
                        experimentMarker.Write("begin_experiment_condition");
                }
                if ((NinjaGame.generatedTrials!=null) && (NinjaGame.generatedTrials.Count == 0))
                {
                    Debug.Log("Load post experiment scene");
                    if (model != null)
                        model.SetActive(true);
                    SceneManager.LoadSceneAsync(postExperimentScene, LoadSceneMode.Additive);
                    if (experimentMarker != null)
                        experimentMarker.Write("end_experiment_condition");
                }
            }else {
                Debug.LogError("No instance of SteamVR found!");
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