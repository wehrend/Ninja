
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LSL;
using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Common;
using VRCapture;


namespace Assets.NinjaGame.Scripts
{
    public class CaptureScene : MonoBehaviour
    {


        /// <summary>
        /// capture
        /// </summary>
        public GameObject cameraGO;
        [Tooltip("Capture cameras for video recording")]
        private VRCaptureVideo captureVideo;
        public static GameObject captureVideoInstance;
        private bool isProcessing;
        public VRCaptureVideo[] vrCaptureVideos;
        public bool doCapture;
        public bool capturing;
        //public GameObject capture;
        private VRCaptureVideo curVideoObj;
        public LSLMarkerStream CaptureStream;
        private int framenumber;
        private int encFramenumber;
        private int previousFramenumber = 0;
        private int previousEncFramenumber = 0;
        private ExperimentSceneController expScene;

        public static string videoSavePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString()+ Path.DirectorySeparatorChar+ "CaptureVideos"+ Path.DirectorySeparatorChar;
        // Use this for initialization
        void Start()
        {
            Assert.IsNotNull(CaptureStream, "You forgot to reference the LSLMarkerStream. please do so.");
            var debugLog = "Video File Path:" + VRCaptureConfig.SaveFolder.ToString() + "\n" +
                "\n Camera Cache File Path:" + System.IO.Path.GetFullPath(string.Format(@"{0}", "Cache"));
            Debug.LogWarning(debugLog);
            if (captureVideoInstance == null)
            {
                Debug.Log(cameraGO.ToString());
                captureVideoInstance = cameraGO.transform.FindChild("CaptureCamera").gameObject;
                Debug.Log(captureVideoInstance.ToString());
            }
            captureVideo = captureVideoInstance.GetComponentInChildren<VRCaptureVideo>();
            Debug.Log(captureVideo.ToString());
            VRCapture.VRCapture.Instance.vrCaptureVideos = new VRCaptureVideo[] { captureVideo };
            curVideoObj = VRCapture.VRCapture.Instance.vrCaptureVideos[0];
            Assert.IsNotNull(curVideoObj, "curVideoObject is null");
            StartCapture();
        }

        // Update is called once per frame
        void Update()
        {
            if (doCapture)
            {
                if (!capturing)
                {
                    StartCapture();
                }
            }
            else
            {
                if (capturing)
                {
                    FinishCapture();
                }
            }
            if (CaptureStream != null)
            {
                if (capturing && curVideoObj != null)
                {

                    ///Here get the framenumber

                    var framenumber = curVideoObj.capturedFrameCount;
                    var encFramenumber = curVideoObj.encodedFrameCount;
                    if (framenumber != previousFramenumber)
                    {
                        CaptureStream.Write(string.Format("Frame# {0}, encFrame# {1}", framenumber, encFramenumber));
                        Debug.Log(framenumber.ToString());
                        Debug.Log(encFramenumber.ToString());
                    }
                    previousFramenumber = framenumber;
                    previousEncFramenumber = encFramenumber;

                }
            }
            else { Debug.LogAssertion("No CaptureStream available!"); }
            if (Input.GetKey(KeyCode.S))
            {
                StartCapture();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                FinishCapture();
            }
        }

        public void StartCapture()
        {
            Debug.Log("StartCapture()");
            capturing = true;
            VRCapture.VRCapture.Instance.BeginCaptureSession();
        }

        public void FinishCapture()
        {
            Debug.Log("StopCapture");
            capturing = false;
            VRCapture.VRCapture.Instance.EndCaptureSession();

        }

        void HandleCaptureFinish()
        {
            isProcessing = true;
            print("Capture Finish");
        }
    }

}
