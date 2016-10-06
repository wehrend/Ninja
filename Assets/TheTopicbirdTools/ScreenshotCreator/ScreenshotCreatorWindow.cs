using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ScreenshotCreatorWindow : EditorWindow {
	List <ScreenshotCreator.CameraObject> list = new List<ScreenshotCreator.CameraObject>();

	bool foldoutSettings = false;

	// image settings
	int superSize = 2;

	// name settings
	string customName = "";
	int lastCamID = 0;
	Camera lastCam;
	bool includeCamName = true;
	bool includeDate = true;
	bool includeResolution = true;

	// type settings
	enum FileType {PNG, JPG};
	FileType fileType;

	enum CaptureMethod {RenderTexture, CaptureScreenshot};
	CaptureMethod captureMethod;

	// window menu entry
	[MenuItem ("Window/Screenshot Creator")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(ScreenshotCreatorWindow));

		ScreenshotCreatorWindow window = EditorWindow.GetWindow<ScreenshotCreatorWindow>();
		GUIContent cont = new GUIContent ("Screenshots");
		window.titleContent = cont;
	}

	void OnDisable(){
		refreshRequests ();
	}

	// reset all X delete questions to standard
	void refreshRequests(){
		for (int i = 0; i < list.Count; i++) {
			list[i].deleteQuestion = false;
		}
	}

	void OnGUI () {
		GUI.color = Color.white;
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		foldoutSettings = EditorGUILayout.Foldout(foldoutSettings, "Settings");

		if (foldoutSettings) {
			EditorGUILayout.BeginHorizontal ();
			superSize = EditorGUILayout.IntSlider ("size multiplier", superSize, 1, 16);
			EditorGUILayout.LabelField (getResolution (), EditorStyles.boldLabel, GUILayout.MaxWidth (100));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.SelectableLabel ("folder: " + System.IO.Directory.GetCurrentDirectory () + @"\Screenshots\");

			customName = EditorGUILayout.TextField ("custom name", customName);

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("CamName")) {
				includeCamName = !includeCamName;
			}
			if (GUILayout.Button ("Date")) {
				includeDate = !includeDate;
			}
			if (GUILayout.Button ("Resolution")) {
				includeResolution = !includeResolution;
			}
			fileType = (FileType)EditorGUILayout.EnumPopup (fileType);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.LabelField ("file name: " + getFileName (lastCamID));

			EditorGUILayout.Space ();

			captureMethod = (CaptureMethod)EditorGUILayout.EnumPopup ("capture method", captureMethod);
		}

		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.Label ("Cameras:", EditorStyles.boldLabel);

		for (int i = 0; i < list.Count; i++) {
			ScreenshotCreator.CameraObject c = list[i];

			GUI.color = Color.white;
			EditorGUILayout.BeginHorizontal (EditorStyles.helpBox);

			list[i].cam = (GameObject) EditorGUILayout.ObjectField(list[i].cam, typeof(GameObject), true);

			//EditorGUI.BeginDisabledGroup (!EditorApplication.isPlaying);
			if (list [i].cam != null) {
				if (GUILayout.Button ("USE " + list [i].cam.name, new GUIStyle(EditorStyles.miniButtonLeft))) {
					refreshRequests();
					if (captureMethod == CaptureMethod.RenderTexture) {
						Camera attachedCam = list [i].cam.GetComponent<Camera> ();
						if (attachedCam == null) {
							CaptureScreenshots (i, true);
						} else {
							CaptureRenderTexture (attachedCam, i);
						}
					} else if (captureMethod == CaptureMethod.CaptureScreenshot) {
						CaptureScreenshots (i, false);
					}

					lastCam = list [lastCamID].cam.GetComponent<Camera> ();
				}
			}
			//EditorGUI.EndDisabledGroup();

			// the delete button
			if (c.deleteQuestion){
				GUI.color = Color.red;
				if (GUILayout.Button ("YES?", new GUIStyle(EditorStyles.miniButtonRight), GUILayout.MaxWidth(45), GUILayout.MaxHeight(14))) {
					refreshRequests();
					Delete (i);
				}
			} else {
				GUI.color = (Color.red + Color.white * 2f) / 3f;
				if (GUILayout.Button ("X", new GUIStyle(EditorStyles.miniButtonRight), GUILayout.MaxWidth(45), GUILayout.MaxHeight(14))) {
					refreshRequests();
					RequestDelete (i);
				}
			}

			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.Space ();

		GUI.color = new Color (0.54f, 0.68f, 0.95f);
		if(GUILayout.Button("ADD CAMERA", GUILayout.MaxWidth (100), GUILayout.MinWidth (100), GUILayout.MaxHeight (25), GUILayout.MinHeight (25))) {
			refreshRequests();
			Create ();
		}
	}

	// create a more blurry screenshot if there are multiple cameras or no camera is found on the GameObject
	void CaptureScreenshots(int id, bool fallback){
		for (int i = 0; i < list.Count; i++) {
			if (list[i].cam != null)
				list [i].cam.SetActive (false);
		}
		list[id].cam.SetActive (true);

		if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Screenshots/")){
			Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Screenshots/");
		}

		string fileName = Directory.GetCurrentDirectory() + "/Screenshots/";

		fileName += getFileName (id);

		Application.CaptureScreenshot (fileName, superSize);

		if (fallback) {
			Debug.Log ("Fallback to Application.CaptureScreenshot because a GameObject without Camera (or Camera group) was used. Screenshot saved to: " + fileName);
		} else {
			Debug.Log ("Screenshot saved to: " + fileName);
		}
	}

	// create a sharp screenshot for a single Camera
	void CaptureRenderTexture(Camera attachedCam, int id){
		for (int i = 0; i < list.Count; i++) {
			if (list[i].cam != null)
				list [i].cam.SetActive (false);
		}
		list[id].cam.SetActive (true);

		if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Screenshots/")){
			Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Screenshots/");
		}

		string fileName = Directory.GetCurrentDirectory() + "/Screenshots/";

		fileName += getFileName (id);

		int resWidth = attachedCam.pixelWidth * superSize;
		int resHeight = attachedCam.pixelHeight * superSize;

		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);

		attachedCam.targetTexture = rt;
		Texture2D screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGB24, false);
		attachedCam.Render ();
		RenderTexture.active = rt;
		screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
		attachedCam.targetTexture = null;
		RenderTexture.active = null;
		DestroyImmediate (rt);
		byte[] bytes = screenShot.EncodeToPNG ();

		System.IO.File.WriteAllBytes (fileName, bytes);
		Debug.Log ("Screenshot saved to: " + fileName);
	}

	public void Create(){
		list.Add (new ScreenshotCreator.CameraObject());
	}

	public void RequestDelete (int id){
		list [id].deleteQuestion = true;
	}

	public void Delete (int id){
		list.Remove (list [id]);

		if (list.Count == 0) {
			Create ();
		}
	}

	void Awake(){
		if (list.Count == 0) {
			Create ();
		}
	}

	string getFileName(int camID){
		string fileName = "";

		// custom name
		if (customName != "") {
			fileName += customName;
		} else {
			string dp = Application.dataPath;
			string[] s;
			s = dp.Split("/"[0]);
			fileName += s[s.Length - 2];
		}

		// include cam name
		if (includeCamName){
			fileName += "_";

			if (camID < 0 || camID >= list.Count || list[camID] == null || list[camID].cam == null) {
				fileName += "CameraName";
				lastCamID = 0;
			} else {
				fileName += list [camID].cam.name;
				lastCamID = camID;
			}
		}

		// include date
		if (includeDate) {
			fileName += "_";
	
			fileName += DateTime.Now.ToString ("yyyy-MM-dd-HH-mm-ss");
		}

		// include resolution
		if (includeResolution){
			fileName += "_";

			fileName += getResolution ();
		}

		// select filetype
		if (fileType == FileType.JPG) {
			fileName += ".jpg";
		} else if (fileType == FileType.PNG){
			fileName += ".png";
		}
			
		return fileName;
	}

	string getResolution(){
		//return gameViewDimensions.width * superSize + "x" + gameViewDimensions.height * superSize;

		if (lastCam == null || list[lastCamID].cam != lastCam.gameObject) {
			if (list [lastCamID].cam != null) {
				lastCam = list [lastCamID].cam.GetComponentInChildren<Camera> ();
			} else {
				for (int i = 0; i < list.Count; i++) {
					if (list [i] == null || list [i].cam == null)
						continue;
					lastCam = list [i].cam.GetComponentInChildren<Camera> ();
					if (lastCam != null) {
						break;
					}
				}
			}
		}

		if (lastCam == null) {
			return "-x-";
		}
			
		return lastCam.pixelWidth * superSize + "x" + lastCam.pixelHeight * superSize;
	}
}