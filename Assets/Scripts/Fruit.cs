using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Fruit : MonoBehaviour {

   public Rigidbody Body {get; private set;}
   private MeshRenderer renderer;
   float breakForce=50f;

    private void Start()
    {
        Body = GetComponent<Rigidbody>();
        renderer = Body.GetComponent<MeshRenderer>();
        Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }


    private void OnCollisionEnter(Collision collision)
    {
        var collisionForce = GetCollisionForce(collision);
        renderer.material.color = Color.green;
        Debug.Log("Fruit contacted!");

        if (collisionForce > 0)
        {
            renderer.material.color = Color.blue;
            Destroy(Body.gameObject,1f);
       
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
}
