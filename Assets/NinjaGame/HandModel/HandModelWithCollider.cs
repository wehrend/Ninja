using UnityEngine;
using System.Collections;
using VRTK;

namespace Hand
{
    [RequireComponent(typeof(Animator))]
    public class HandModelWithCollider : MonoBehaviour
    {
        Animator animator;
        VRTK_ControllerEvents events;
        public CapsuleCollider collider;

        int idle, point, open, gesture, fist, pick, grab;

        void Awake()
        {
            events = GetComponentInParent<VRTK_ControllerEvents>();
            if (events == null)
                Debug.LogError("Vrtk controller events not found");
            animator = GetComponent<Animator>();
            point = Animator.StringToHash("Point");
            idle = Animator.StringToHash("Idle");
            gesture = Animator.StringToHash("Gesture");
            open = Animator.StringToHash("Open");
            fist = Animator.StringToHash("Fist");
            pick = Animator.StringToHash("Pick");
            grab = Animator.StringToHash("Grab");
        }

        void Start() {
            Point();
        }



        void Update()
        {/*
            if (events.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.Touchpad_Touch))
                Point();
            else
                Idle();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Idle();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Point();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Gesture();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Open();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Fist();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Pick();
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Grab();
            }*/

        }

        [ContextMenu("Idle")]
        public void Idle()
        {
           // Debug.Log("Set Idle");
            animator.SetTrigger(idle);
        }

        [ContextMenu("Point")]
        public void Point()
        {
            animator.SetTrigger(point);
            //collider for Pointer Gesture 
            collider = animator.GetComponent<CapsuleCollider>();
            collider.direction = 2; //z-Axis
            collider.height = 0.31f;//0.31f;
            collider.radius = 0.04f;//0.05f;
            collider.center = new Vector3(0, 0.18f, -0.1f);
            Debug.LogWarning("Pointer collider set.");
        }

        [ContextMenu("Gesture")]
        public void Gesture()
        {
            animator.SetTrigger(gesture);
        }

        [ContextMenu("Open")]
        public void Open()
        {
            animator.SetTrigger(open);
        }

        [ContextMenu("Fist")]
        public void Fist()
        {
            Debug.Log("Set Fist");
            animator.SetTrigger(fist);
        }

        [ContextMenu("Pick")]
        public void Pick()
        {
            animator.SetTrigger(pick);
        }

        [ContextMenu("Grab")]
        public void Grab()
        {
            animator.SetTrigger(grab);
        }


    }
}
