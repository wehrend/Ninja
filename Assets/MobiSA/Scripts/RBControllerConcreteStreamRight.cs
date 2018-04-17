using Assets.MobiSA.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.MobiSA.Scripts
{
    public class RBControllerConcreteStreamRight : RBControllerAbstractStream
    {
        public const string unique_id = "BF53CD1A80B1434BB2BED1AB447722FA";
        public const string streamname = "Rigid_Controller_Right";
        public SteamVR_Controller.Device concretedevice = null;

        public static RBControllerConcreteStreamRight instance;

        public RBControllerConcreteStreamRight(string unique_id, string streamname, SteamVR_Controller.Device concretedevice) : base(unique_id, streamname, concretedevice)
        {
            Debug.LogWarning("[RBEyetracking] Got a Right Vive Controller, can build stream");
        }

        void Awake()
        {
            if (!instance)
            {
                var deviceId = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost, Valve.VR.ETrackedDeviceClass.Controller);
                concretedevice = SteamVR_Controller.Input(deviceId);
                instance = new RBControllerConcreteStreamRight(unique_id, streamname, concretedevice);
                DontDestroyOnLoad(gameObject);
            }
        }

        public static RBControllerConcreteStreamRight Instance
        {

            get
            {
                if (!instance)
                {
                    instance = (RBControllerConcreteStreamRight)FindObjectOfType(typeof(RBControllerConcreteStreamRight));
                    if (!instance)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = "RBControllerStreamLeft";
                        instance = gameObject.AddComponent(typeof(RBControllerConcreteStreamRight)) as RBControllerConcreteStreamRight;

                    }
                }
                return instance;
            }
        }
    }
}