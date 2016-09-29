using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class ShowLevelNames {
     
    [MenuItem("Foo/SHOW LEVELS")]
    public static void Show()
    {
        var window = EditorWindow.GetWindow<DemoLevelListWindow>();

        window.Show();


    }

    internal static List<GuiContainerForScenes> GetAllScenes()
    {
        var scenes = AssetDatabase.FindAssets("t:Scene").Select(
            guid => {

                var path = AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileNameWithoutExtension(path);
                return new GuiContainerForScenes()
                {
                    Name = name,
                    Guid = guid,
                    Path = path,
                    guiContent = new GUIContent(name)
                };
            
            } ).ToList();

        return scenes;
    }

    public class DemoLevelListWindow : EditorWindow
    {
        List<GuiContainerForScenes> allScenes;
        GUIContent[] listToDisplay;


        int selectedIndex = 0; //default
        
        void OnEnable()
        {
            allScenes = GetAllScenes();
            listToDisplay = allScenes.Select(s => s.guiContent).ToArray();
        }

        void OnGUI()
        {
            int heightOfTheSelectedBox = 20;
            selectedIndex = EditorGUI.Popup(new Rect(0, 0, position.width, heightOfTheSelectedBox), new GUIContent( "Available Scenes"), selectedIndex, listToDisplay);

            Debug.Log("Selected: " + allScenes[selectedIndex].Name);
            
        }
    }

    internal class GuiContainerForScenes
    {
        public string Name;
        public string Guid;
        public string Path;
        public GUIContent guiContent;
    }
}

