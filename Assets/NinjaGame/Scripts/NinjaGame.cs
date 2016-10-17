using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using Random= UnityEngine.Random;
using System.Collections.Generic;

namespace Assets.NinjaGame.Scripts
{
    public class NinjaGame : MonoBehaviour
    {

        public float pausetime = 5;
        public int angle = 90;
        public int NumberOfSpawnerInstances = 1;
        public float velocityAvg = 5.0f;
        public float velocityRange = 1.0f;
        public float velocity;
        public float spawnerDistance=5.0f;
        public float spawnerRange = 1.0f;
        public float height = 2.0f;
        private float angleAlignment = 45;
        public bool gamePlaying;
        public Color32[]  objectColors = new Color32[] { Color.white, Color.green, Color.red};
        public bool particleEffects=false;
        public Vector3 center;
        public Vector3 target;
        public MovingRigidbodyPhysics[] fruitsAndBombs;
        public NinjaGameEventController ninjaGameEvent;
        public GUIContent guiContent;

  

        void Start()
        {
            gamePlaying = true;
            velocity = velocityAvg+ Random.Range(-velocityRange/2, velocityRange/2);
      
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
            var position = Vector3.one + Vector3.up*(height-1);
            center = new Vector3(0, 2.0f, 0);

            foreach ( var spawner in spawnerInstances)
            {   
                //choose randomly from fruit prefabs and instantiate canon
                prefab = fruitsAndBombs[Random.Range(0, fruitsAndBombs.Length)];
                var color = objectColors[Random.Range(0,objectColors.Length)];
                spawner.position= (position - center).normalized * (spawnerDistance+Random.Range(-spawnerRange/2,spawnerRange/2)) + center;
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
                    prefab.speed = velocity;
                    prefab.startPoint = spawner.position;
                    prefab.color = color;
                    Instantiate(prefab, spawner.position, Quaternion.identity);


                 }
                yield return new WaitForFixedUpdate();
                //Debug.Log("Object " + prefab.transform.name + " instantiated");
            }

        }


    }
}