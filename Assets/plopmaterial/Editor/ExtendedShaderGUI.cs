using UnityEngine;
using UnityEditor;

//Custom GUI for the extended "Plop" shader 
public class ExtendedShaderGUI : ShaderGUI {

    MaterialEditor editor;
    MaterialProperty[] properties;

    bool renderdoubleside = true;


    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {



        Material targetMat = editor.target as Material;
        renderdoubleside = EditorGUILayout.Toggle("Render Doubleside", renderdoubleside);
        targetMat.SetShaderPassEnabled("Backside",renderdoubleside);

        base.OnGUI(editor, properties);

    }
}
