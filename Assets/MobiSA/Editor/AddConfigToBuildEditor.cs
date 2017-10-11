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
        const string filename = "DefaultConfig.txt";
        static string  defaultConfig = Application.dataPath + "/MobiSA/Config/"+filename;

        [PostProcessBuildAttribute(2)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var buildName = Path.GetFileNameWithoutExtension(pathToBuiltProject);

            var buildHostDirectory = pathToBuiltProject.Replace(Path.GetFileName(pathToBuiltProject), "");

            var dataDirectoryName = buildName + "_Data";

            var pathToDataDirectory = Path.Combine(buildHostDirectory, dataDirectoryName);

            var configPathInBuild = pathToDataDirectory + "/StreamingAssets/Config/";

            if (!Directory.Exists(configPathInBuild)) {
                Directory.CreateDirectory(configPathInBuild);
                Debug.Log(string.Format("[BUILD] Created Directory {0}", configPathInBuild));
            }
            if ((File.Exists(defaultConfig)) && (Directory.Exists(configPathInBuild)))
            {
                Debug.Log(string.Format("[BUILD] Copy default config from {0} to {1}", defaultConfig, configPathInBuild+filename));
                FileUtil.CopyFileOrDirectory(defaultConfig, configPathInBuild+filename);
            }
            
        }

    }
}