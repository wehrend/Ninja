using UnityEngine;
using System.Collections;
using VRTK;

[RequireComponent(typeof(MeshRenderer))]
public class Shield : VRTK_InteractableObject
{
    private VRTK_ControllerActions controllerActions;
    private VRTK_ControllerEvents controllerEvents;

    private float protectionPower = 500f;
    private float collisionForce = 0f;
    private MeshRenderer renderer;


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
        renderer = GetComponent<MeshRenderer>();
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (controllerActions && controllerEvents && IsGrabbed())
        {
            StartCoroutine(blinkShield(3,0.3f));
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
            renderer.enabled = !renderer.enabled;

            //wait for a bit
            yield return new WaitForSeconds(blinkTime);
        }

        //make sure renderer is enabled when we exit
        renderer.enabled = true;
    }

}
