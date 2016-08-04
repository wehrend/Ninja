using UnityEngine;
using UnityEditor;
using System.Collections;



namespace Assets.NinjaGame.EditorExtensions
{
    public class ExperimentConfigurationEditor : EditorWindow
    {
        [MenuItem("NinjaGame/Configuration")]

        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ExperimentConfigurationEditor));
        }

        void OnGUI()
        {
            GUILayout.Label("NinjaGame Experiment Configuration",EditorStyles.boldLabel);

            GUILayout.Label("Start Positions (Target behind Subject)");







        }

    }
}