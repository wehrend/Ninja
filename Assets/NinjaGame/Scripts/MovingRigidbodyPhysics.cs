using UnityEngine;
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
        //private MeshRenderer renderer;
        public float distance;
        public Vector3 startPoint;
        public float hoverStrenght = 140f;
        public float hoverHeight = 2.5f;
        public float speed = 5.0f;//Random.Range(1,20);
        public Vector3 target;
        //floor is layermask 8
        public int layermask = 1 << 8;


        private void Awake()
        {
            Body = GetComponent<Rigidbody>();
            gameController = FindObjectOfType(typeof(GameController)) as GameController;
            //renderer = Body.GetComponent<MeshRenderer>();
            Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
           
        }

        void Start()
        {
            target = -transform.position;
            target.y = -target.y;
        }


        void FixedUpdate()
        {

            Ray ray = new Ray(Body.transform.position, -transform.up);
            RaycastHit hit;
            // Check if we are over floor right now.
            if (Physics.Raycast(ray, out hit, hoverHeight,layermask))
            {
                float propHeight = (hoverHeight - hit.distance) / hoverHeight;
                Vector3 appliedHovering = Vector3.up * propHeight * hoverStrenght;
                Body.AddForce(appliedHovering, ForceMode.Acceleration);

            }
            // startpoint is signinverted of target 
            Vector3 currentDistance = (transform.position - startPoint);
            //Destroy if we have passed the subject. Distance is the radius 
            if (currentDistance.magnitude > 1.74f * distance)
                DestroyObject(Body.gameObject, 0.01f);

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