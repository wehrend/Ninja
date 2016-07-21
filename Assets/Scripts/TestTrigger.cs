using UnityEngine;
using System.Collections;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class TestTrigger : MonoBehaviour {

	// Update is called once per frame
	void FixedUpdate () {
        	var deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        	if (deviceIndex != -1 && SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        		SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(1000);
        
    }
}
