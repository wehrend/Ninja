﻿using UnityEngine;
using System.Collections;
//using UnityStandardAssets.Effects;
using VRTK;

namespace Assets.NinjaGame.Scripts
{

    public class Bomb : MovingRigidbodyPhysics
    {

        //let it be one of a mild bomb
        public int damagePoints = 5;
        public float explosionMultiplier = 0.3f;

        public override void CollisionWithForce(float collisionForce)
        {
            int damage = (int)collisionForce / 100 * damagePoints;
            Destroy(Body.gameObject, 0.5f);
            if (gameController)
                gameController.issueDamage(damage);

            Debug.Log("Bomb damaged you with" + damage + "damage!\n");
        }

    }
}