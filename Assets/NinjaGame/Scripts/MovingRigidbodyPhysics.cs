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



        public Rigidbody Body { get; private set; }
        protected GameController gameController;
        private MeshRenderer meshrenderer;
        private ParticleSystem ps;
        public float distance;
        public Vector3 startPoint;
        public Color32 color = Color.white;
        public bool particleEffects= false;
        public float hoverStrenght = 140f;
        public float hoverHeight = 2.5f;
        public float speed = 5.0f;//Random.Range(1,20);
        public Vector3 target;
        //floor is layermask 8
        public int layermask = 1 << 8;
        public float breakForce=50f;


        private void Awake()
        {
            Body = GetComponent<Rigidbody>();
            meshrenderer = GetComponent<MeshRenderer>();
            ps= GetComponent<ParticleSystem>();
            gameController = FindObjectOfType(typeof(GameController)) as GameController;
            Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        void Start()
        {
            meshrenderer.material.color = color;
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

       /* void OnTriggerEnter(Collider enteredCollider)
        {
            //should be kill zone 
            Destroy(Body.gameObject);
        }*/

        private void OnCollisionEnter(Collision collision)
        {
            var collisionForce = GetCollisionForce(collision);
      
            if (collisionForce > 0)
                CollisionWithForce(collisionForce);
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
            else
            {
                Debug.LogWarning("Controller or Hands collision: " + collision.collider.name);
                return 100 * 1.2f;
            }
        }
    }
}