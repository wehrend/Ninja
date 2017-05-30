using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.VREF.Scripts;

public class FadeObjectOnCollision : MonoBehaviour {

    float distance;
    float time;
    public float fade=1f;
    bool flag = false;
    float stepwidth;
    public int frameSteps=10;
    public float waitTime;
    [Tooltip("Total Animation Time (ms)")]
    public int animationTimeDuration=100;
    private Vector4 objPos;
    MeshRenderer renderer;
    Rigidbody body;
    AudioSource audio;

    //[RequireComponent(typeof(AudioSource))]
	// Use this for initialization
	void Start () {
        audio =GetComponent<AudioSource>();
        //animation time duration in milliseconds for 100 steps
        waitTime = animationTimeDuration / (1000f*frameSteps);
        stepwidth = 1f / frameSteps;
        renderer = GetComponent<MeshRenderer>();
        body = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void OnCollisionEnter(Collision collisionInfo) {
        fade = 0f;
        flag = true;
        ContactPoint contact;
        Vector3 localPoint;
        // theres only one contact point
        contact = collisionInfo.contacts[0];

        System.String collidername = contact.otherCollider.name;
        if (collidername.Contains("HandCursor"))
        {
            localPoint = contact.thisCollider.transform.localPosition;
            time = Time.realtimeSinceStartup;
            //var collision = new Vector4(contact.point.x, contact.point.y, contact.point.z, time);
            //Debug.Log("DEBUG:"+collision);
            //renderer.material.SetVector("_Collision", collision);
            audio.Play();
            StartCoroutine(FadeAlphaAndScale());
         } else {
            //Debug.LogWarning("[Destroyed]");
            DestroyObject(body.gameObject, 0.5f);
        }


    }

    IEnumerator FadeAlphaAndScale()
    {
        for (fade = 0f; fade <= 1; fade += stepwidth)
        {
            transform.localScale += Vector3.one * fade * 0.1f;
            Color color = renderer.material.color;
            color.a -= fade;
            renderer.material.color = color;
            yield return new WaitForSeconds(waitTime);
            if (Mathf.Approximately(fade, 1f))
            {
                //Debug.LogWarning("[Destroyed]");
                DestroyObject(body.gameObject, 0.5f);
            }
        }

    }
}

/* old
// Update is called once per frame
void OnCollisionEnter(Collision collisionInfo)
{
    alpha = 0f;
    flag = true;
    ContactPoint contact;
    Vector3 localPoint;
    // theres only one contact point
    contact = collisionInfo.contacts[0];
    localPoint = contact.thisCollider.transform.localPosition;
    time = Time.realtimeSinceStartup;
    var collision = new Vector4(contact.point.x, contact.point.y, contact.point.z, time);
    //Debug.Log("DEBUG:"+collision);
    renderer.material.SetVector("_Collision", collision);
    audio.Play();

}
/*
private void Update()
{
    if (flag)
    {
        transform.localScale += Vector3.one * 0.001f;
        alpha += stepwidth;
        if (renderer)
            renderer.material.SetVector("_AlphaCutoff", new Vector4(alpha, 0f, 0f, 0f));
    }
    if (Mathf.Approximately(alpha, 1f))
    {
        //Debug.LogWarning("[Destroyed]");
        DestroyObject(body.gameObject, 0.5f);
    }
}

*/