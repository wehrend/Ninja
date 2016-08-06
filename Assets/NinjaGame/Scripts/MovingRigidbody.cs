using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{
    [Serializable]
    public class MovingRigidbody : MonoBehaviour {
        public MovingRigidbodyPhysics which_prefab;
        public Vector3 startPosition;
        public Vector3 startRotation;
        public float startTime;
        public AnimationCurve velocity;
    }
}