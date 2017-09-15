
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
namespace Assets.MobiSA.Scripts
{
    [RequireComponent(typeof(MobiSACoreEventController))]
    public class MobiSACore : MonoBehaviour
    {
        public static ExperimentSceneController expController;
        public static WallText wallText;
        [HideInInspector]
        public static string configDataDirectory;
        public static Config config;
        private Config val;
        private Experiment experiment;
        private Block curBlock;
        public static List<Trial> generatedTrials;
       
        //public float pausetime = 5;
        public Vector3 objectScale = new Vector3(0.5f,0.5f,0.5f);
        //public float velocity;
        public float spawnerDistance=5.0f;
        public float spawnerRange = 1.0f;
        public float distractorDestroyDistance = 1.0f;
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

        public MobiSACoreEventController ninjaControl;
        LSLMarkerStream experimentMarker;

        string jsonConfig;
        string saveJsonConfig;
        
        //actual trial values
        private float scale;
        private float distance;
        private float velocity;

        int trialNumber;




        void Awake()
        {
            wallText = FindObjectsOfType(typeof(WallText)).FirstOrDefault() as WallText;
            if (wallText!= null)
                Debug.Log("Found wallText " + wallText.ToString());

            experimentMarker = FindObjectsOfType(typeof(LSLMarkerStream)).FirstOrDefault() as LSLMarkerStream;
            if (experimentMarker != null)
                Debug.Log("Found LSL Stream"+experimentMarker.lslStreamName);


            expController = FindObjectOfType(typeof(ExperimentSceneController)) as ExperimentSceneController;
            if (expController) //Get gloabl config if we are in multi scenes
            {
                val = expController.configAsset;
                curBlock = expController.blockEnum.Current;

                //Debug.Log("Loaded ExpController Config" + val.ToString());
                experiment = val.experiment;
                distractorDestroyDistance = experiment.distractorDestroyDistance;

            // }else {
            //  val=LoadConfig(); //local load config function for single scene
            //Debug.Log("Loaded Ninja Config" + val.ToString());

            }
            Debug.Log("BLOCK " +  curBlock.name + " with " + curBlock.generatedTrials.Count+" Trials/Objects");

            prefab = Resources.Load("BasicPrefab", typeof(MovingRigidbodyPhysics)) as MovingRigidbodyPhysics;
            if (prefab == null)
                Debug.LogError("Coudn't load BasicPrefab");
        }


        public Config LoadConfig()
        {
            Config configVal = new Config();
#if (UNITY_EDITOR)
            configDataDirectory = Application.dataPath + "/MobiSA/Config/";
#else
            configDataDirectory = Application.streamingAssetsPath + "/Config/";
#endif
            jsonConfig = configDataDirectory + "DefaultConfig";
            saveJsonConfig = configDataDirectory + "DefaultConfig";

            if (config == null)
            {
                config = new Config();
                // trialsConfig = ScriptableObject.CreateInstance(typeof(TrialsList)) as TrialsList;
                //load default trials config


               
                config = ConfigUtil.LoadConfig<Config>(new FileInfo(jsonConfig), false, () =>
                {
                    Debug.LogError("Something is wrong with the AppConfig. Was not found and I was not able to create one!");
                });
                //check if fields exists aka config scheme is new enough

              
                    ///instructions
                    if (wallText != null)
                    {
                        Debug.LogWarning("Loaded instructionlists");
                        wallText.instructionlists = config.instructionlists;
                    }

                foreach (Block b in config.blocks)
                {
                    b.generatedTrials = b.GenerateTrialsList(b.listOfTrials);
                    /*foreach (Trial t in b.generatedTrials)
                    {
                        Debug.LogWarning(t.trial + " " + t.color);
                    }*/

                    var trialsMax = b.generatedTrials.Count;
                    Debug.Log("Config from" + jsonConfig + "with " + b.generatedTrials.Count + " trials successfully loaded!");
                    b.trialsMax = trialsMax;
                }
                //configVal.blocks = config.listOfBlocks;
            }
            return configVal;
        }

        void Start()
        {
#region Experiment Event logic


            if (ExperimentSceneController.experimentInfo)
            {
                ExperimentSceneController.experimentInfo.health = startHealth;
                ExperimentSceneController.experimentInfo.totalscore = startScore;
            }

            ninjaControl = gameObject.GetComponent<MobiSACoreEventController>();
            if (ninjaControl == null)
            {
                Debug.LogError("[" + this.GetType().Name + "] The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            ninjaControl.FruitCollision += new MobiSACoreEventHandler(fruitCollision);
            ninjaControl.BombCollision += new MobiSACoreEventHandler(bombCollision);
            ninjaControl.StartGame += new MobiSACoreEventHandler(StartGame);
            ninjaControl.GameOver += new MobiSACoreEventHandler(GameOver);

#endregion
            //Debug.LogWarning(objectPool);
            gamePlaying = true;
            
            StartCoroutine(FireDelay());

       
        }
#region Game Event logic

        void fruitCollision(object sender, MobiSACoreEventArgs eve)
        {
            ExperimentSceneController.experimentInfo.score = eve.score;
            ExperimentSceneController.experimentInfo.totalscore += ExperimentSceneController.experimentInfo.score;
            eve.totalscore = ExperimentSceneController.experimentInfo.totalscore;

            //eventMarker.Write("Event: Fruit Collision");

            Debug.LogWarning("score:" + eve.score);
            Debug.LogWarning("totalscore:" + eve.totalscore);
   

        }

        void bombCollision(object sender, MobiSACoreEventArgs eve)
        {
            ExperimentSceneController.experimentInfo.damage = eve.damage;
            ExperimentSceneController.experimentInfo.health -= ExperimentSceneController.experimentInfo.damage;
            eve.health = ExperimentSceneController.experimentInfo.health;
            //eventMarker.Write("Event: Bomb Collision");
            Debug.LogWarning("health:" + eve.health);
           
        }


        void StartGame(object sender, MobiSACoreEventArgs eve)
        {
            //eve.totalscore = startScore;
            //eve.health = startHealth;
            Debug.Log("Start Game");
            ExperimentSceneController.experimentInfo.triggerPressed = true;
        }

        void GameOver(object sender, MobiSACoreEventArgs eve)
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
                var totalPausetime = experiment.pausetime + Random.Range(-experiment.pausetimeTimingJitter / 2, experiment.pausetimeTimingJitter / 2);
                yield return new WaitForSeconds(totalPausetime);
            }
        }

        IEnumerator FireFruit()
        {
            if (experiment != null)
            {
                //Debug.Log("parallelspawns "+experiment.parallelSpawns);
                List<Transform> spawnerInstances = Enumerable.Repeat(transform, experiment.parallelSpawns).ToList();
                //Debug.Log(spawnerInstances.Count);

                foreach (var spawner in spawnerInstances)
                {

                    var selected = Trial.PickAndDelete(curBlock);
                    Debug.LogWarning(coloredLogString(selected.trial, selected.color));
                    if (selected != null)
                    {
                        var position = Vector3.one + Vector3.up * (selected.heigth - 1);
                        center = new Vector3(0, selected.heigth, 0);
                        trialNumber = expController.blockEnum.Current.trialsMax - expController.blockEnum.Current.generatedTrials.Count;
#if UNITY_EDITOR
                        Debug.Log("Trials:" + trialNumber + " " + selected.trial + ' ' + selected.color + ' ' + selected.distanceAvg);
#else
                        Debug.LogError("Trials:" + trialNumber + " " + selected.trial + ' ' + selected.color + ' ' + selected.distanceAvg);
#endif                        
                        var halfDistVar = selected.distanceVar / 2;
                        distance = selected.distanceAvg + Random.Range(-halfDistVar, halfDistVar);
                        spawner.position = (position - center).normalized * distance + center;
                        float currentAngle = Random.Range(-experiment.maximumAngle / 2, experiment.maximumAngle / 2) - angleAlignment;
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
                        prefab.distance = distance;
                        prefab.velocity = velocity; //velocity;
                        prefab.startPoint = spawner.position;
                        prefab.color = selected.color;
                        // prefab.hoverHeight = selected.heigth;
                        prefab.transform.localScale = scale * Vector3.one;
                        prefab.name = trialNumber.ToString();
                        prefab.type = selected.trial;
                        // now, after all assigns, we can spawn! 
                        Instantiate(prefab, spawner.position, Quaternion.identity);

                        if (experimentMarker != null)
                        {
                            experimentMarker.Write("spawn_trial_" + trialNumber + ": name:" + selected.trial + ",color:" + selected.color + ", spawn point:" + spawner.position + ",velocity:" + velocity);
                        }
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


        void DestroyNarrowDistracts() {

        }

#endregion
        //Utils
        String coloredLogString(String str,Color c)
        {
           return string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
               (byte)(c.r * 255f), (byte)(c.g * 255f), (byte)(c.b * 255f), str);
        }


      
    }
}