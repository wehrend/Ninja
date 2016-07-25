using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Fruit : MonoBehaviour {

   public Rigidbody Body {get; private set;}
   float breakForce=150f;

    private void Start()
    {
        Body = GetComponent<Rigidbody>();
        Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }


    private void OnCollisionEnter(Collision collision)
    {
        var collisionForce = GetCollisionForce(collision);
        Debug.Log("Fruit contacted!");
        if (collisionForce > 0)
        {
            Destroy(this.gameObject,5f);
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
