using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode] public class ScreenshotCreator : MonoBehaviour {
	[System.Serializable] public class CameraObject {
		public GameObject cam;
		public bool deleteQuestion = false;
		public string hotkey = "";
	}

	[Tooltip("Select the number of cameras and drag them in here. If you want to use multiple cameras at the same time (e. g. with different depth layers), insert their parent Gameobject.")]
	public List <CameraObject> list = new List<CameraObject>();

	[HideInInspector] public bool foldoutSettings = false;

	// image settings
	[Tooltip("Select the screenshot resolution multiplier. If you select 1, the screenshot taken will have the same resolution as your Game View.")]
	[Range(1, 16)] public int superSize = 2;

	// name settings
	[Tooltip("The name of your screenshot or screenshot session. Camera name and current date will be added automatically.")]
	public string customName = "";
	[HideInInspector] public int lastCamID = 0;
	[HideInInspector] public Camera lastCam;
	public bool includeCamName = true;
	public bool includeDate = true;
	public bool includeResolution = true;

	// type settings
	public enum FileType {PNG, JPG};
	public FileType fileType;

	public enum CaptureMethod {RenderTexture, CaptureScreenshot};
	public CaptureMethod captureMethod;

	public void Create(){
		list.Add (new CameraObject());
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

	#if UNITY_EDITOR
	void LateUpdate(){
		if (Input.anyKeyDown) {
			//Debug.Log ("pressed something");
			for (int i = 0; i < list.Count; i++) {
				if (list [i].hotkey.Length != 1) {
					continue;
				}
				if (Input.GetKeyDown (list [i].hotkey)) {
					if (list [i] != null) {
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
					} else {
						Debug.Log ("Screenshot by Hotkey (" + list [i].hotkey + ") could not be created! Camera not available.");
					}
				}
			}
		}
	}
	#endif

	// create a more blurry screenshot if there are multiple cameras or no camera is found on the GameObject
	public void CaptureScreenshots(int id, bool fallback){
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
	public void CaptureRenderTexture(Camera attachedCam, int id){
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

	public string getFileName(int camID){
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

	public string getResolution(){
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