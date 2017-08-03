using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Assets.NinjaGame.Scripts
{

    public class Bomb : MovingRigidbodyPhysics
    {
        private NinjaGameEventArgs eve;
        //let it be one of a mild bomb
        public int damagePoints = 5;
        public float explosionMultiplier = 0.3f;

        public override void CollisionWithForce(float collisionForce)
        {
            int damage = (int)collisionForce / 100 * damagePoints;
            if(ninjaEvents !=null)
                ninjaEvents.OnBombCollision(eve);

            //Debug.Log("Bomb damaged you with" + damage + "damage!\n");
        }

    }
}