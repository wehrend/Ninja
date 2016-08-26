using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

using VRTK;
using System;
using UnityEngine.Events;
//using Assets.LSL4Unity.Scripts;

namespace Assets.NinjaGame.Scripts
{

    public class Sword : VRTK_InteractableObject
    {
        private VRTK_ControllerActions controllerActions;
        private VRTK_ControllerEvents controllerEvents;
        //public LSLMarkerStream markerStream;

        private float impactMagnifier = 120f;
        private float collisionForce = 0f;

        private GrabbingProxy grabbingProxy;

        public float CollisionForce()
        {
            return collisionForce;
        }

        public override void Grabbed(GameObject grabbingObject)
        {
            base.Grabbed(grabbingObject);
            controllerActions = grabbingObject.GetComponent<VRTK_ControllerActions>();
            controllerEvents = grabbingObject.GetComponent<VRTK_ControllerEvents>();

            grabbingProxy.GenerateGrabEvent(grabbingObject);
        }

        protected override void Awake()
        {
            base.Awake();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            grabbingProxy = GetComponent<GrabbingProxy>();
        }


        private void OnCollisionEnter(Collision collision)
        {
            //markerStream.Write("Collision with Object");
            
            // if (IsGrabbed())
            //     Debug.Log("IsGrabbed()");
            if (controllerActions && controllerEvents && IsGrabbed())
            {
                collisionForce = controllerEvents.GetVelocity().magnitude * impactMagnifier;
                controllerActions.TriggerHapticPulse((ushort)collisionForce, 0.5f, 0.01f);
                // ScoreAndStats.scores += scores;
            }
            else
            {
                collisionForce = collision.relativeVelocity.magnitude * impactMagnifier;
            }
        }
    }
}