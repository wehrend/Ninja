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
        public int damagePoints = 4;
        public float explosionMultiplier = 0.3f;

        private void OnCollisionEnter(Collision collision)
        {
            int damage =  GetCollisionForce(collision);

            if (Body && damage > 0)
            {
                if (gameController)
                    gameController.issueDamage(damage);
                Debug.Log("Bomb damaged you with" + damage + "damage!\n");
            }
            Destroy(Body.gameObject);
        }

        private int GetCollisionForce(Collision collision)
        {
            if ((collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce))
            {
                return (int)(collision.collider.GetComponent<Sword>().CollisionForce()/20) * damagePoints;
            }
            else if ((collision.collider.name.Contains("Paddle") && collision.collider.GetComponent<Paddle>().CollisionForce() > breakForce))
            {
                return (int)( collision.collider.GetComponent<Paddle>().CollisionForce()/20) * damagePoints;
            }

            return 0;
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