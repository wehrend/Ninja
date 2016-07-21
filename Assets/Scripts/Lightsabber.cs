using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Lightsabber : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        //device.TriggerHapticPulse(500);
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }

}
