using UnityEngine;
using System.Collections;
using System.Linq;
using VRTK;

namespace Assets.NinjaGame.Scripts
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Mesh))]
    public abstract class MovingRigidbodyPhysics : MonoBehaviour
    {


        [HideInInspector]
        public Rigidbody Body { get; private set; }
        [HideInInspector]
        protected NinjaGameEventController ninjaControl;
        [HideInInspector]
        protected NinjaGameEventArgs eve;
        [HideInInspector]
        private MeshRenderer meshrenderer;
        private ParticleSystem particleSys;
        public float distance;
        public Vector3 startPoint;
        public Color32 color = Color.white;
        public bool particleEffects= false;
        public float hoverStrenght = 140f;
        public float hoverHeight = 2.5f;
        public float velocity = 5.0f;//Random.Range(1,20);
        [HideInInspector]
        public Vector3 target;
        //floor is layermask 8, this is needed for raycasting 
        [HideInInspector]
        public int layermask = 1 << 8;
        public float breakForce=50f;


        private void Awake()
        {
            Body = GetComponent<Rigidbody>();
            meshrenderer = GetComponent<MeshRenderer>();
            particleSys= GetComponent<ParticleSystem>();
            ninjaControl = FindObjectOfType(typeof(NinjaGameEventController)) as NinjaGameEventController;
            Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        void Start()
        {
            meshrenderer.material.color = color;
            //Todo: Get rid of this logic, currently necessary to let objects fly
            //by beyond the subject 
            target = -transform.position;
            target.y = -target.y;
        }


        void FixedUpdate()
        {
            //constant choosen by testing, may vary between 1.6 to 1.9

            const float hideMultiplier = 1.75f;
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
            
       
            if (currentDistance.magnitude > hideMultiplier * distance)
                DestroyObject(Body.gameObject, 0.01f);

            Body.transform.LookAt(target);
            //Sword sword = FindObjectOfType<Sword>();
            //TODO: We want event mnessaging here
            //if (sword.IsGrabbed()) 
            if (Time.realtimeSinceStartup > 5)
                Body.AddRelativeForce(Vector3.forward * velocity, ForceMode.Force);
        }


        private void OnCollisionEnter(Collision collision)
        {
            var collisionForce = GetCollisionForce(collision);
            
            if (collisionForce > 0)
                CollisionWithForce(collisionForce);
        }

        void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
        }

        abstract public void CollisionWithForce(float force);

        private float GetCollisionForce(Collision collision)
        {

            if ((collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce))
            {
                return collision.collider.GetComponent<Sword>().CollisionForce() * 1.2f;
            }

            if ((collision.collider.name.Contains("Paddle") && collision.collider.GetComponent<Paddle>().CollisionForce() > breakForce))
            {
                return collision.collider.GetComponent<Paddle>().CollisionForce() * 1.2f;
            }
            /*if ((collision.collider.name.Contains("ControllerCollider") && collision.collider.GetComponent<HandCursorController>().CollisionForce() > breakForce))
            {
               return collision.collider.GetComponent<HandCursorController>().CollisionForce()  1.2f;
                Debug.LogWarning("Controller or Hands collision: " + collision.collider.name);
            }*/
            else
            {
                Debug.LogWarning("Controller or Hands collision: " + collision.collider.name);
                return 100 * 1.2f;
            }
        }
    }
}