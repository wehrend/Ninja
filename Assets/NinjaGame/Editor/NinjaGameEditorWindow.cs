    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEditor;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
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
            public const string AdvancedModeText = "Set angle and distance\n Caution: This is not implemeneted yet due to design reasons\n";
                                         


            int selectedLevelIndex = 0; //default
            int selectedSpawnerIndex = 0;
            float minSpeedValue = 1.0f;
            float maxSpeedValue = 10.0f;
            float minSpeedLimit = 0.01f;
            float maxSpeedLimit = 20.0f;
            bool advancedMode = false;
            // currently mockup stadium
            float minAngleValue = 45f;
            float maxAngleValue = 160f;
            float minAngleLimit = 0f;
            float maxAngleLimit = 360f;

        void OnEnable()
        {
            levelScenes = GetLevelScenes();
            listToDisplay = levelScenes.Select(s => s.guiContent).ToArray();

        }

        void OnGUI()
        {

            var spawnerPrefabs = FindFruitAndBombSpawnerInstances(); //GetSpawnerPrefabsOfScene(selectedScene);
            Debug.Log(spawnerPrefabs.Count());
            spawnersToDisplay = new GUIContent("test");
            // Debug.Log("SpawnerPrefabs count:"+spawnerPrefabs.Count());
            
            EditorGUILayout.HelpBox(HelpBoxText, MessageType.Info);
            selectedLevelIndex = EditorGUILayout.Popup(new GUIContent("Available Scenes"), selectedLevelIndex, listToDisplay);
            selectedScene = levelScenes[selectedLevelIndex].Name;
            //EditorGUILayout.MinMaxSlider(new GUIContent("Speed"), ref minSpeedValue, ref maxSpeedValue, minSpeedLimit, maxSpeedLimit);
            advancedMode=EditorGUILayout.BeginToggleGroup("Change fixed distance and angle",advancedMode);
                selectedSpawnerIndex=EditorGUILayout.Popup(new GUIContent("Select Spawner"), selectedSpawnerIndex, spawnersToDisplay);
                var selectedSpawner = spawnerPrefabs[selectedSpawnerIndex];

                EditorGUILayout.HelpBox(AdvancedModeText, MessageType.Error);
                EditorGUILayout.MinMaxSlider(new GUIContent("Angle"), ref minAngleValue, ref maxAngleValue, minAngleLimit, maxAngleLimit);
                          
                EditorGUILayout.EndToggleGroup();

                if (GUI.Button(new Rect(position.width - 125, position.height - 35, 120, 30), new GUIContent("Load Level"))
                    && SceneManager.GetActiveScene().name != selectedScene)
                {
                    Debug.Log("Loaded level:" + selectedScene);
                    SceneManager.LoadScene(selectedScene);
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
