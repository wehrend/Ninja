using UnityEngine;
using UnityEditor;

namespace VRCapture.Editor {

    [CustomEditor(typeof(VRRadialMenu))]
    public class VRRadialMenuInspector : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            VRRadialMenu rMenu = (VRRadialMenu)target;
            if(GUILayout.Button("Regenerate Buttons")) {
                rMenu.RegenerateButtons();
            }
        }
    }
}