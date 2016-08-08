using UnityEngine;
using System.Collections;
using Random= UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{

    public class FruitAndBombSpawner : MonoBehaviour
    {

        public float pausetime = 5;
        public int angle = 90;
        public float speed;
        public float SpawnerDistance = 0.5f;
        public Vector3 center;
        public Vector3 target;

        public MovingRigidbodyPhysics[] fruitsAndBombs;

        void Start()
        {
            speed = Random.Range(3f, 20.0f);
            StartCoroutine(FireDelay());
        }



        IEnumerator FireDelay()
        {
            // As long as the Game is playing, later conditional
            while (true)
            {
                StartCoroutine(FireFruit());
                yield return new WaitForSeconds(pausetime);
            }
        }



        IEnumerator FireFruit()
        {

            //Debug.Log("Fire from "+ myTransform.position);
            //choose randomly from fruit prefabs and instantiate canon
            MovingRigidbodyPhysics prefab = fruitsAndBombs[Random.Range(0, fruitsAndBombs.Length)];
            target = -transform.position;
            center = new Vector3(0, 2.0f, 0);
            transform.position = (transform.position - center).normalized * SpawnerDistance + center;
            float currentAngle = Random.Range(-angle / 2, angle / 2);
            transform.RotateAround(center, Vector3.up, currentAngle );
           // Debug.Log("TransformPosition:" + transform.position + " Target.Position " + target+ " from angle "+ currentAngle );
            prefab.distance = SpawnerDistance;
            prefab.speed = speed;
            prefab.startPoint = transform.position;
            // wait some small time
            yield return new WaitForSeconds(1.0f);
            if (prefab != null)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);

                //Debug.Log("Object " + prefab.transform.name + " instantiated");
            }

        }
    }
}
