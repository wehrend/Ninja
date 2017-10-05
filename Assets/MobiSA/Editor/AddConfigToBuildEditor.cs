using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections;

using System.Collections.Generic;


namespace Assets.MobiSA.Scripts
{
    public class AddConfigToBuildEditor
    {
        static string  defaultConfig = Application.dataPath + "/MobiSA/Config/DefaultConfig";
        static string configPathInBuild = Application.streamingAssetsPath + "/Config/";

        [PostProcessBuildAttribute(2)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (!Directory.Exists(configPathInBuild)) {
                Directory.CreateDirectory(configPathInBuild);
                Debug.Log(string.Format("[BUILD] Created Directory {0}", configPathInBuild));
            }
            if ((File.Exists(defaultConfig)) && (Directory.Exists(configPathInBuild)))
            {
                Debug.Log(string.Format("[BUILD] Copy default config from {0} to {1}", defaultConfig, configPathInBuild));
                FileUtil.CopyFileOrDirectory(defaultConfig, configPathInBuild);
            }
            
        }

    }
}