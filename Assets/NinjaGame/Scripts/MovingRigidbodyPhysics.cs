﻿using UnityEngine;
using System.Collections;
using VRTK;

namespace Assets.NinjaGame.Scripts
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MovingRigidbodyPhysics : MonoBehaviour
    {



        public Rigidbody Body { get; private set; }
        protected GameController gameController;
        private MeshRenderer renderer;
        public float hoverStrenght = 140f;
        public float hoverHeight = 2.5f;
        public float speed = 25f;
        public Vector3 target;


        private void Awake()
        {
            Body = GetComponent<Rigidbody>();
            gameController = FindObjectOfType(typeof(GameController)) as GameController;
            renderer = Body.GetComponent<MeshRenderer>();
            Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
           
        }

        void Start()
        {
            target = -transform.position;
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
            if (Time.realtimeSinceStartup > 5)
                Body.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
        }

        void OnTriggerEnter(Collider enteredCollider)
        {
            //should be kill zone 
            Destroy(Body.gameObject);
        }
    }
}