
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Assets.LSL4Unity.Scripts;
using Assets.VREF.Scripts;

/// <summary>
/// Main class of the Paradigm-specific code, 
/// Includes Game controller logic and object spawning
/// </summary>
namespace Assets.NinjaGame.Scripts
{
    [RequireComponent(typeof(NinjaGameEventController))]
    public class NinjaGame : MonoBehaviour
    {
        public static ExperimentSceneController expController;
        [HideInInspector]
        public static string configDataDirectory;
        public static Config config;
        public static List<Trial> generatedTrials;
         
        //public float pausetime = 5;
        public Vector3 objectScale = new Vector3(0.5f,0.5f,0.5f);
        //public float velocity;
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

        public NinjaGameEventController ninjaControl;
        LSLMarkerStream experimentMarker;

        string expectedTrialsConfig;
        string saveTrialsConfig;

        //actual trial values
        private float scale;
        private float distance;
        private float velocity;

        //Experiment
        int angle;
        int parallelSpawns;
        float pausetimeTimingJitter;
        float pausetime=5;
        int trialNumber;
        public int trialsMax;
        void Awake()
        {
            experimentMarker = FindObjectsOfType(typeof(LSLMarkerStream)).FirstOrDefault() as LSLMarkerStream;
            if (experimentMarker != null)
                Debug.Log("Found LSL Stream"+experimentMarker.lslStreamName);   
            ExperimentSceneController.experimentInfo.triggerPressed = false;
#if (UNITY_EDITOR)
            configDataDirectory = Application.dataPath + "/NinjaGame/Config/";
#else
            configDataDirectory = Application.streamingAssetsPath + "/Config/";
#endif
            expectedTrialsConfig = configDataDirectory + "trialslist.json";
            saveTrialsConfig = configDataDirectory + "trialslist.json";

            if (config == null)
            {
                config = new Config();
                // trialsConfig = ScriptableObject.CreateInstance(typeof(TrialsList)) as TrialsList;
                //load default trials config

                config = ConfigUtil.LoadConfig<Config>(new FileInfo(expectedTrialsConfig), true, () =>
                {
                    Debug.LogError("Something is wrong with the AppConfig. Was not found and I was not able to create one!");
                });

                angle = config.experiment.maximumAngle;
                parallelSpawns = config.experiment.parallelSpawns;
                pausetimeTimingJitter = config.experiment.pausetimeTimingJitter;
                pausetime = config.experiment.pausetime;
                generatedTrials =config.GenerateTrialsList(config.listOfTrials);
                trialsMax = generatedTrials.Count;
                Debug.Log("Config from" + expectedTrialsConfig + "with " + generatedTrials.Count + " trials successfully loaded!");
            }
        }

        void Start()
        {

#region Experiment Event logic
   

            if (ExperimentSceneController.experimentInfo)
            {
                ExperimentSceneController.experimentInfo.health = startHealth;
                ExperimentSceneController.experimentInfo.totalscore = startScore;
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
            
            StartCoroutine(FireDelay());

       
        }
#region Game Event logic

        void fruitCollision(object sender, NinjaGameEventArgs eve)
        {
            ExperimentSceneController.experimentInfo.score = eve.score;
            ExperimentSceneController.experimentInfo.totalscore += ExperimentSceneController.experimentInfo.score;
            eve.totalscore = ExperimentSceneController.experimentInfo.totalscore;

            //eventMarker.Write("Event: Fruit Collision");

            Debug.LogWarning("score:" + eve.score);
            Debug.LogWarning("totalscore:" + eve.totalscore);
   

        }

        void bombCollision(object sender, NinjaGameEventArgs eve)
        {
            ExperimentSceneController.experimentInfo.damage = eve.damage;
            ExperimentSceneController.experimentInfo.health -= ExperimentSceneController.experimentInfo.damage;
            eve.health = ExperimentSceneController.experimentInfo.health;
            //eventMarker.Write("Event: Bomb Collision");
            Debug.LogWarning("health:" + eve.health);
           
        }


        void StartGame(object sender, NinjaGameEventArgs eve)
        {
            //eve.totalscore = startScore;
            //eve.health = startHealth;
            Debug.Log("Start Game");
            ExperimentSceneController.experimentInfo.triggerPressed = true;
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
                var totalPausetime = pausetime + Random.Range(-pausetimeTimingJitter / 2, pausetimeTimingJitter / 2);
                yield return new WaitForSeconds(totalPausetime);
            }
        }

        IEnumerator FireFruit()
        {
            if (config != null)
            {
                List<Transform> spawnerInstances = Enumerable.Repeat(transform, parallelSpawns-1).ToList();
                //Debug.Log(spawnerInstances.Count);
                var position = Vector3.one + Vector3.up * (height - 1);
                center = new Vector3(0, 2.0f, 0);
                foreach (var spawner in spawnerInstances)
                {

                    var selected = Trial.PickAndDelete(generatedTrials);
                    if (selected != null)
                    {

                        trialNumber = trialsMax - generatedTrials.Count;
                        Debug.Log("Trials:" + trialNumber + " " + selected.trial + ' ' + selected.color + ' ' + selected.distanceAvg);
                        var halfDistVar = selected.distanceVar / 2;
                        distance = selected.distanceAvg + Random.Range(-halfDistVar, halfDistVar);
                        spawner.position = (position - center).normalized * distance + center;
                        float currentAngle = Random.Range(-angle / 2, angle / 2) - angleAlignment;
                        var halfVelVar = selected.velocityVar / 2;
                        velocity = selected.velocityAvg + Random.Range(-halfVelVar,halfVelVar);

                        //Debug.Log("Transform position:" + spawner.position + "Angle:" +(currentAngle-angleAlignment));
                        spawner.RotateAround(center, Vector3.up, currentAngle);
                        var startposition = spawner.position;
                        target = new Vector3(-startposition.x, startposition.y, -startposition.z);
                        //Debug.Log("TransformPosition:" + startposition + " Target.Position " + target+ " from angle "+ currentAngle- angleAlignment );
                        // wait some small time
                        var halfScaleVar = selected.scaleVar / 2;
                        scale = selected.scaleAvg + Random.Range(-halfScaleVar, halfScaleVar);
                        prefab = Resources.Load("BasicPrefab", typeof(MovingRigidbodyPhysics)) as MovingRigidbodyPhysics;
                    
                        prefab.distance = distance;
                        prefab.velocity = velocity; //velocity;
                        prefab.startPoint = spawner.position;
                        prefab.color = selected.color;
                        prefab.transform.localScale = scale * Vector3.one;
                        Instantiate(prefab, spawner.position, Quaternion.identity);
                        prefab.name = trialNumber.ToString();
                        experimentMarker.Write("spawn_trial_" + trialNumber + ": name:" + selected.trial + ",color:" + selected.color + " ,distance:" + distance + ",velocity:" + velocity);

                        yield return new WaitForFixedUpdate();
                        //Debug.Log("Object " + prefab.transform.name + " instantiated");
                    }
                    else
                    {
                        Debug.LogError("Empty object");
                    }
                    
                }
            }
        }

#endregion

      
    }
}