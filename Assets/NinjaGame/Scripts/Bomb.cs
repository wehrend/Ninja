using UnityEngine;
using System.Collections;
//using UnityStandardAssets.Effects;
using VRTK;

namespace Assets.NinjaGame.Scripts
{

    public class Bomb : MovingRigidbodyPhysics
    {

        public float breakForce = 50f;
        //let it be one of a mild bomb
        public int damagePoints = 5;
        public float explosionMultiplier = 0.3f;


        void Start()
        {

         /* var systems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems)
            {
                system.startSize *= explosionMultiplier;
                system.startSpeed *= explosionMultiplier;
                system.startLifetime *= Mathf.Lerp(explosionMultiplier, 1, 0.5f);
                system.Clear();
                system.Play();
            }*/
        }




        private void OnCollisionEnter(Collision collision)
        {
            var damage = GetCollisionForce(collision);

            if (Body && damage > 0)
            {
                if (gameController)
                    gameController.issueDamage(damage);
                Debug.Log("Bomb damaged you with" + damage + "damage!\n");
            }
        }

        private float GetCollisionForce(Collision collision)
        {
            if ((collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce))
            {
                return collision.collider.GetComponent<Sword>().CollisionForce() * 0.01f * damagePoints;
            }
            else if (collision.collider.name.Contains("Shield"))
            {
                return 0f;
            }
            return 0f;
        }


        /* void OnTriggerEnter(Collider enteredCollider)
         {
             if (Body && enteredCollider.CompareTag("kill zone") )
             {
                 Destroy(gameObject);
             }
         }*/
    }
}