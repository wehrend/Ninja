﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Linq;
using Assets.LSL4Unity.Scripts;

namespace Assets.MobiSA.Scripts
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Mesh))]
    public abstract class MovingRigidbodyPhysics : MonoBehaviour
    {
        public AudioSource audio;
        public AudioClip HitSound;
        public AudioClip MissSound;
        public AudioClip FalseAlarmSound;

        [HideInInspector]
        public Rigidbody Body { get; private set; }
        [HideInInspector]
        public MobiSACoreEventController ninjaEvents;
        private MobiSACore core;

        [HideInInspector]
        private MeshRenderer meshrenderer;
        public float distance;
        public float velocity = 5.0f;//Random.Range(1,20);
        public float distractorDestroyDistance;
        public Vector3 startPoint;
        public Color32 color = Color.white;
        public string type;
        public float hoverStrenght = 10f;
        //public float hoverHeight = 1.6f;
        public float environmentCorrection = -0.6f;
        //float hoverHeightWithEnvironment;
        public float range = 0.005f;

        [HideInInspector]
        public Vector3 target;
        //floor is layermask 8, this is needed for raycasting 
        [HideInInspector]
        public int layermask = 1 << 8;
        public float breakForce=50f;
        ExperimentMarker experimentMarker;

        private void Awake()
        {
            audio = GetComponent<AudioSource>();
                   
            Body = GetComponent<Rigidbody>();

            meshrenderer = GetComponent<MeshRenderer>();
            ninjaEvents = FindObjectOfType(typeof(MobiSACoreEventController)) as MobiSACoreEventController;
            core = FindObjectOfType(typeof(MobiSACore)) as MobiSACore;
            if (core != null)
                distractorDestroyDistance = core.distractorDestroyDistance;
            Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        void Start()
        {
            var experimentSceneController = FindObjectsOfType(typeof(ExperimentSceneController)).FirstOrDefault() as ExperimentSceneController;
            experimentMarker = experimentSceneController.GetExperimentMarker();
            //if (experimentMarker != null)
            // Debug.Log("Found expMarker for touch trial");
            //meshrenderer.enabled = false;
            meshrenderer.material.color = color;

            //Todo: Get rid of this logic, currently necessary to let objects fly
            //by beyond the subject 
            target = -transform.position;
            target.y = -target.y;
            //hoverHeightWithEnvironment = hoverHeight + environmentCorrection;
        }


        void FixedUpdate()
        {
            
            //constant choosen by testing, may vary between 1.6 to 1.9
            const float hideMultiplier = 1.75f;
            /*Ray ray = new Ray(Body.transform.position, -transform.up);
         
            RaycastHit hit;
            // Check if we are over floor right now.
            if (Physics.Raycast(ray, out hit, hoverHeight, layermask))
            {
                float propHeight = (hoverHeight - hit.distance) / hoverHeight;
                Vector3 appliedHovering = Vector3.up * propHeight * hoverStrenght;
                Body.AddForce(appliedHovering, ForceMode.Acceleration);

            }*/
          /*  else
            {
                //stabilizing 
                if (transform.position.y > hoverHeight)
                {
                    Body.AddForce(Vector3.up * hoverStrenght);
                }
                else
                {
                    Body.AddForce(Vector3.up * (-hoverStrenght));
                }
            }*/
            // startpoint is signinverted of target 
            Vector3 currentDistance = (transform.position - startPoint);
            //Destroy if we have passed the subject. Distance is the radius
                 
            if (currentDistance.magnitude > hideMultiplier * distance)
                DestroyObject(Body.gameObject, 0.01f);
            
            //also destroy if its before eyes (and distractor)

            Vector3 distanceToHead = (Camera.main.transform.position - transform.position);
            //Debug.Log(type);
            if ((distanceToHead.magnitude < distractorDestroyDistance) && (type.ToLowerInvariant().Equals("target"))) {
                audio.clip = MissSound;
                audio.Play();
                Debug.Log("playMiss");
                experimentMarker.PlaySound("Miss");
                DestroyObject(Body.gameObject, 0.05f);
            }
               


            Body.transform.LookAt(target);
            Body.AddRelativeForce(Vector3.forward * velocity, ForceMode.Force);

            /*if ( ((hoverHeightWithEnvironment - range) < Body.transform.position.y) && (Body.transform.position.y < ((hoverHeightWithEnvironment + range))))
            {
                meshrenderer.enabled = true;
                //meshrenderer.material.color = Color.blue; //Debug
                      
            }*/

        }


        private void OnCollisionEnter(Collision collision)
        {

            var collisionForce = GetCollisionForce(collision);
            foreach (ContactPoint contact in collision.contacts)
            {
                print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            }


            if (collisionForce > 0)
            {
                //switch physics of and set velcoitys to zero
               /* Body.isKinematic = true;
                Body.velocity = Vector3.zero;
                Body.AddTorque(0f, 0f, 3f);
                */
                CollisionWithForce(collisionForce);
            }
        }

        abstract public void CollisionWithForce(float force);

        private float GetCollisionForce(Collision collision)
        {
            Debug.Log(collision.collider.name);

           /* if ((collision.collider.name.Contains("Paddle") && collision.collider.GetComponent<Paddle>().CollisionForce() > breakForce))
            {
                return collision.collider.GetComponent<Paddle>().CollisionForce() * 1.2f;
            }*/

            if ((collision.collider.name.Contains("HandCursor_edited")))
            {

                //We want markers only for these targets touched by controller.
                if (experimentMarker != null)
                    experimentMarker.Touch(collision.gameObject);
                else
                {
                    Debug.LogError("Some trial touched, but no Instance of experimentMarker found ");
                }
                if (this.type.ToLowerInvariant().Equals("target"))
                {
                    audio.clip = HitSound;
                    Debug.Log("playHit");
                    experimentMarker.PlaySound("Hit");
                }
                if (this.type.ToLowerInvariant().Equals("distract"))
                {
                    audio.clip = FalseAlarmSound;
                    Debug.Log("playFalseAlarm");
                    experimentMarker.PlaySound("FalseAlarm");
                }
                audio.Play();

                Debug.LogWarning("Controller or Hands collision: " + collision.collider.name);
                return 100 * 1.2f;
            }
            else {
                return 0;
                }
        }
    }
}