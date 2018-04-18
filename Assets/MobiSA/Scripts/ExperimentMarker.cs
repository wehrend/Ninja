using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.LSL4Unity.Scripts;
using System.Reflection;
using System.Text;

namespace Assets.MobiSA.Scripts
{

    public class ExperimentMarker
    {
        public const string start = "start";
        public const string end = "end";

        public const string touchCondition = "touch";
        public const string spawnCondition = "spawn";

        public const string experimentCondition = "experiment";
        public const string baselineCondition = "baseline";
        public const string blockCondition = "block";
        public const string pauseCondition = "pause";

        public LSLMarkerStream markerStream;
        public Experiment experiment;

        public ExperimentMarker(LSLMarkerStream markerStream, Experiment experiment)
        {
            this.markerStream = markerStream;
            this.experiment = experiment;
        }

        public void StartExperiment()
        {
            StartMarker(experimentCondition);
            InfoMarker(experiment);
        }

        public void Touch(GameObject touchedObject)
        {
            GameObjectMarker(touchCondition, touchedObject);
        }

        public void Spawn(GameObject spawnedObject)
        {
            GameObjectMarker(spawnCondition, spawnedObject);
        }

        public void PlaySound()
        {
            markerStream.Write("play_sound");
        }

        public void NewGazedObject(GameObject gazedObject)
        {
            string markerString;
            var condition = "newgazedobject";
            var _object = gazedObject.GetComponent(typeof(Object)) as Object;
            if (_object != null)
            {
                Color color = _object.color;
                Vector3 spawnPointVector = _object.startPoint;
                var formattedColor = string.Format("[{0:0.0#},{1:0.0#},{2:0.0#},{3:0.0#}]", color.r, color.g, color.b, color.a);
                var spawnPoint = string.Format("[{0:0.00#},{1:0.00#},{2:0.00#}]", spawnPointVector.x, spawnPointVector.y, spawnPointVector.z);

                 markerString = string.Format("{0}, type:{1}, color {2}, speed:{3}, spawnPoint: {4}, instance#: {5}", condition, _object.type, formattedColor, _object.velocity, spawnPoint, _object.name);
            }
            else {

                markerString = string.Format("{0}, type:{1}", condition, "other");
            }

            Debug.Log(markerString);
            markerStream.Write(markerString);
        }

        public void EndExperiment()
        {
            EndMarker(experimentCondition);
        }

        public void StartBaseline()
        {
            StartMarker(baselineCondition);
        }

        public void EndBaseline()
        {
            EndMarker(baselineCondition);
        }

        public void StartBlock(Block currentBlock)
        {
            StartMarker(blockCondition);
            InfoMarker(currentBlock);
        }

        public void EndBlock()
        {
            EndMarker(blockCondition);
        }

        public void StartPause()
        {
            StartMarker(pauseCondition);
        }

        public void EndPause()
        {
            EndMarker(pauseCondition);
        }

        public void InfoMarker(object configClassOrBlock)
        {
            if (configClassOrBlock != null)
            {
                //Get field info by reflection to simply iterate over all
                //and append to strings
                StringBuilder builder = new StringBuilder();
                FieldInfo[] fields = configClassOrBlock.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var fieldInfo in fields)
                {
                    builder.Append(string.Format("{0}:{1}", fieldInfo.Name, fieldInfo.GetValue(configClassOrBlock)));
                    builder.Append("\n"); //newline
                }
                markerStream.Write(builder.ToString());
                Debug.Log(builder.ToString());
            }
            else
            {
                Debug.LogError("config class or block is null! InfoMarker could not be written!");
            }
        }


        public void GameObjectMarker(string condition, GameObject gameobject)
        {
            var _object = gameobject.GetComponent(typeof(Object)) as Object;

            Color color = _object.color;
            Vector3 spawnPointVector = _object.startPoint;
            var formattedColor = string.Format("[{0:0.0#},{1:0.0#},{2:0.0#},{3:0.0#}]", color.r, color.g, color.b, color.a);
            var spawnPoint = string.Format("[{0:0.00#},{1:0.00#},{2:0.00#}]", spawnPointVector.x, spawnPointVector.y, spawnPointVector.z);
            string markerString = string.Format("{0}, type:{1}, color {2}, speed:{3}, spawnPoint: {4}, instance#: {5}", condition, _object.type, formattedColor, _object.velocity, spawnPoint, _object.name);

            Debug.Log(markerString);
            if (condition.Equals(spawnCondition))
                markerStream.WriteBeforeFrameIsDisplayed(markerString);
            else
                markerStream.Write(markerString);

        }


        public void StartMarker(string condition)
        {
            var startMarker = condition + ":" + start;
            markerStream.Write(startMarker);
            Debug.Log(startMarker);
        }

        public void EndMarker(string condition)
        {
            var endMarker = condition + ":" + end;
            markerStream.Write(endMarker);
            Debug.Log(endMarker);
        }
    }



}