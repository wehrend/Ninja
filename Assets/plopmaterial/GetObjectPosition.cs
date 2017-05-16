using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectPosition : MonoBehaviour {

    float distance;
    Vector3 mousePos;
    float isHit = 0.0f;
    float alpha=0f;
    float time;
    float factor = 0.020f;
    float waitTime = 0.005f;
    private Vector4 objPos;

    MeshRenderer renderer;
    Rigidbody body;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<MeshRenderer>();
        body = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void OnCollisionEnter(Collision collisionInfo) {
        ContactPoint contact;
        Vector3 localPoint;

        //Debug.LogWarning("[COLLISION] ContactPoints: " + collisionInfo.contacts.Length);

        // theres only one contact point
        contact = collisionInfo.contacts[0];
        localPoint = contact.thisCollider.transform.localPosition;
        time = Time.realtimeSinceStartup;
        var collision = new Vector4(contact.point.x, contact.point.y, contact.point.z, time);
        Debug.Log("DEBUG:"+collision);
        renderer.material.SetVector("_Collision", collision);
        StartCoroutine(FadeAlpha());

    }

    IEnumerator FadeAlpha()
    {
        for (alpha = 0f; alpha <= 1; alpha += factor)
        {
            renderer.material.SetVector("_AlphaCutoff", new Vector4(alpha, alpha, alpha, alpha));
           
            yield return new WaitForSeconds(waitTime);
            if (Mathf.Approximately(alpha, 1f))
            {
                Debug.LogWarning("[Destroyed]");
                DestroyObject(body.gameObject, 0.5f);
            }
        }

    }
}
