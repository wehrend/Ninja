using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace Assets.NinjaGame.Scripts
{
    //Big Todo: Make scenes single, not additive !
    //But this involves some interscene communication probs.
    //TODO: Make static / singletone 
    public class ExperimentSceneController : MonoBehaviour
    {        
        public string preExperimentScene = "Empty_room";
        public string experimentScene = "experimentScene";
        public string postExperimentScene;
        //Next todo: using singletones here 
        public static ExperimentInfo experimentInfo;
        bool flag;
        GameObject model;
        // Use this for initialization
        void Awake()
        {
            experimentInfo = new ExperimentInfo();
        }

        void Start()
        {
            preExperimentScene = "Empty_room";
            experimentScene = "experimentScene";
            postExperimentScene = "Empty_room";

            //is SteamVR working??
            if (SteamVR.instance != null)
            {


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
                    Debug.Log("Start Experiment");
                    var emptyRoom = SceneManager.GetActiveScene();
                    //SceneManager.UnloadSceneAsync(emptyRoom);
                    //Debug.Log("Unload Scene");

                    //unload model

                    model = GameObject.Find("Model");
                    Debug.LogWarning("Found model:" + model.name);
                    if (model != null)
                        model.SetActive(false);

                    SceneManager.LoadSceneAsync(experimentScene, LoadSceneMode.Additive);
                }
                if ((SceneManager.GetActiveScene().name == experimentScene) && NinjaGame.generatedTrials.Count == 0)
                {

                    Debug.Log("Load post experiment scene");
                    if (model != null)
                        model.SetActive(true);
                    SceneManager.LoadSceneAsync(experimentScene, LoadSceneMode.Additive);
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