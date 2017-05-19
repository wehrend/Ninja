using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.VREF.Scripts;

public class FadeObjectOnCollision : MonoBehaviour {

    float distance;
    float time;
    public float alpha=1f;
    bool flag = false;
    float stepwidth;
    public int frameSteps;
    public float waitTime;
    [Tooltip("Total Animation Time (ms)")]
    public int animationTimeDuration=100;
    private Vector4 objPos;
    MeshRenderer renderer;
    Rigidbody body;

	// Use this for initialization
	void Start () {
        //animation time duration in milliseconds for 100 steps
        waitTime = animationTimeDuration / (1000f*frameSteps);
        stepwidth = 1f / frameSteps;
        renderer = GetComponent<MeshRenderer>();
        body = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void OnCollisionEnter(Collision collisionInfo) {
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
        

    }

    private void Update()
    {
        if (flag)
        {
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



    /*IEnumerator FadeAlpha()
    {
        for (alpha = 0f; alpha <= 1; alpha += 0.01f)
        {
            renderer.material.SetVector("_AlphaCutoff", new Vector4(alpha,0f,0f,0f));
           
            yield return new WaitForSeconds(waitTime);
            if (Mathf.Approximately(alpha, 1f))
            {
                //Debug.LogWarning("[Destroyed]");
                DestroyObject(body.gameObject, 0.5f);
            }
        }

    }*/
}
