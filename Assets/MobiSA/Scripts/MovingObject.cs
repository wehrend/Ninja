using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace Assets.MobiSA.Scripts
{

    public class MovingObject : MovingRigidbodyPhysics
    {
        private MobiSACoreEventArgs eve;
        //public float fruitBreakForce = 50f;  
        public int bonusPoints = 50;




        public override void CollisionWithForce( float collisionForce)
        {
            if (ninjaEvents != null)
            {
                Debug.Log("Trialtype " + type);
                if (type.Equals("Distract"))
                {
                    ninjaEvents.OnBombCollision(eve);
                }
                else if (type.Equals("Target"))
                {
                    ninjaEvents.OnFruitCollision(eve);
                }
            }
            //gameController.issueBoni(bonusPoints);
            //Debug.Log("Fruit killed!");
        }

    }


}