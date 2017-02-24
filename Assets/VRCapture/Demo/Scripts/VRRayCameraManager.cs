using UnityEngine;

namespace VRCapture.Demo {
    public class VRRayCameraManager : MonoBehaviour {
        [SerializeField]
        private GameObject cameraScreen;
        [SerializeField]
        private GameObject cameraSphere;
        [SerializeField]
        private GameObject facingObject;
        [SerializeField]
        private GameObject captureText;

        public Transform HandParent;
        public Vector3 screenPos = new Vector3(-0.2f, 0.2f, 0);
        private bool enabledCapture;
        private bool capturing;

        private GameObject CameraScreen {
            get {
                return cameraScreen;
            }

            set {
                cameraScreen = value;
            }
        }
        public GameObject FacingObject {
            get {
                return facingObject;
            }
            set {
                facingObject = value;
            }
        }
        private void Awake() {
            if(CameraScreen == null) {
                throw new MissingComponentException("CameraScreen not attached!");
            }
            if(cameraSphere == null) {
                throw new MissingComponentException("CameraSphere not attached!");
            }
            if(FacingObject == null) {
                throw new MissingComponentException("FacingObject not attached!");
            }
            if(captureText == null) {
                throw new MissingComponentException("CaptureText not attached!");
            }
        }
        private void FixedUpdate() {
            if(enabledCapture) {
                this.transform.LookAt(FacingObject.transform);
            }
        }
        public bool Enabled() {
            return enabledCapture;
        }

        public bool Capturing() {
            return capturing;
        }
        public void EnableCamera() {
            CameraScreen.SetActive(true);
            cameraSphere.SetActive(false);
            enabledCapture = true;
            cameraScreen.transform.parent = HandParent;
            CameraScreen.transform.localPosition = screenPos;
            CameraScreen.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }

        public void StartCapture() {
            capturing = true;
            captureText.SetActive(true);
            VRCapture.Instance.BeginCaptureSession();
        }

        public void FinishCapture() {
            captureText.SetActive(false);
            capturing = false;
            VRCapture.Instance.EndCaptureSession();

        }
    }
}
