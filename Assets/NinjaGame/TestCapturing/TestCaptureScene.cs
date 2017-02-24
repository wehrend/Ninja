
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRCapture;
using VRCapture.Demo;

    public class TestCaptureScene : MonoBehaviour
    {
        public GameObject cameraGO;
        [Tooltip("Capture cameras for video recording")]
        private VRCaptureVideo captureVideo;
        private bool isProcessing;
        public VRCaptureVideo[] vrCaptureVideos;
        public bool capturing;
        public GameObject capture;


        // Use this for initialization
        void Start()
        {
            VRCapture.VRCapture.Instance.RegisterSessionCompleteDelegate(HandleCaptureFinish);
            var debugLog = "Video File Path:" + VRCaptureConfig.SaveFolder.ToString() + "\n" +
                "\n Camera Cache File Path:" + System.IO.Path.GetFullPath(string.Format(@"{0}", "Cache"));
            Debug.LogWarning(debugLog);
            var captureVideoGO = this.transform.FindChild("Camera (eye)").gameObject;
            Debug.Log(captureVideoGO.ToString());
            captureVideo=captureVideoGO.GetComponentInChildren<VRCaptureVideo>();
            Debug.Log(captureVideo.ToString());
            VRCapture.VRCapture.Instance.vrCaptureVideos = new VRCaptureVideo[] { captureVideo };
            StartCapture();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
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
