using UnityEngine;
using System.Collections;
using Valve.VR;

public class WandController : MonoBehaviour {

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int) trackedObject.index); } }

    private SteamVR_TrackedObject trackedObject;

	// Use this for initialization
	void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (controller == null)
        {
            Debug.Log("Vive Controller not initialized");
            return;
        }
    }
        
}
