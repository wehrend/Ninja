using UnityEngine;
using System.Collections;

public class animationHands : MonoBehaviour {

    public Animation anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
        foreach (AnimationState state in anim)
        {
            state.speed = 0.5f;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
