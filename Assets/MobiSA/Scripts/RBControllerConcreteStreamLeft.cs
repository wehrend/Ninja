using Assets.MobiSA.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.MobiSA.Scripts
{
    public class RBControllerConcreteStreamLeft : RBControllerAbstractStream
    {
        public const string unique_id = "21888D87A8084180A5D3282B077DC881";
        public const string streamname = "Rigid_Controller_Left";
        public SteamVR_Controller.Device concretedevice = null;

        public static RBControllerConcreteStreamLeft instance;

        public RBControllerConcreteStreamLeft(string unique_id, string streamname, SteamVR_Controller.Device concretedevice) : base(unique_id, streamname, concretedevice)
        {
            Debug.LogWarning("[RBEyetracking] Got a Left Vive Controller, can build stream");
        }

        void Awake()
        {
            if (!instance)
            {
                var deviceId = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost, Valve.VR.ETrackedDeviceClass.Controller);
                concretedevice = SteamVR_Controller.Input(deviceId);
                instance = new RBControllerConcreteStreamLeft(unique_id, streamname,concretedevice);
                DontDestroyOnLoad(gameObject);
            }
        }

        public static RBControllerConcreteStreamLeft Instance
        {

            get
            {
                if (!instance)
                {
                    instance = (RBControllerConcreteStreamLeft)FindObjectOfType(typeof(RBControllerConcreteStreamLeft));
                    if (!instance)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = "RBControllerStreamLeft";
                        instance = gameObject.AddComponent(typeof(RBControllerConcreteStreamLeft)) as RBControllerConcreteStreamLeft;

                    }
                }
                return instance;
            }
        }
    }
}