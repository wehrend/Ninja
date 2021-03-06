﻿// Derivarted work ----------------------------------------------------------------------
//
// Portions (c) Copyright 1997-2013, SensoMotoric Instruments GmbH
// Portions (c) Copyright 2013, SCCN, UCSD
// 

using UnityEngine;
using UnityEngine.SceneManagement;
using LSL;
using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Common;
using Valve.VR;
using SMI;
using System;
using System.Linq;
/// <summary>
/// Code from LSL4Unity Demo Script 
/// </summary>


namespace Assets.MobiSA.Scripts
{
    [ExecuteAfter(typeof(SMIEyetracking))]
    public class RBeyetrackingStream : MonoBehaviour
    {
        public const string unique_source_id = "881A35BC64454035B52C9C518C250E69";
        public const string StreamName = "Rigid_Eyetracking";

        public int ChannelCount = 20;
        //smi
        private SMI.SMIGazeController gazeCon;
        private SMI.SMIGazeController.unity_SampleHMD sample;
        //lsl
        private liblsl.StreamOutlet outlet;
        private liblsl.StreamInfo streamInfoGaze;
       

        public static RBeyetrackingStream instance;
        private GameObject lastGameObjectInFocus;

        private liblsl.XMLElement objs, obj;
        private liblsl.XMLElement channels, chan;
        public liblsl.StreamInfo GetStreamInfo()
        {
            return streamInfoGaze;
        }

        public static ExperimentMarker experimentMarker;
        /// <summary>
        /// Use a array to reduce allocation costs
        /// </summary>
        private double[] currentSample;

        public double dataRate;

        public double GetDataRate()
        {
            return dataRate;
        }

        public void SetDataRate(double rate)
        {
            dataRate = rate;
        }


        public bool HasConsumer()
        {
            if (outlet != null)
                return outlet.have_consumers();

            return false;
        }

        public string StreamType = "Gaze";


        public MomentForSampling sampling;

        public Transform sampleSource;


        void Awake()
        {

            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                lastGameObjectInFocus = new GameObject();
                var experimentSceneController = FindObjectsOfType(typeof(ExperimentSceneController)).FirstOrDefault() as ExperimentSceneController;
                experimentMarker = experimentSceneController.GetExperimentMarker();
                var eyetracking = FindObjectOfType(typeof(SMIEyetracking)) as SMIEyetracking;
                if (eyetracking)
                {
                    Debug.LogError("[RBEyetracking] Found SMIEyetracking object");
                    foreach (var item in eyetracking.SMIEyeTracker.transform)
                    {
                        Debug.LogWarning("[RBEyetracking] Found child object:" + item.ToString());
                    }
                }
                    gazeCon = eyetracking.SMIEyeTracker.GetComponentInChildren<SMIGazeController>();
     
                //if (!gazeCon.isActiveAndEnabled)
                //{
                //    Debug.LogError("[RBEyetracking] Gaze controller from SMI Vive not found.Break up. ");
                    //Application.Quit();
                //} else   
                if(gazeCon != null)
                {
                    // initialize the array once
                    currentSample = new double[ChannelCount];

                    //initialize stream only if available
                    Debug.LogWarning("[RBEyetracking] Got SMI GazeController, can build stream");
                    //
                    streamInfoGaze = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount, dataRate, liblsl.channel_format_t.cf_float32, unique_source_id);
                    //setup LSL stream metadata (code from smi-lsl-apps


                    ///For whatever reasons the foollowing doent work -- commented out
                    //streamInfoGaze.desc().append_child("synchronization").append_child_value("can_drop_samples", "false");

                    var setup = streamInfoGaze.desc().append_child("setup");
                    setup.append_child_value("name", StreamName);
                    // channels with position and orientation in quaternions
                    objs = setup.append_child("objects");
                    obj = objs.append_child("object");
                    obj.append_child_value("label", StreamName);
                    obj.append_child_value("id", StreamName);
                    obj.append_child_value("type", "Eyetracking");
                    
                    channels = streamInfoGaze.desc().append_child("channels");
                    channels.append_child("channel").append_child_value("label", "Screen_X_both").append_child_value("eye", "both").append_child_value("type", "ScreenX").append_child_value("unit", "pixels");
                    channels.append_child("channel").append_child_value("label", "Screen_Y_both").append_child_value("eye", "both").append_child_value("type", "ScreenY").append_child_value("unit", "pixels");
                    // per-eye scene image coordinates

                    channels.append_child("channel").append_child_value("label", "Screen_X_left").append_child_value("eye", "left").append_child_value("type", "ScreenX").append_child_value("unit", "pixels");
                    channels.append_child("channel").append_child_value("label", "Screen_Y_left").append_child_value("eye", "left").append_child_value("type", "ScreenY").append_child_value("unit", "pixels");
                    channels.append_child("channel").append_child_value("label", "Screen_X_right").append_child_value("eye", "right").append_child_value("type", "ScreenX").append_child_value("unit", "pixels");
                    channels.append_child("channel").append_child_value("label", "Screen_Y_right").append_child_value("eye", "right").append_child_value("type", "ScreenY").append_child_value("unit", "pixels");
                    // pupil radii
                    channels.append_child("channel").append_child_value("label", "PupilRadius_left").append_child_value("eye", "left").append_child_value("type", "Radius").append_child_value("unit", "millimeters");
                    channels.append_child("channel").append_child_value("label", "PupilRadius_right").append_child_value("eye", "right").append_child_value("type", "Radius").append_child_value("unit", "millimeters");
                    // 3d positions
                    channels.append_child("channel").append_child_value("label", "EyePosition_X_left").append_child_value("eye", "left").append_child_value("type", "PositionX").append_child_value("unit", "millimeters");
                    channels.append_child("channel").append_child_value("label", "EyePosition_Y_left").append_child_value("eye", "left").append_child_value("type", "PositionY").append_child_value("unit", "millimeters");
                    channels.append_child("channel").append_child_value("label", "EyePosition_Z_left").append_child_value("eye", "left").append_child_value("type", "PositionZ").append_child_value("unit", "millimeters");
                    channels.append_child("channel").append_child_value("label", "EyePosition_X_right").append_child_value("eye", "right").append_child_value("type", "PositionX").append_child_value("unit", "millimeters");
                    channels.append_child("channel").append_child_value("label", "EyePosition_Y_right").append_child_value("eye", "right").append_child_value("type", "PositionY").append_child_value("unit", "millimeters");
                    channels.append_child("channel").append_child_value("label", "EyePosition_Z_right").append_child_value("eye", "right").append_child_value("type", "PositionZ").append_child_value("unit", "millimeters");
                    // 3d directions
                    channels.append_child("channel").append_child_value("label", "EyeDirection_X_left").append_child_value("eye", "left").append_child_value("type", "DirectionX").append_child_value("unit", "normalized");
                    channels.append_child("channel").append_child_value("label", "EyeDirection_Y_left").append_child_value("eye", "left").append_child_value("type", "DirectionY").append_child_value("unit", "normalized");
                    channels.append_child("channel").append_child_value("label", "EyeDirection_Z_left").append_child_value("eye", "left").append_child_value("type", "DirectionZ").append_child_value("unit", "normalized");
                    channels.append_child("channel").append_child_value("label", "EyeDirection_X_right").append_child_value("eye", "right").append_child_value("type", "DirectionX").append_child_value("unit", "normalized");
                    channels.append_child("channel").append_child_value("label", "EyeDirection_Y_right").append_child_value("eye", "right").append_child_value("type", "DirectionY").append_child_value("unit", "normalized");
                    channels.append_child("channel").append_child_value("label", "EyeDirection_Z_right").append_child_value("eye", "right").append_child_value("type", "DirectionZ").append_child_value("unit", "normalized");

                    ///following not available, N/A

                    // confidence values
                    //channels.append_child("channel").append_child_value("label", "PupilConfidence_left").append_child_value("eye", "left").append_child_value("type", "Confidence").append_child_value("unit", "normalized");
                    //channels.append_child("channel").append_child_value("label", "PupilConfidence_right").append_child_value("eye", "right").append_child_value("type", "Confidence").append_child_value("unit", "normalized");     
                    // uncertainties
                    // channels.append_child("channel").append_child_value("label", "EyeballUncertainty_left").append_child_value("eye", "left").append_child_value("type", "Uncertainty").append_child_value("unit", "custom")
                    //     .append_child_value("description", "Measure of uncertainty of eyeball estimator. Lower is better. For ETG: -1.5 is very good, 1.0 is mediore, 4.0 is bad.");
                    //  channels.append_child("channel").append_child_value("label", "EyeballUncertainty_right").append_child_value("eye", "right").append_child_value("type", "Uncertainty").append_child_value("unit", "custom")
                    //     .append_child_value("description", "Measure of uncertainty of eyeball estimator. Lower is better. For ETG: -1.5 is very good, 1.0 is mediore, 4.0 is bad.");
                    // frame numbers
                    //channels.append_child("channel").append_child_value("label", "SceneFrameNumber").append_child_value("eye", "both").append_child_value("type", "FrameNumber").append_child_value("unit", "integer");
                    //channels.append_child("channel").append_child_value("label", "EyeFrameNumber").append_child_value("eye", "both").append_child_value("type", "FrameNumber").append_child_value("unit", "integer");

                    // misc information
                    streamInfoGaze.desc().append_child("acquisition")
                        .append_child_value("manufacturer", "SMI");
                    // instantiate gaze data outlet
                    outlet = new liblsl.StreamOutlet(streamInfoGaze);
                }
            }
        }

        private void pushSample()
        {
            if (outlet == null)
                return;
            // reuse the array for each sample to reduce allocation costs
            // currently only for right-hand device

            if (gazeCon != null)
            {
                sample = gazeCon.smi_getSample();
                // assemble gaze sample
                //if (Time.time % 10==0)
                //{
                //    Debug.Log("Left POR: " + sample.left.por.x+","+ sample.right.por.y + "Right POR: "+sample.right.por.x +","+ sample.right.por.y);
                //}
                GameObject gameObjectInFocus = gazeCon.smi_getGameObjectInFocus();
                if (gameObjectInFocus != null && gameObjectInFocus != lastGameObjectInFocus )
                {
                    experimentMarker.NewGazedObject(gameObjectInFocus);
                }
                currentSample[0] = sample.por.x;
                currentSample[1] = sample.por.y;
                currentSample[2] = sample.left.por.x;
                currentSample[3] = sample.left.por.y;
                currentSample[4] = sample.right.por.x;
                currentSample[5] = sample.right.por.y;
                currentSample[6] = sample.left.pupilRadius;
                currentSample[7] = sample.right.pupilRadius;
                currentSample[8] = sample.left.gazeBasePoint.x;
                currentSample[9] = sample.left.gazeBasePoint.y;
                currentSample[10] = sample.left.gazeBasePoint.z;
                currentSample[11] = sample.right.gazeBasePoint.x;
                currentSample[12] = sample.right.gazeBasePoint.y;
                currentSample[13] = sample.right.gazeBasePoint.z;
                currentSample[14] = sample.left.gazeDirection.x;
                currentSample[15] = sample.left.gazeDirection.y;
                currentSample[16] = sample.left.gazeDirection.z;
                currentSample[17] = sample.right.gazeDirection.x;
                currentSample[18] = sample.right.gazeDirection.y;
                currentSample[19] = sample.right.gazeDirection.z;
                /* double now = liblsl.local_clock();
                 if (now < offset_valid_until)
                 {
                     // update sample age estimate
                     double sample_time = ((double)sample.timestamp) / 1000000000.0 + time_offset;
                     double age = (now - sample_time);
                     sample_age_accum += age;
                     sample_age_count += 1;
                     sample_age = sample_age_accum / sample_age_count;
                 }*/


                outlet.push_sample(currentSample, liblsl.local_clock());
                lastGameObjectInFocus = gameObjectInFocus;
            }

        }

        void FixedUpdate()
        {
            if (sampling == MomentForSampling.FixedUpdate)
                pushSample();
        }

        void Update()
        {
            if (sampling == MomentForSampling.Update)
                pushSample();
        }

        void LateUpdate()
        {
            if (sampling == MomentForSampling.LateUpdate)
                pushSample();

                
        }

     

        public static RBeyetrackingStream Instance
        {

            get {
                if (!instance)
                {
                    instance = (RBeyetrackingStream)FindObjectOfType(typeof(RBeyetrackingStream));
                    if (!instance)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = "RBeyetrackingStream";
                        instance = gameObject.AddComponent(typeof(RBeyetrackingStream)) as RBeyetrackingStream;

                    }
                }
                return instance;
            }
        }

    }
}
