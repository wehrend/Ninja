using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using Random= UnityEngine.Random;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{

    public class FruitAndBombSpawner : MonoBehaviour
    {

        public float pausetime = 5;
        public int angle = 90;
        public int NumberOfSpawnerInstances = 1;
        public float speed = 5.0f;
        public float spawnerDistance = 0.5f;
        public Vector3 center;
        public Vector3 target;
        public float startHeight = 2.0f;
        private float angleAlignment = 45;
        public bool gamePlaying;
        public MovingRigidbodyPhysics[] fruitsAndBombs;
        public NinjaGameEventController ninjaGameEvent;
        public GUIContent guiContent;

        void OnSceneGUI()
        {
            DebugExtension.DrawCircle(Vector3.zero, Vector3.up, Color.green, spawnerDistance);
        }

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
            MovingRigidbodyPhysics prefab; 
            List<Transform> spawnerInstances = Enumerable.Repeat(transform, NumberOfSpawnerInstances).ToList();
            var position = Vector3.one + Vector3.up*(startHeight-1);
            center = new Vector3(0, 2.0f, 0);

            foreach ( var spawner in spawnerInstances)
            {   
                //choose randomly from fruit prefabs and instantiate canon
                prefab = fruitsAndBombs[Random.Range(0, fruitsAndBombs.Length)];
                spawner.position= (position - center).normalized * (spawnerDistance+Random.Range(-1,1)) + center;
                float currentAngle = Random.Range(-angle / 2, angle / 2)-angleAlignment;

                Debug.Log("Transform position:" + spawner.position + "Angle:" +(currentAngle-angleAlignment));
                spawner.RotateAround(center, Vector3.up, currentAngle);
                var startposition = spawner.position; 
                target = new Vector3(-startposition.x, startposition.y, -startposition.z); 
                //Debug.Log("TransformPosition:" + startposition + " Target.Position " + target+ " from angle "+ currentAngle- angleAlignment );
                // wait some small time
                if (prefab != null)
                {
                    prefab.distance = spawnerDistance;
                    prefab.speed = speed;
                    prefab.startPoint = spawner.position;
                    Instantiate(prefab, spawner.position, Quaternion.identity);


                 }
                yield return new WaitForFixedUpdate();
                //Debug.Log("Object " + prefab.transform.name + " instantiated");
            }

        }


    }
}