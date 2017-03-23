using UnityEngine;
using System.Collections;
//using VRTK;


/// <summary>
/// This is actually a Script for abn obsolete prefab, may be deprecated in future
/// </summary>
/*namespace Assets.NinjaGame.Scripts
{

    [RequireComponent(typeof(MeshRenderer))]
    public class Shield : VRTK_InteractableObject
    {
        private VRTK_ControllerActions controllerActions;
        private VRTK_ControllerEvents controllerEvents;

        private float protectionPower = 500f;
        private float collisionForce = 0f;
        private Renderer render;


        public float CollisionForce()
        {
            return collisionForce;
        }

        public override void Grabbed(GameObject grabbingObject)
        {
            base.Grabbed(grabbingObject);
            controllerActions = grabbingObject.GetComponent<VRTK_ControllerActions>();
            controllerEvents = grabbingObject.GetComponent<VRTK_ControllerEvents>();
        }

        protected override void Awake()
        {
            base.Awake();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            render = GetComponent<Renderer>();
        }


        private void OnCollisionEnter(Collision collision)
        {

            if (controllerActions && controllerEvents && IsGrabbed())
            {
                StartCoroutine(blinkShield(3, 0.3f));
                protectionPower -= 50f;
                Debug.Log("Shield Protection");
                if (protectionPower < 100f)
                    StartCoroutine(blinkShield(3, 0.1f));

            }
        }

        IEnumerator blinkShield(float duration, float blinkTime)
        {
            while (duration > 0f)
            {
                duration -= Time.deltaTime;

                //toggle renderer
                render.enabled = !render.enabled;

                //wait for a bit
                yield return new WaitForSeconds(blinkTime);
            }

            //make sure renderer is enabled when we exit
            render.enabled = true;
        }

    }
}*/
