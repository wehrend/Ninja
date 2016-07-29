using UnityEngine;
using System.Collections;
using VRTK;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Fruit : MonoBehaviour {

    public Rigidbody Body {get; private set;}
    private MeshRenderer renderer;
    public float breakForce=50f;
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
        if (Body)
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
            Sword sword = FindObjectOfType<Sword>();
            //TODO: We want event mnessaging here
            if (sword.IsGrabbed())
            {
                Body.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collisionForce = GetCollisionForce(collision);

        if (Body && collisionForce > 0 )
        {
            renderer.material.color = Color.red;
            Destroy(Body.gameObject,0.5f);
       
            Debug.Log("Fruit killed!");
        }
    }

    private float GetCollisionForce(Collision collision)
    {
        if ((collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce))
        {
            return collision.collider.GetComponent<Sword>().CollisionForce() * 1.2f;
        }

        return 0f;
    }

   /* void OnTriggerEnter(Collider enteredCollider)
    {
        if (Body && enteredCollider.CompareTag("kill zone") )
        { 
            Destroy(gameObject);
        }
    }*/
}
