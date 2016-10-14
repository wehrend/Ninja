﻿using UnityEngine;
using LSL;
using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Common;
/// <summary>
/// Code from LSL4Unity Demo Script 
/// </summary>

namespace Assets.NinjaGame.Scripts
{
    public class LSLInterface : MonoBehaviour {
        public const string unique_source_id = "21888D87A8084180A5D3282B077DC881";


        SteamVR_Controller.Device firstDevice;

        int firstControllerIndex;

        private liblsl.StreamOutlet outlet;
        private liblsl.StreamInfo streamInfo;
        public liblsl.StreamInfo GetStreamInfo()
        {
            return streamInfo; 
        }

        /// <summary>
        /// Use a array to reduce allocation costs
        /// </summary>
        private float[] currentSample;

        public double dataRate;

        public double GetDataRate()
        {
            return dataRate;
        }

        public bool HasConsumer()
        {
            if (outlet != null)
                return outlet.have_consumers();

            return false;
        }

        public string StreamName = "SituationalAwareness.Unity.ViveData";
        public string StreamType = "rigidBody";
        // we use 7 DoF:
        // 3 Pos. (x,y,z) + 4 Rot (x,y,z,w)   
        public int ChannelCount = 7; 

        public MomentForSampling sampling;

        public Transform sampleSource;

        void Start()
        {
            SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.First, Valve.VR.ETrackedDeviceClass.Controller);
            firstDevice = SteamVR_Controller.Input( firstControllerIndex);

            // initialize the array once
            currentSample = new float[ChannelCount];

            dataRate = LSLUtils.GetSamplingRateFor(sampling);

            streamInfo = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount, dataRate, liblsl.channel_format_t.cf_float32, unique_source_id);

            outlet = new liblsl.StreamOutlet(streamInfo);
        }

        private void pushSample()
        {
            if (outlet == null)
                return;
            if (Vector3.Magnitude(firstDevice.velocity) > 1)
                Debug.Log("Position:" +firstDevice.transform.pos);
            if (Vector3.Magnitude(firstDevice.angularVelocity) > 1)
                Debug.Log("Rotation"+firstDevice.transform.rot);
            // reuse the array for each sample to reduce allocation costs
            // currently only for right-hand device
            currentSample[0] = firstDevice.transform.pos.x;
            currentSample[1] = firstDevice.transform.pos.y;
            currentSample[2] = firstDevice.transform.pos.z;
            currentSample[2] = firstDevice.transform.rot.x;
            currentSample[4] = firstDevice.transform.rot.y;
            currentSample[5] = firstDevice.transform.rot.z;
            currentSample[6] = firstDevice.transform.rot.w;

            outlet.push_sample(currentSample, liblsl.local_clock());
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
    }
}
