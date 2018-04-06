using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RockVR.Video;
using LSL;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using System.Linq;

namespace Assets.MobiSA.Scripts
{

    public class InfoUI : MonoBehaviour
    {

        private const string noStreamsFound = "No streams found!";

        private liblsl.ContinuousResolver resolver, specificResolver;
        public liblsl.StreamInfo[] streams;
        public List<liblsl.StreamInlet> inletsList;
        public string StreamType = "Gaze";//"rigidBody";
        public List<string> listOfStreams = new List<string>();
        private string streamResult;

        private void Init()
        {
            resolver = new liblsl.ContinuousResolver();
        }

        // Update is called once per frame
        void OnGUI()
        {
            if (resolver == null)
                Init();
            UpdateStreams();

            GUI.TextField(new Rect(new Vector2(30, 20), new Vector2(300, 20)), "Capture Path:" + PathConfig.saveFolder);
            if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.FINISH)
                GUI.TextField(new Rect(new Vector2(30, 40), new Vector2(500, 20)), string.Format("Video Capture File is {0} written.", StringUtils.GetMp4FileName(CaptureStringUtils.GetBlockName())));

            //stream info
            GUI.TextField(new Rect(new Vector2(30, 60), new Vector2(150, 20)), "Stream Info");
            int y = 80;
            foreach (var item in listOfStreams)
            {
                GUI.TextField(new Rect(new Vector2(30, y), new Vector2(300, 20)), item.ToString());
                y += 20;
            }
        }

        void UpdateStreams()
        {
            listOfStreams.Clear();

            var streamInfos = resolver.results();
            if (streamInfos.Length == 0)
            {
                streamResult = noStreamsFound;
            }
            else
            {    //Add only if it is from our own machine
                var localStreamInfos = streamInfos.Where(r => r.hostname().ToUpperInvariant().Equals(SystemInfo.deviceName));

                foreach (var item in localStreamInfos)
                {
                    listOfStreams.Add(string.Format("{0}; {1}; sRate: {2} Hz", item.name(), item.type(), item.nominal_srate()));

                }

                /*var filteredStreams = localStreamInfos.Where(r => r.type().Equals(StreamType)).ToList();
                Debug.Log(string.Format("Local streams {0},  Filtered Streams {1} ", localStreamInfos.Count(), filteredStreams.Count()));
                foreach (var stream in filteredStreams)
                {
                    var inlet = new liblsl.StreamInlet(stream);
                    Debug.Log(string.Format("{0}:{1}", inlet.info().name(), inlet.samples_available()));
                    //inletsList.Add(inlet);
                }*/
            }
        }
    }
}