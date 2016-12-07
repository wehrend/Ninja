
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Assets.LSL4Unity.Scripts;

/// <summary>
/// Main class of the Paradigm-specific code, 
/// Includes Game controller logic and object spawning
/// </summary>
namespace Assets.NinjaGame.Scripts
{
    [RequireComponent(typeof(NinjaGameEventController))]
    public class NinjaGame : MonoBehaviour
    {

        public List<Trial> trialsList;
         
        public float pausetime = 5;
        public Vector3 objectScale = new Vector3(0.5f,0.5f,0.5f);
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

        public Vector3 center;
        public Vector3 target;
        //public NinjaGameEventController ninjaGameEvent;
        public GUIContent guiContent;

        public int startHealth = 1000;
        public int startScore = 0;
        //Next todo: using singletones here 
        public static GameInfo game;
        public NinjaGameEventController ninjaControl;
        //private LSLMarkerStream eventMarker;

        void Start()
        {
            
            #region Game Event logic
            game = new GameInfo();
            if (game)
            {
                game.health = startHealth;
                game.totalscore = startScore;
            }
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
            //Debug.LogWarning(objectPool);
            gamePlaying = true;
            
           // prefabObjects = 
            velocity = velocityAvg+ Random.Range(-velocityRange/2, velocityRange/2);
            StartCoroutine(FireDelay());

       
        }
        #region Game Event logic

        void fruitCollision(object sender, NinjaGameEventArgs eve)
        {
            game.score = eve.score;
            game.totalscore += game.score;
            eve.totalscore = game.totalscore;

            //eventMarker.Write("Event: Fruit Collision");

            Debug.LogWarning("score:" + eve.score);
            Debug.LogWarning("totalscore:" + eve.totalscore);
   

        }

        void bombCollision(object sender, NinjaGameEventArgs eve)
        {
            game.damage = eve.damage;
            game.health -= game.damage;
            eve.health = game.health;
            //eventMarker.Write("Event: Bomb Collision");
            Debug.LogWarning("health:" + eve.health);
           
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
                if (game.trialsList != null)
                {
                   
                    var selected = Trial.PickAndDelete(game.trialsList);
                    Debug.Log("Trials-Countdown:" + game.trialsList.Count+" "+selected.trial + ' ' + selected.color + ' ' + selected.distance);
                    spawner.position = (position - center).normalized * selected.distance + center;
                    float currentAngle = Random.Range(-game.maximumAngle / 2, game.maximumAngle / 2) - angleAlignment;
                    //Debug.Log("Transform position:" + spawner.position + "Angle:" +(currentAngle-angleAlignment));
                    spawner.RotateAround(center, Vector3.up, currentAngle);
                    var startposition = spawner.position;
                    target = new Vector3(-startposition.x, startposition.y, -startposition.z);
                    //Debug.Log("TransformPosition:" + startposition + " Target.Position " + target+ " from angle "+ currentAngle- angleAlignment );
                    // wait some small time
                    prefab = Resources.Load("BasicPrefab", typeof(MovingRigidbodyPhysics)) as MovingRigidbodyPhysics;
                    prefab.distance = selected.distance;
                    prefab.velocity = selected.velocity; //velocity;
                    prefab.startPoint = spawner.position;
                    prefab.color = selected.color;
                    prefab.transform.localScale = selected.scale * Vector3.one;
                    Instantiate(prefab, spawner.position, Quaternion.identity);
                }
                yield return new WaitForFixedUpdate();
                //Debug.Log("Object " + prefab.transform.name + " instantiated");
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
            public List<Trial> trialsList;
            public float maximumAngle;

            public void setMaximumAngle(int angle)
            {
                game.maximumAngle= angle;
            }
            public void setListOfTrials(List<Trial> trials)
            {
                game.trialsList = trials;
            }
        }
    }
}