using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class GrabbingProxy : MonoBehaviour {

    public AObjectGrabbedEvent onGrabbed;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void GenerateGrabEvent(GameObject grabbingObject)
    {
        if(onGrabbed.GetPersistentEventCount() > 0)
        {
            onGrabbed.Invoke(grabbingObject);
        }
    }
}

[Serializable]
public class AObjectGrabbedEvent : UnityEvent<GameObject> { }