using UnityEngine;
using System.Collections;
using VRTK;
using System;
using UnityEngine.Events;
//using Assets.LSL4Unity.Scripts;

namespace Assets.NinjaGame.Scripts
{
    public class Weapon : VRTK_InteractableObject
    {
            private VRTK_ControllerActions controllerActions;
            private VRTK_ControllerEvents controllerEvents;
            //public LSLMarkerStream markerStream;
            private GameController gameController;
            private float impactMagnifier = 120f;
            private float collisionForce = 0f;
            public ushort forceFeedbackIntensity = 3;
            public float forceFeedbackDuration = 0.5f;
            public float forceFeedbackPulse = 0.01f;
            //private GrabbingProxy grabbingProxy;



        public float CollisionForce()
            {
                return collisionForce;
            }

            public override void Grabbed(GameObject grabbingObject)
            {
                base.Grabbed(grabbingObject);
                controllerActions = grabbingObject.GetComponent<VRTK_ControllerActions>();
                controllerEvents = grabbingObject.GetComponent<VRTK_ControllerEvents>();

                // grabbingProxy.GenerateGrabEvent(grabbingObject);
            }

            protected override void Awake()
            {
                base.Awake();
                gameController = FindObjectOfType(typeof(GameController)) as GameController;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                //grabbingProxy = GetComponent<GrabbingProxy>();
            }

            protected override void Update()
            {
                base.Update();
                if (gameController.getHealth() < 3)
                    GameOver();


            }

        private void OnCollisionEnter(Collision collision)
            {
                //markerStream.Write("Collision with Object");

                // if (IsGrabbed())
                //     Debug.Log("IsGrabbed()");
                if (controllerActions && controllerEvents && IsGrabbed())
                {
                    collisionForce = controllerEvents.GetVelocity().magnitude * impactMagnifier;
                    if (collision.collider.name.Contains("Bomb")) 
                        controllerActions.TriggerHapticPulse((ushort)(collisionForce * forceFeedbackIntensity*3), forceFeedbackDuration, forceFeedbackPulse);
                    else
                        controllerActions.TriggerHapticPulse((ushort)(collisionForce * forceFeedbackIntensity), forceFeedbackDuration, forceFeedbackPulse );
                // ScoreAndStats.scores += scores;
                }
                else
                {
                    collisionForce = collision.relativeVelocity.magnitude * impactMagnifier;
                }
            }

            public void GameOver()
            {
                Debug.Log("Game Over!");
                //this.Ungrabbed(grabbingObject);
            }

        }
}