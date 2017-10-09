using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.VREF.Scripts;
using Valve.VR;

public class FadeObjectOnCollision : MonoBehaviour {

    float distance;
    float time;
    public float fade=1f;
    bool flag = false;
    float stepwidth;
    public int frameSteps=10;
    public float waitTime;
    public float deltatime;
    public bool forceFeedback;
    //[Tooltip("Total Animation Time (ms)")]
   // public float animationTimeDuration=0.1f;
    private Vector4 objPos;
    MeshRenderer renderer;
    Color rendererColor;
    Color alphaColor;
    Rigidbody body;
    AudioSource audio;
    private Vector3 statictransformScale;

    int deviceID;

    //[RequireComponent(typeof(AudioSource))]
    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();
        deviceID = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        if (deviceID == -1)
            Debug.LogWarning("No Controller");
        Debug.Log("Controller ID:"+deviceID);
        //animation time duration in milliseconds for 100 steps
       // waitTime = animationTimeDuration / (1000f*frameSteps);
        stepwidth = 1f / frameSteps;
        renderer = GetComponent<MeshRenderer>();
        rendererColor = renderer.material.color;
        alphaColor = Color.white;
        alphaColor.a = 0f;
        body = GetComponent<Rigidbody>();
        statictransformScale = transform.localScale * 1.5f;

    }
	
	// Update is called once per frame
	void OnCollisionEnter(Collision collisionInfo) {
        fade = 0f;

        ContactPoint contact;
        Vector3 localPoint;
        // theres only one contact point
        contact = collisionInfo.contacts[0];
        body.velocity = Vector3.zero;
        System.String collidername = contact.otherCollider.name;
        if (collidername.Contains("HandCursor"))
        {
            flag = true;
            localPoint = contact.thisCollider.transform.localPosition;
            time = Time.realtimeSinceStartup;
            //var collision = new Vector4(contact.point.x, contact.point.y, contact.point.z, time);
            //Debug.Log("DEBUG:"+collision);
            //renderer.material.SetVector("_Collision", collision);
            audio.Play();
            if (forceFeedback)
            {
                SteamVR_Controller.Input(deviceID).TriggerHapticPulse(800);
                Debug.Log("Triggered haptic pulse");
            }
            contact.thisCollider.enabled = false;
            StartCoroutine(FadeAlphaAndScale());
         }

    }

    void Update()
    {
        //if (deltatime > 0)
        //    Debug.LogWarning("deltat:" + deltatime);
    }

    IEnumerator FadeAlphaAndScale()
    {
        //var starttime = Time.realtimeSinceStartup;
        //for (fade = 0f; fade <= 1; fade += stepwidth)
        //{
        //transform.localScale += Vector3.one * fade * 0.1f;
        //rendererColor.a -= fade;
        float startTime = Time.time;
        float animationTimeDuration = 0.1f;
        while (Time.time < startTime + animationTimeDuration)
        { 
          
            transform.localScale = Vector3.Lerp(transform.localScale, statictransformScale, (Time.time - startTime) / animationTimeDuration);
            rendererColor = Vector4.Lerp(rendererColor, alphaColor, (Time.time - startTime) /animationTimeDuration);
            renderer.material.color = rendererColor;

            yield return null;
        } 
           
            if (Mathf.Approximately(rendererColor.a, 0f))
            {
                //Debug.LogWarning("[Destroyed]");
                DestroyImmediate(body.gameObject);
            }
        }
        //deltatime = Time.realtimeSinceStartup - starttime;

    //}
}
