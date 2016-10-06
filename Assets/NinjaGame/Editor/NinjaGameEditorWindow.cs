    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
	using Assets.NinjaGame.Scripts;
   
//fixme: Namespaces

    public class NinjaGameEditorWindow
    {

        [MenuItem("Ninja Game / Configuration")]
        public static void Show()
        {
            var window = EditorWindow.GetWindow<DemoLevelListWindow>();

            window.Show();


        }

        internal static List<GuiContainerForScenes> GetLevelScenes()
        {
            var levelPath = new string[] { "Assets/NinjaGame/Levels" };
            var scenes = AssetDatabase.FindAssets("t:Scene", levelPath).Select(
                guid =>
                {

                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var name = Path.GetFileNameWithoutExtension(path);
                    return new GuiContainerForScenes()
                    {
                        Name = name,
                        Guid = guid,
                        Path = path,
                        guiContent = new GUIContent(name)
                    };

                }).ToList();

            return scenes;
        }

    internal static List<FruitAndBombSpawner> FindFruitAndBombSpawnerInstances()
    {
      

        FruitAndBombSpawner[] allObjects = GameObject.FindObjectsOfType(typeof(FruitAndBombSpawner)) as FruitAndBombSpawner[];
        List<FruitAndBombSpawner> result = new List<FruitAndBombSpawner>();
        foreach (var go in allObjects)
        {
            Debug.Log(go);
            result.Add(go);
        }

        return result;
    }


    public class DemoLevelListWindow : EditorWindow
        {
            List<GuiContainerForScenes> levelScenes;
            //List<GuiContainerForPrefabs> spawnerPrefabs;
            GUIContent[] listToDisplay;
            GUIContent[] spawnersToDisplay;
            string selectedScene;

            public const string HelpBoxText = "Ninja Game Configuration Window, works only in play mode:\n"
                                           + "Select a Level and configurate speed etc. to your own needs.\n"
                                           + "If you choose optional settings you can change fixed parameters.";
            public const string AdvancedModeText = "Set angle range, quantity and distant range.\n"
                                                 +"Caution: Changes here will override the values of loaded Scene \n";
                                         


            int selectedLevelIndex = 0; //default
            int selectedSpawnerIndex = 0;
            float minSpeedValue = 1.0f;
            float maxSpeedValue = 10.0f;
            float minSpeedLimit = 0.1f;
            float maxSpeedLimit = 20.0f;
            bool advancedMode = false;
            // currently mockup stadium
            float minAngleValue = 45f;
            float maxAngleValue = 160f;
            float minAngleLimit = 0f;
	        float maxAngleLimit = 360f;

            int minQuantityLimit = 1;
            int maxQuantityLimit = 10;

            float minDistanceValue = 2.5f;
	        float maxDistanceValue = 15f;
	        float minDistanceLimit = 1f;
	        float maxDistanceLimit = 50f;

        void OnEnable()
        {
	        levelScenes = GetLevelScenes();
		        
            listToDisplay = levelScenes.Select(s => s.guiContent).ToArray();
        }

        void OnGUI()
        {
	        if (Application.isPlaying )
	        {
	            var spawnerPrefabs = FindFruitAndBombSpawnerInstances(); //GetSpawnerPrefabsOfScene(selectedScene);
	            Debug.Log(spawnerPrefabs.Count());
		        //spawnersToDisplay = new GUIContent("test");
	            // Debug.Log("SpawnerPrefabs count:"+spawnerPrefabs.Count());
	            
		        EditorGUILayout.HelpBox(HelpBoxText, MessageType.Info);
		        float maxFloatWidth = GUI.skin.textField.CalcSize(new GUIContent(10.23f.ToString(CultureInfo.InvariantCulture))).x;
                float maxIntWidth = GUI.skin.textField.CalcSize(new GUIContent(100.ToString(CultureInfo.InvariantCulture))).x;
                selectedLevelIndex = EditorGUILayout.Popup(new GUIContent("Available Scenes"), selectedLevelIndex, listToDisplay);
		        selectedScene = levelScenes[selectedLevelIndex].Name;
		        
		        EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Velocity " + minSpeedValue.ToString("0.0") + " m/s - " + maxSpeedValue.ToString("0.0") + "m/s");
		        EditorGUILayout.LabelField(minSpeedLimit.ToString(), GUILayout.MaxWidth(maxFloatWidth));
		        EditorGUILayout.MinMaxSlider( ref minSpeedValue, ref maxSpeedValue, minSpeedLimit, maxSpeedLimit);
		        EditorGUILayout.LabelField(maxSpeedLimit.ToString(), GUILayout.MaxWidth(maxFloatWidth));  
		        EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
		        advancedMode=EditorGUILayout.BeginToggleGroup("Overwrite loaded scene with custom Values ",advancedMode);
		        
		                EditorGUILayout.HelpBox(AdvancedModeText, MessageType.Error);
		                EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.PrefixLabel("Angle "+minAngleValue.ToString("0")+ " °- " + maxAngleValue.ToString("000")+ "°");
                        EditorGUILayout.LabelField(minAngleLimit.ToString()+"°",GUILayout.MaxWidth(maxIntWidth));
		                EditorGUILayout.MinMaxSlider( ref minAngleValue, ref maxAngleValue, minAngleLimit, maxAngleLimit);
		                EditorGUILayout.LabelField(maxAngleLimit.ToString()+"°",GUILayout.MaxWidth(maxIntWidth));
		                EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Quantity ");
                        EditorGUILayout.LabelField(minQuantityLimit.ToString(), GUILayout.MaxWidth(maxIntWidth));
                        EditorGUILayout.IntSlider( 2, minQuantityLimit, maxQuantityLimit);
                        EditorGUILayout.LabelField(maxQuantityLimit.ToString(), GUILayout.MaxWidth(maxIntWidth));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Distance " + minDistanceValue.ToString("0.00") + " m - " + maxDistanceValue.ToString("0.00") + "m");
		                EditorGUILayout.LabelField(minDistanceLimit.ToString(),GUILayout.MaxWidth(maxIntWidth));
		                EditorGUILayout.MinMaxSlider(ref minDistanceValue, ref maxDistanceValue, minDistanceLimit, maxDistanceLimit);   
		                EditorGUILayout.LabelField(maxDistanceLimit.ToString(),GUILayout.MaxWidth(maxIntWidth));
		                EditorGUILayout.EndHorizontal();

                        if (GUI.Button(new Rect(5, position.height - 35, 120, 30), new GUIContent("Save Level Config")))
                            {
                                var savePath = Application.dataPath + "Assets/NinjaGame/Levels";
                                //Debug.Log("Saved scene as");
                                //EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),savePath, true);
                            }
         EditorGUILayout.EndToggleGroup();

		        if (selectedScene!=null)
                {
			                if (GUI.Button(new Rect(position.width - 125, position.height - 35, 120, 30), new GUIContent("Load Level"))
				                && SceneManager.GetActiveScene().name != selectedScene)
			                        {
			                            Debug.Log("Loaded level:" + selectedScene);
			                            SceneManager.LoadScene(selectedScene);
			                        }
		        	    }
	               
	           } else {
	        	
	        	EditorGUILayout.HelpBox("Ninja Game not running, please press play button to configure Ninja Game.\n", MessageType.Error);
	          }
	       }
        }

        internal class GuiContainerForScenes
        {
            public string Name;
            public string Guid;
            public string Path;
            public GUIContent guiContent;
        }

        internal class GuiContainerForPrefab : GuiContainerForScenes
        {   }

    }
