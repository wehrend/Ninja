using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCameraTransform : MonoBehaviour {
    [Tooltip("Set the GameObject of the Camera, which has to be duplicated for the streaming")]
    public GameObject camera;


	// Update is called once per frame
	void Update () {
        this.transform.position = camera.transform.position;
        this.transform.rotation = camera.transform.rotation;
        
	}
}
