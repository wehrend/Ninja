using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnerRangeVisualisationEditor))]
public class SpawnerRangeVisualisationEditor : Editor
{
    float spawnerDistance = 5.0f;

    void OnSceneGUI()
    {
        SpawnerRangeVisualisation t = target as SpawnerRangeVisualisation;
        Debug.Log("SpawnerRangeViz");
        Handles.color = Color.green;
        
        Handles.DrawSolidArc( t.transform.position, t.transform.up, t.transform.right, 360, spawnerDistance);
    }


}
