using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections;

using System.Collections.Generic;


namespace Assets.NinjaGame.Scripts
{
    public class AddConfigToBuildEditor
    {
        [PostProcessBuildAttribute(2)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            //Debug.Log(pathToBuiltProject);
            if (!Directory.Exists(Application.streamingAssetsPath + "/Config/")) {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Config/");
                Debug.Log("[BUILD] Created Directory " + Application.streamingAssetsPath + "/Config/");
            }
            Debug.Log("[BUILD] Copy config directory from " + NinjaGame.configDataDirectory + "to " + Application.streamingAssetsPath + "/Config/");
            FileUtil.CopyFileOrDirectory(NinjaGame.configDataDirectory, Application.streamingAssetsPath + "/Config/");
        }

    }
}