
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LSL;
using Assets.MobiSA.Scripts;
using RockVR.Video;


namespace Assets.MobiSA.Scripts
{
    public class CaptureScene : MonoBehaviour
    {


        /// <summary>
        /// capture
        /// </summary>
        [Tooltip("Capture cameras for video recording")]
        private VideoCapture captureVideo;
        public static GameObject captureVideoInstance;
        private bool isProcessing;
        public VideoCapture[] vrCaptureVideos;
        public bool doCapture;
        public bool capturing;
        //public GameObject capture;
        private VideoCapture curVideoObj;
        private VideoCaptureCtrl captureCtrl;
        private CaptureMarkerStream captureStream;
        private int framenumber;
        private int encFramenumber;
        private int previousFramenumber = 0;
        private int previousEncFramenumber = 0;
        private ExperimentSceneController expScene;
        // Use this for initialization
        void Start()
        {

            var expGO = GameObject.Find("[ExperimentSceneController]");
            if (expGO)
            {
                expScene = expGO.GetComponent<ExperimentSceneController>();
            }
            if (!expScene)
                Debug.LogAssertion("No experimentScene Controller available!");
            captureStream = GetComponent<CaptureMarkerStream>();
            Assert.IsNotNull(captureStream, "You forgot to reference the LSLMarkerStream. please do so.");
            if (captureVideoInstance == null)
            {
                //Debug.Log(cameraGO.ToString());
                captureVideoInstance = this.gameObject;//.transform.FindChild("CaptureCamera").gameObject;
                //Debug.Log(captureVideoInstance.ToString());
            }
            captureVideo = captureVideoInstance.GetComponentInChildren<VideoCapture>();
            captureCtrl = captureVideoInstance.GetComponentInChildren<VideoCaptureCtrl>();
            //Debug.Log(captureVideo.ToString());
            VideoCapture[] video = new VideoCapture[] { captureVideo };
            curVideoObj = video[0];
            Assert.IsNotNull(curVideoObj, "curVideoObject is null");



        }

        // Update is called once per frame
        void Update()
        {
            if ((expScene) && (captureStream != null))
            {
              /*  if ((expScene.sceneFsm.State == ExperimentSceneController.SceneStates.ExperimentScene) && (!capturing))
                    StartCapture();
                if ((expScene.sceneFsm.State == ExperimentSceneController.SceneStates.ExperimentScene) && (capturing))
                    FinishCapture();
                    */
                if ((curVideoObj != null) && (capturing))
                {

                    ///Here we get the framenumber

                    var framenumber = curVideoObj.capturedFrameCount;
                    var encFramenumber = curVideoObj.encodedFrameCount;
                    if (framenumber != previousFramenumber)
                    {
                        string logFrames = string.Format("Frame# {0}, encFrame# {1}", framenumber, encFramenumber);
                        captureStream.Write(logFrames);
                        //if (Debug.isDebugBuild)
                        //    Debug.Log(logFrames);
                    }

                    previousFramenumber = framenumber;
                    previousEncFramenumber = encFramenumber;

                }
            }
        }

        public void StartCapture()
        {
            var videoFilepathLog = "Video File Path:" + PathConfig.saveFolder.ToString() + "\n" +
              "\n Camera Cache File Path:" + System.IO.Path.GetFullPath(string.Format(@"{0}", "Cache"));
            Debug.LogWarning(videoFilepathLog);
            Debug.Log("StartCapture()");
            capturing = true;
            captureStream.Write("Video file path"+PathConfig.saveFolder.ToString());
            captureCtrl.StartCapture();
        }

        public void FinishCapture()
        {
            Debug.Log("StopCapture");
            capturing = false;
            captureCtrl.StopCapture();

        }

        void HandleCaptureFinish()
        {
            isProcessing = true;
            print("Capture Finish");
        }


    }
}

