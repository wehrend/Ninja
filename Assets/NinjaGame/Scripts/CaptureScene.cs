
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        private bool isProcessing;
        public VRCaptureVideo[] vrCaptureVideos;
        public bool doCapture;
        public bool capturing;
        public GameObject capture;
        public LSLMarkerStream framenumber;

      
        // Use this for initialization
        void Start()
        {
            framenumber = new LSLMarkerStream(); 
            VRCapture.VRCapture.Instance.RegisterSessionCompleteDelegate(HandleCaptureFinish);
            var debugLog = "Video File Path:" + VRCaptureConfig.SaveFolder.ToString() + "\n" +
                "\n Camera Cache File Path:" + System.IO.Path.GetFullPath(string.Format(@"{0}", "Cache"));
            Debug.LogWarning(debugLog);
            var captureVideoGO = this.transform.FindChild("Camera (eye)").gameObject;
            Debug.Log(captureVideoGO.ToString());
            captureVideo = captureVideoGO.GetComponentInChildren<VRCaptureVideo>();
            Debug.Log(captureVideo.ToString());
            VRCapture.VRCapture.Instance.vrCaptureVideos = new VRCaptureVideo[] { captureVideo };
        }

        // Update is called once per frame
        void Update()
        {

           /* if (doCapture)
            {
                StartCapture();
                
            }
            else
            { 
                FinishCapture();
            }
            if (framenumber)
            {
                if (capturing)
                {
                    ///Here get the framerate 


                }
            }*/
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
