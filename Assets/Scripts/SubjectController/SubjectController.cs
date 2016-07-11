using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class SubjectController : MonoBehaviour {
  
    public CharacterController Body;
    public Transform Head;
    public Camera ViewerPerspective;

    private float speed = 6.0f;
    private float jumpSpeed= 8.0f;
    private float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (grounded) {
			GetCharacterMoves();
		}

	}

	void GetCharacterMoves(){
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDirection = new Vector3(horizontal, 0, vertical);
		//moveDirection = Transform.transformDirection(moveDirection);
		moveDirection *= speed;
        if (Input.GetButton ("Jump")) {
            moveDirection.y = jumpSpeed;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

}
