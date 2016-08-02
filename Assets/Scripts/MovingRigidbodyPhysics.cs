using UnityEngine;
using System.Collections;
using VRTK;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class MovingRigidbodyPhysics : MonoBehaviour {


    public Rigidbody Body {get; private set;}
    private MeshRenderer renderer;
    public float hoverStrenght = 140f;
    public float hoverHeight = 2.5f;
    public float speed = 25f;
    public Transform target;
    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
        renderer = Body.GetComponent<MeshRenderer>();
        Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(Body.transform.position, -transform.up);
		RaycastHit hit;
		// Check if we are over floor right now.
		if (Physics.Raycast(ray, out hit, hoverHeight))
		{
			float propHeight = (hoverHeight - hit.distance) / hoverHeight;
			Vector3 appliedHovering = Vector3.up * propHeight * hoverStrenght;
			Body.AddForce(appliedHovering, ForceMode.Acceleration);

		}
		Body.transform.LookAt(target);
		//Sword sword = FindObjectOfType<Sword>();
		//TODO: We want event mnessaging here
		//if (sword.IsGrabbed())
        if (Time.realtimeSinceStartup > 5 )
	        Body.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);

    }
}
