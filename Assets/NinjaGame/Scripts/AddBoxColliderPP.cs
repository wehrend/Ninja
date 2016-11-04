using UnityEngine;
using UnityEditor;
using System.Collections;

public class AddBoxColliderPP :AssetPostprocessor
{

    void OnPostProcessModel(GameObject gob)
    {
        if (assetPath.Contains("Hand"))
        {
            Renderer[] allRenderers = gob.GetComponentsInChildren<Renderer>();
            foreach (Renderer R in allRenderers)
            {
                R.gameObject.AddComponent<BoxCollider>();
            }
        }
    } 
}
