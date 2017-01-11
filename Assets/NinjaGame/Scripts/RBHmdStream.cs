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

namespace Assets.NinjaGame.Scripts
{
    public class RBHmdStream : MonoBehaviour
    {
        public const string unique_source_id = "A723E9DC2E5A4EA5959C2772DA1D3DB3";

        SteamVR_Camera cameraInfo;

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

        public string StreamName = "Rigid_HMD";
        public string StreamType = "rigidBody";
        // we use 7 DoF:
        // 3 Pos. (x,y,z) + 4 Rot (x,y,z,w)   
        public int ChannelCount = 7;

        public MomentForSampling sampling;

        public Transform sampleSource;

        void Start()
        {
            cameraInfo = new SteamVR_Camera();

            // initialize the array once
            currentSample = new float[ChannelCount];

            //dataRate = LSLUtils.GetSamplingRateFor(sampling);

            streamInfo = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount, dataRate, liblsl.channel_format_t.cf_float32, unique_source_id);

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
            if (cameraInfo != null)
            {
                currentSample[0] = cameraInfo.transform.position.x;
                currentSample[1] = cameraInfo.transform.position.y;
                currentSample[2] = cameraInfo.transform.position.z;
                currentSample[2] = cameraInfo.transform.rotation.x;
                currentSample[4] = cameraInfo.transform.rotation.y;
                currentSample[5] = cameraInfo.transform.rotation.z;
                currentSample[6] = cameraInfo.transform.rotation.w;

                outlet.push_sample(currentSample, liblsl.local_clock());
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
    }
}
