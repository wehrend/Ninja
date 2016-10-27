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
            public NinjaGameEventController ninjaGameEvent;
            public NinjaGameEventArgs eve;
            private float impactMagnifier = 120f;
            private float collisionForce = 0f;
            public ushort forceFeedbackIntensity = 3;
            public float forceFeedbackDuration = 0.5f;
            public float forceFeedbackPulse = 0.01f;
            //private GrabbingProxy grabbingProxy;


           /* public override void Start()
            {
                base.Start();
               // ninjaGameEvent.StartGame += Start;
               // ninjaGameEvent.GameOver += GameOver;
            //GetComponent<VRTK_ControllerEvents>().AliasGrabOn() += new ControllerInteractionEventHandler(GrabWeapon);
            //GetComponent<VRTK_ControllerEvents>().AliasGrabOff() += new ControllerInteractionEventHandler(UngrabWeapon);
            //GetComponent<VRTK_ControllerEvents>().AliasUseOn() += new ControllerInteractionEventHandler(UseWeapon);
            //GetComponent<VRTK_ControllerEvents>().AliasUseOff() += new ControllerInteractionEventHandler(DeUseWeapon);
        }*/



            public float CollisionForce()
            {
                return collisionForce;
            }

            public override void Grabbed(GameObject grabbingObject)
            {
                base.Grabbed(grabbingObject);
                controllerActions = grabbingObject.GetComponent<VRTK_ControllerActions>();
                controllerEvents = grabbingObject.GetComponent<VRTK_ControllerEvents>();
                controllerActions.SetControllerOpacity(0.0f);
               
            }

        public override void Ungrabbed(GameObject grabbingObject)
        {
            base.Ungrabbed(grabbingObject);
            controllerActions = grabbingObject.GetComponent<VRTK_ControllerActions>();
            controllerEvents = grabbingObject.GetComponent<VRTK_ControllerEvents>();
            controllerActions.SetControllerOpacity(1f);
        }

        protected override void Awake()
            {
                base.Awake();
                
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                //grabbingProxy = GetComponent<GrabbingProxy>();
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

        }
}