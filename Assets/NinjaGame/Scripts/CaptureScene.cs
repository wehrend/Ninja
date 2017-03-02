
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LSL;
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
        public CaptureMarkerStream captureStream;
        private int framenumber;
        private int encFramenumber;
        private int previousFramenumber = 0;
        private int previousEncFramenumber = 0;
        private ExperimentSceneController expScene;

        public static string videoSavePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString() + Path.DirectorySeparatorChar + "CaptureVideos" + Path.DirectorySeparatorChar;
        // Use this for initialization
        void Start()
        {
            Assert.IsNotNull(captureStream, "You forgot to reference the LSLMarkerStream. please do so.");
            if (captureVideoInstance == null)
            {
                //Debug.Log(cameraGO.ToString());
                captureVideoInstance = cameraGO.transform.FindChild("CaptureCamera").gameObject;
                //Debug.Log(captureVideoInstance.ToString());
            }
            captureVideo = captureVideoInstance.GetComponentInChildren<VRCaptureVideo>();
            //Debug.Log(captureVideo.ToString());
            VRCapture.VRCapture.Instance.vrCaptureVideos = new VRCaptureVideo[] { captureVideo };
            curVideoObj = VRCapture.VRCapture.Instance.vrCaptureVideos[0];
            Assert.IsNotNull(curVideoObj, "curVideoObject is null");
        }

        // Update is called once per frame
        void Update()
        {
            if (expScene.sceneFsm.State == ExperimentSceneController.SceneStates.ExperimentScene)
                StartCapture();
            else if (expScene.sceneFsm.State == ExperimentSceneController.SceneStates.ExperimentScene)
                FinishCapture();

            if (captureStream != null)
            {
                if ((curVideoObj != null) && (capturing))
                {

                    ///Here get the framenumber

                    var framenumber = curVideoObj.capturedFrameCount;
                    var encFramenumber = curVideoObj.encodedFrameCount;
                    if (framenumber != previousFramenumber)
                    {
                        string logFrames = string.Format("Frame# {0}, encFrame# {1}", framenumber, encFramenumber);
                        captureStream.Write(logFrames);
                        if (Debug.isDebugBuild)
                            Debug.Log(logFrames);
                    }

                    previousFramenumber = framenumber;
                    previousEncFramenumber = encFramenumber;

                }
            }
            else
            {
                Debug.LogAssertion("No CaptureStream available!");
            }


        }

        public void StartCapture()
        {
            var videoFilepathLog = "Video File Path:" + VRCaptureConfig.SaveFolder.ToString() + "\n" +
              "\n Camera Cache File Path:" + System.IO.Path.GetFullPath(string.Format(@"{0}", "Cache"));
            Debug.LogWarning(videoFilepathLog);
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

    

    //Using an own LSLMarkerStream here for the Metadata  
    public class CaptureMarkerStream : MonoBehaviour
    {
        private const string unique_source_id = "4314E8F41CCE499EBB7414F170D836C6";

        public string lslStreamName = "CaptureMarkerStream";
        public string lslStreamType = "LSL_Marker_Strings";

        private liblsl.StreamInfo lslStreamInfo;
        private liblsl.StreamOutlet lslOutlet;
        private int lslChannelCount = 1;

        //Assuming that markers are never send in regular intervalls
        private double nominal_srate = liblsl.IRREGULAR_RATE;

        private const liblsl.channel_format_t lslChannelFormat = liblsl.channel_format_t.cf_string;

        private string[] sample;

        void Awake()
        {
            sample = new string[lslChannelCount];

            lslStreamInfo = new liblsl.StreamInfo(
                                        lslStreamName,
                                        lslStreamType,
                                        lslChannelCount,
                                        nominal_srate,
                                        lslChannelFormat,
                                        unique_source_id);

            lslOutlet = new liblsl.StreamOutlet(lslStreamInfo);
        }

        public void Write(string marker)
        {
            sample[0] = marker;
            lslOutlet.push_sample(sample);
        }

/*        public void Write(string marker, double customTimeStamp)
        {
            sample[0] = marker;
            lslOutlet.push_sample(sample, customTimeStamp);
        }

        public void Write(string marker, float customTimeStamp)
        {
            sample[0] = marker;
            lslOutlet.push_sample(sample, customTimeStamp);
        }
*/

    }

}

