using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Fruit : MonoBehaviour {

   public Rigidbody Body {get; private set;}

   void Awake()
    {
        Body = GetComponent<Rigidbody>();
    }


    void OnTriggerEnter(Collider enteredCollider)
    {
    //  if (enteredCollider.CompareTag("kill zone"))
    //            Destroy(gameObject);
    }
}
