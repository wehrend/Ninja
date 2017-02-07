using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DestroyOnFinish : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(gameObject.GetComponentInChildren<Text>().color.a <= 0.01)
        {
            Destroy(gameObject);
        }
	}
}
