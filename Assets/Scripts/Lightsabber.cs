using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Lightsabber : MonoBehaviour {

    public Rigidbody lightsabber;
    private Camera mainCamera;

	// Use this for initialization
	void Start() {
        mainCamera = Camera.main;
        lightsabber = GetComponent<Rigidbody>();
        lightsabber.rotation = Quaternion.Euler(45f, 0f, 45f);
        lightsabber.position = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
	}

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 screen = Input.mousePosition;
        screen.z = 1.0f;
        Vector3 position = mainCamera.ScreenToWorldPoint(screen);
        position.y = 1.0f;
        lightsabber.MovePosition(position);
        //Debug.Log("Lightsabber Position" + lightsabber.position);
    }

 /*   void OnDrawGizmos() {

        Vector3 screen = Input.mousePosition;
        screen.z = 10.0f;
	    Vector3 position= mainCamera.ScreenToWorldPoint(screen);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(position, 0.1F);
        //Debug.Log("Lightsabber Position" + lightsabber.position);
    }*/

   void OnMouseDown()
    {
        //lightsabber.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log("Lightsabber Position" + lightsabber.position);
    }
    
}
