
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
/// <summary>
/// Main class of the Paradigm-specific code, 
/// Includes Game controller logic and object spawning
/// </summary>
namespace Assets.NinjaGame.Scripts
{
    [RequireComponent(typeof(NinjaGameEventController))]
    public class NinjaGame : MonoBehaviour
    {
       
        List<Probability> objectPool=new List<Probability>();
        public int fruitsProbability=50;
        public int bombsProbability=50;
        public int gapsProbability;
         
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
        public MovingRigidbodyPhysics prefab;
        //choose some static color 
        public List<Probability> Colors = new List<Probability>(); //{ { Color.white, 70 }, { Color.green, 15 }, { Color.red, 15 } };
        public Vector3 center;
        public Vector3 target;
        //public NinjaGameEventController ninjaGameEvent;
        public GUIContent guiContent;

        public int startHealth = 1000;
        public int startScore = 0;
        //Next todo: using singletones here 
        public static GameInfo scores;
        public NinjaGameEventController ninjaControl;


        void Start()
        {
            scores = new GameInfo();
            #region Game Event logic
            scores.health = startHealth;
            scores.totalscore = startScore;

            if (ninjaControl == null)
            {
                Debug.LogError("The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            ninjaControl.FruitCollision += new NinjaGameEventHandler(fruitCollision);
            ninjaControl.BombCollision += new NinjaGameEventHandler(bombCollision);
            ninjaControl.StartGame += new NinjaGameEventHandler(StartGame);
            ninjaControl.GameOver += new NinjaGameEventHandler(GameOver);

            #endregion 

            Colors.Add(new Probability(Color.white, 60));
            Colors.Add(new Probability(Color.red, 20));
            Colors.Add(new Probability(Color.green, 20));
            objectPool.Add(new Probability("SomeFruit", fruitsProbability));
            objectPool.Add(new Probability("BombWithEffect", bombsProbability));
            gapsProbability = 100 - (fruitsProbability + bombsProbability);
            //Debug.LogWarning(objectPool);
            gamePlaying = true;
            
           // prefabObjects = 
            velocity = velocityAvg+ Random.Range(-velocityRange/2, velocityRange/2);
            
            StartCoroutine(FireDelay());
        }
        #region Game Event logic

        void fruitCollision(object sender, NinjaGameEventArgs eve)
        {
            scores.score = eve.score;
            scores.totalscore += scores.score;
            eve.totalscore = scores.totalscore;
            
            Debug.Log("Event: FruitCollision");
            Debug.LogWarning("score:" + eve.score);
            Debug.LogWarning("totalscore:" + eve.totalscore);
   

        }

        void bombCollision(object sender, NinjaGameEventArgs eve)
        {
            scores.damage = eve.damage;
            scores.health -= scores.damage;
            eve.health = scores.health;
            Debug.LogWarning("health:" + eve.health);
            Debug.Log("Event: BombCollision");
        }


        void StartGame(object sender, NinjaGameEventArgs eve)
        {
            //eve.totalscore = startScore;
            //eve.health = startHealth;

            Debug.Log("Start Game");
        }

        void GameOver(object sender, NinjaGameEventArgs eve)
        {
            eve.health = 0;
            Debug.Log("GameOver");
            //light.enabled = false;
        }

        #endregion

        #region ObjectSpawner
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
            List<Transform> spawnerInstances = Enumerable.Repeat(transform, NumberOfSpawnerInstances).ToList();
            var position = Vector3.one + Vector3.up*(height-1);
            center = new Vector3(0, 2.0f, 0);

            foreach ( var spawner in spawnerInstances)
            {
                //choose randomly from fruit prefabs and instantiate canon
                string prefabname = RandomWithProbability(objectPool).ToString();

                var color = RandomWithProbability(Colors);
                spawner.position= (position - center).normalized * (spawnerDistance+Random.Range(-spawnerRange/2,spawnerRange/2)) + center;
                float currentAngle = Random.Range(-angle / 2, angle / 2)-angleAlignment;
                //Debug.Log("Transform position:" + spawner.position + "Angle:" +(currentAngle-angleAlignment));
                spawner.RotateAround(center, Vector3.up, currentAngle);
                var startposition = spawner.position; 
                target = new Vector3(-startposition.x, startposition.y, -startposition.z); 
                //Debug.Log("TransformPosition:" + startposition + " Target.Position " + target+ " from angle "+ currentAngle- angleAlignment );
                // wait some small time
                if(prefabname !="None")
                    prefab = Resources.Load(prefabname, typeof(MovingRigidbodyPhysics)) as MovingRigidbodyPhysics;
                if (prefab == null)
                    Debug.LogWarning("Prefab '"+prefabname+"' not found in Resources!");
                else
                {
                    Debug.Log(prefab.name);
                    prefab.distance = spawnerDistance;
                    prefab.velocity = velocity;
                    prefab.startPoint = spawner.position;
                    prefab.color = (Color) color;

                    Instantiate(prefab, spawner.position, Quaternion.identity);

                }

                yield return new WaitForFixedUpdate();
                //Debug.Log("Object " + prefab.transform.name + " instantiated");
            }

        }


        void Update()
        {
            //it may be dynamic in later scenarios
            gapsProbability = 100 - (fruitsProbability + bombsProbability);

        }

        object RandomWithProbability( List<Probability> objectPool)
        {
            int randomValue = (int) Random.Range(0,100);
   
            int cumulative = 0;
            object selected = "None";

            for (int i = 0; i < objectPool.Count; i++)
            {

                cumulative += objectPool[i].m_probability;
                if (randomValue < cumulative)
                {
                    selected = objectPool[i].m_object;
                    break;
                }
            }
            return selected;
        }

        [Serializable]
        public class Probability
        {
            public object m_object;
            public int m_probability;

            public Probability(object _object, int probability)
            {
                m_object = _object;
                m_probability = probability;
            }
        }
        #endregion

        [Serializable]
        public class GameInfo : ScriptableObject
        {
            public int score;
            public int totalscore;
            public int damage;
            public int health;
        }
    }
}