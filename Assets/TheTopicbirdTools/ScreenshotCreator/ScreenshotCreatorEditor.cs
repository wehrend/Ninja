using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ScreenshotCreator))]
public class ScreenshotCreatorEditor : Editor {
	[SerializeField] ScreenshotCreator script;

	void OnEnable(){
		script = (ScreenshotCreator)target;
	}

	void OnDisable(){
		refreshRequests ();
	}

	// reset all X questions to standard
	void refreshRequests(){
		for (int i = 0; i < script.list.Count; i++) {
			script.list[i].deleteQuestion = false;
		}
	}

	public override void OnInspectorGUI() {
		EditorUtility.SetDirty (target);

		GUI.color = Color.white;
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);
		EditorGUI.indentLevel++;
		script.foldoutSettings = EditorGUILayout.Foldout(script.foldoutSettings, "Settings");

		EditorGUI.indentLevel--;

		if (script.foldoutSettings) {
			EditorGUILayout.BeginHorizontal ();
			script.superSize = EditorGUILayout.IntSlider ("size multiplier", script.superSize, 1, 16);
			EditorGUILayout.LabelField (script.getResolution (), EditorStyles.boldLabel, GUILayout.MaxWidth (100));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.SelectableLabel ("folder: " + System.IO.Directory.GetCurrentDirectory () + @"\Screenshots\");

			script.customName = EditorGUILayout.TextField ("custom name", script.customName);

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("CamName")) {
				script.includeCamName = !script.includeCamName;
			}
			if (GUILayout.Button ("Date")) {
				script.includeDate = !script.includeDate;
			}
			if (GUILayout.Button ("Resolution")) {
				script.includeResolution = !script.includeResolution;
			}
			script.fileType = (ScreenshotCreator.FileType)EditorGUILayout.EnumPopup (script.fileType);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.LabelField ("file name: " + script.getFileName (script.lastCamID));

			EditorGUILayout.Space ();

			script.captureMethod = (ScreenshotCreator.CaptureMethod)EditorGUILayout.EnumPopup ("capture method", script.captureMethod);
		}

		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.Label ("Cameras:", EditorStyles.boldLabel);

		for (int i = 0; i < script.list.Count; i++) {
			ScreenshotCreator.CameraObject c = script.list[i];

			GUI.color = Color.white;
			EditorGUILayout.BeginHorizontal (EditorStyles.helpBox);

			script.list[i].cam = (GameObject) EditorGUILayout.ObjectField(script.list[i].cam, typeof(GameObject), true);

			EditorGUI.BeginDisabledGroup (script.list [i].cam == null);
			script.list[i].hotkey = EditorGUILayout.TextField(script.list[i].hotkey, GUILayout.MaxWidth(24));
			EditorGUI.EndDisabledGroup();

			//EditorGUI.BeginDisabledGroup (!EditorApplication.isPlaying);
			if (script.list [i].cam != null) {
				if (GUILayout.Button ("USE " + script.list [i].cam.name, new GUIStyle(EditorStyles.miniButtonLeft))) {
					refreshRequests();
					if (script.captureMethod == ScreenshotCreator.CaptureMethod.RenderTexture) {
						Camera attachedCam = script.list [i].cam.GetComponent<Camera> ();
						if (attachedCam == null) {
							script.CaptureScreenshots (i, true);
						} else {
							script.CaptureRenderTexture (attachedCam, i);
						}
					} else if (script.captureMethod == ScreenshotCreator.CaptureMethod.CaptureScreenshot) {
						script.CaptureScreenshots (i, false);
					}

					script.lastCam = script.list [script.lastCamID].cam.GetComponent<Camera> ();
				}
			}
			//EditorGUI.EndDisabledGroup();

			// the delete button
			if (c.deleteQuestion){
				GUI.color = Color.red;
				if (GUILayout.Button ("YES?", new GUIStyle(EditorStyles.miniButtonRight), GUILayout.MaxWidth(45), GUILayout.MaxHeight(14))) {
					refreshRequests();
					script.Delete (i);
				}
			} else {
				GUI.color = (Color.red + Color.white * 2f) / 3f;
				if (GUILayout.Button ("X", new GUIStyle(EditorStyles.miniButtonRight), GUILayout.MaxWidth(45), GUILayout.MaxHeight(14))) {
					refreshRequests();
					script.RequestDelete (i);
				}
			}

			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.Space ();

		GUI.color = new Color (0.54f, 0.68f, 0.95f);
		if(GUILayout.Button("ADD CAMERA", GUILayout.MaxWidth (100), GUILayout.MinWidth (100), GUILayout.MaxHeight (25), GUILayout.MinHeight (25))) {
			refreshRequests();
			script.Create ();
		}
	}
}