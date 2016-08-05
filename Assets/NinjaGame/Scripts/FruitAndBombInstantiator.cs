using UnityEngine;
using System.Collections;
using Random= UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{

    public class FruitAndBombInstantiator : MonoBehaviour
    {

        public float pausetime = 5;
        public int angle = 90;
        public float distance = 5.0f;
        public Transform target;

        public MovingRigidbodyPhysics[] fruitsAndBombs;
        private Fruit[] activeFruits;
        //public Vector3 startPosition = new Vector3(-0.5f,0.5f,-0.5f);
        //public Vector3 endPosition = new Vector3(-0.1f,2f,-0.1f);


        void Start()
        {

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
            prefab.target = target;
            List<float> used_x = new List<float>();
            float x_max = 2.0f * Mathf.Sin(Mathf.Deg2Rad * (angle / 2)) + 1.5f; //1.5 offset of the vive-cube
            float x = Random.Range(-x_max, x_max);
            while (used_x.Contains(x))
            {
                x = Random.Range(-x_max, x_max);
                used_x.Add(x);
            }

            float z = Random.Range(8f, 10f);
            transform.position = (transform.position - target.position).normalized * distance + target.position;
            //Vector3 position =  new Vector3(x, 2f, z);
            transform.RotateAround(target.position, Vector3.up, Random.Range(5, 20));

            // wait some small time
            yield return new WaitForSeconds(1.0f);
            if (prefab != null)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
                Debug.Log("Object " + prefab.transform.name + " instantiated");
            }
        }
    }
}
