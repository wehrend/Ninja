using UnityEngine;
using System.Collections;
using VRTK;

namespace Assets.NinjaGame.Scripts
{
    public class Fruit : MovingRigidbodyPhysics
    {
        //public float fruitBreakForce = 50f;  
        public int bonusPoints = 50;

        public override void CollisionWithForce( float collisionForce)
        {
            Destroy(Body.gameObject, 0.5f);
            /* if (gameController)
             {
                 Debug.Log("Game Controller found");
             }*/
            
            eve.score = bonusPoints;
            if (ninjaControl)
                ninjaControl.OnFruitCollision(eve);
            //gameController.issueBoni(bonusPoints);
            Debug.Log("Fruit killed!");
        }
    }
}