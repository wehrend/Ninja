﻿using UnityEngine;
using System.Collections;
using VRTK;

public class Bomb : MovingRigidbodyPhysics {

    public float breakForce=50f;
    //let it be one of a mild bomb
    public int damagePoints = 5;

    private void OnCollisionEnter(Collision collision)
    {
        var damage= GetCollisionForce(collision);

        if (Body && damage > 0 )
        {
            // here we need code
            //health -= damage;
            //Debug.Log("Bomb damaged you with" + damage + "damage!\n Your health is only" +health);
        }
    }

    private float GetCollisionForce(Collision collision)
    {
        if ((collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce))
        {
            return collision.collider.GetComponent<Sword>().CollisionForce() * damagePoints;
        }
        else if (collision.collider.name.Contains("Shield") )
        {
            return 0f;
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
