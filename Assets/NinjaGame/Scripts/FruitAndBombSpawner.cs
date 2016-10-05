using UnityEngine;
using System.Collections;
using Random= UnityEngine.Random;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{

    public class FruitAndBombSpawner : MonoBehaviour
    {

        public float pausetime = 5;
        public int angle = 90;
        public float speed = 5.0f;
        public float SpawnerDistance = 0.5f;
        public Vector3 center;
        public Vector3 target;
        public float startHeight = 2.0f;
        private float angleAlignment = 45;
        public bool gamePlaying;
        public MovingRigidbodyPhysics[] fruitsAndBombs;
        public NinjaGameEventController ninjaGameEvent;
        public GUIContent guiContent;

        void Start()
        {
            gamePlaying = true;
            speed = Random.Range(3f, 20.0f);
            StartCoroutine(FireDelay());
        }



        IEnumerator FireDelay()
        {
            // As long as the Game is playing, later conditional
            while (gamePlaying)
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

            var position = Vector3.one + Vector3.up*(startHeight-1);
            center = new Vector3(0, 2.0f, 0);
            transform.position= (position - center).normalized * SpawnerDistance + center;
            float currentAngle = Random.Range(-angle / 2, angle / 2)-angleAlignment;

            Debug.Log("Transform position:" + transform.position + "Angle:" +(currentAngle-angleAlignment));
            transform.RotateAround(center, Vector3.up, currentAngle);
            var startposition = transform.position; 
            target = new Vector3(-startposition.x, startposition.y, -startposition.z); 
            //Debug.Log("TransformPosition:" + startposition + " Target.Position " + target+ " from angle "+ currentAngle- angleAlignment );
            // wait some small time
            yield return new WaitForSeconds(1.0f);
            if (prefab != null)
            {
                prefab.distance = SpawnerDistance;
                prefab.speed = speed;
                prefab.startPoint = transform.position;
                Instantiate(prefab, transform.position, Quaternion.identity);
                //Debug.Log("Object " + prefab.transform.name + " instantiated");
            }

        }
    }
}