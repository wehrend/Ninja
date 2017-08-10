using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.MobiSA.Scripts
{
    public class CollisionEvent : MonoBehaviour
    {

        public UnityEvent onBombCollision;
        public UnityEvent onFruitCollision;

        private void OnBombCollision()
        {
            onBombCollision.Invoke();
        }

        private void OnFruitCollision()
        {
            onFruitCollision.Invoke();
        }

    }
}