using UnityEngine;
using LSL;
using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Common;
/// <summary>
/// Code from LSL4Unity Demo Script 
/// </summary>


    /*  # channels with position and orientation in quaternions
        objs = setup.append_child("objects")
        obj = objs.append_child("object")
        obj.append_child_value("label", "Rigid" + str(name))
        obj.append_child_value("id", str(name))
        obj.append_child_value("type", "Mocap")

        channels = vizard_rigid.desc().append_child("channels")
        chan = channels.append_child("channel")
        chan.append_child_value("label", "Rigid_" + str(name) + "_X")
        chan.append_child_value("object", "Rigid_" + str(name))
        chan.append_child_value("type", "PositionX")
        chan.append_child_value("unit", "meters")

        chan = channels.append_child("channel")
        chan.append_child_value("label", "Rigid_" + str(name) + "_Y")
        chan.append_child_value("object", "Rigid_" + str(name))
        chan.append_child_value("type", "PositionY")
        chan.append_child_value("unit", "meters")

        chan = channels.append_child("channel")
        chan.append_child_value("label", "Rigid_" + str(name) + "_Z")
        chan.append_child_value("object", "Rigid_" + str(name))
        chan.append_child_value("type", "PositionZ")
        chan.append_child_value("unit", "meters")

        chan = channels.append_child("channel")
        chan.append_child_value("label", "Rigid_" + str(name) + "_quat_X")
        chan.append_child_value("object", "Rigid_" + str(name))
        chan.append_child_value("type", "OrientationX")
        chan.append_child_value("unit", "quaternion")

        chan = channels.append_child("channel")
        chan.append_child_value("label", "Rigid_" + str(name) + "_quat_Y")
        chan.append_child_value("object", "Rigid_" + str(name))
        chan.append_child_value("type", "OrientationY")
        chan.append_child_value("unit", "quaternion")

        chan = channels.append_child("channel")
        chan.append_child_value("label", "Rigid_" + str(name) + "_quat_Z")
        chan.append_child_value("object", "Rigid_" + str(name))
        chan.append_child_value("type", "OrientationZ")
        chan.append_child_value("unit", "quaternion")

        chan = channels.append_child("channel")
        chan.append_child_value("label", "Rigid_" + str(name) + "_quat_W")
        chan.append_child_value("object", "Rigid_" + str(name))
        chan.append_child_value("type", "OrientationW")
        chan.append_child_value("unit", "quaternion")

        if ps_heading:
            chan = channels.append_child("channel")
            chan.append_child_value("label", "Rigid_" + str(name) + "_PS_orientation_yaw")
            chan.append_child_value("object", "Rigid_" + str(name))
            chan.append_child_value("type", "PS_orientation_yaw")
            chan.append_child_value("unit", "euler")
*/

namespace Assets.MobiSA.Scripts
{
    public class RBControllerStream : MonoBehaviour {
        public const string unique_source_id = "21888D87A8084180A5D3282B077DC881";
        public const string StreamName = "Rigid_Controller";
        SteamVR_Controller.Device firstDevice;

        int firstControllerIndex;

        private liblsl.StreamOutlet outlet;
        private liblsl.StreamInfo streamInfo;
        private liblsl.XMLElement objs, obj;
        private liblsl.XMLElement channels, chan;
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

        public void SetDataRate(double rate )
        {
            dataRate=rate;
        }


        public bool HasConsumer()
        {
            if (outlet != null)
                return outlet.have_consumers();

            return false;
        }

        //public string StreamName = "Rigid_Controller";
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

            //dataRate = LSLUtils.GetSamplingRateFor(sampling);

            streamInfo = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount, dataRate, liblsl.channel_format_t.cf_float32, unique_source_id);
            //setup LSL stream metadata (code from vizard) 
            streamInfo.desc().append_child("synchronization").append_child_value("can_drop_samples", "true");
            var setup = streamInfo.desc().append_child("setup");
            setup.append_child_value("name", StreamName);
            // channels with position and orientation in quaternions
            objs = setup.append_child("objects");
            obj = objs.append_child("object");
            obj.append_child_value("label", StreamName);
            obj.append_child_value("id", StreamName);
            obj.append_child_value("type", "Mocap");

            channels = streamInfo.desc().append_child("channels");
            chan = channels.append_child("channel");
            chan.append_child_value("label", StreamName + "_X");
            chan.append_child_value("object", StreamName);
            chan.append_child_value("type", "PositionX");
            chan.append_child_value("unit", "meters");

            chan = channels.append_child("channel");
            chan.append_child_value("label", StreamName + "_Y");
            chan.append_child_value("object", StreamName);
            chan.append_child_value("type", "PositionY");
            chan.append_child_value("unit", "meters");

            chan = channels.append_child("channel");
            chan.append_child_value("label", StreamName + "_Z");
            chan.append_child_value("object", StreamName);
            chan.append_child_value("type", "PositionZ");
            chan.append_child_value("unit", "meters");

            chan = channels.append_child("channel");
            chan.append_child_value("label", StreamName + "_quat_X");
            chan.append_child_value("object", StreamName);
            chan.append_child_value("type", "OrientationX");
            chan.append_child_value("unit", "quaternion");

            chan = channels.append_child("channel");
            chan.append_child_value("label", StreamName + "_quat_Y");
            chan.append_child_value("object", StreamName);
            chan.append_child_value("type", "OrientationY");
            chan.append_child_value("unit", "quaternion");

            chan = channels.append_child("channel");
            chan.append_child_value("label", StreamName + "_quat_Z");
            chan.append_child_value("object", StreamName);
            chan.append_child_value("type", "OrientationZ");
            chan.append_child_value("unit", "quaternion");

            chan = channels.append_child("channel");
            chan.append_child_value("label", StreamName + "_quat_W");
            chan.append_child_value("object", StreamName);
            chan.append_child_value("type", "OrientationW");
            chan.append_child_value("unit", "quaternion");


            outlet = new liblsl.StreamOutlet(streamInfo);
       }

       private void pushSample()
       {
           if (outlet == null)
               return;
          /* if (Vector3.Magnitude(firstDevice.velocity) > 1)
               Debug.Log("Position:" +firstDevice.transform.pos);
           if (Vector3.Magnitude(firstDevice.angularVelocity) > 1)
               Debug.Log("Rotation"+firstDevice.transform.rot);*/
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
