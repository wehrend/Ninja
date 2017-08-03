using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InteractableTest : MonoBehaviour {

    private TextMesh debugText;

	// Use this for initialization
	void Start () {
        debugText = GetComponentInChildren<TextMesh>();
        debugText.text = "Hand model attached";
	}
}
