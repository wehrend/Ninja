using UnityEngine;
using System.Collections;
using VRTK;

namespace Assets.NinjaGame.Scripts
{

    public class Fruit : MovingRigidbodyPhysics
    {

        public float breakForce = 50f;
        public int bonusPoints = 50;

        private void OnCollisionEnter(Collision collision)
        {
            var collisionForce = GetCollisionForce(collision);

            if (Body && collisionForce > 0)
            {
                GetComponent<Renderer>().material.color = Color.red;
                Destroy(Body.gameObject, 0.5f);
                if (gameController)
                {
                    Debug.Log("Game Controller found");
                }
                gameController.issueBoni(bonusPoints);
                Debug.Log("Fruit killed!");
            }
        }


        private float GetCollisionForce(Collision collision)
        {
            if ((collision.collider.name.Contains( "Sword" )  && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce))
            {
                return collision.collider.GetComponent<Sword>().CollisionForce() * 1.2f;
            }
            if ((collision.collider.name.Contains("Paddle") && collision.collider.GetComponent<Paddle>().CollisionForce() > breakForce))
            {
                return collision.collider.GetComponent<Paddle>().CollisionForce() * 1.2f;
            }
            else if ((collision.collider.name.Contains("RigidbodyContainer") ))
            {
                Debug.Log("Hands collision");
                return 50 * 1.2f;
            }

            return 0f;
        }

    }

}