using UnityEngine;

namespace VRCapture.Demo {
    public class VRTouchCameraManager : MonoBehaviour {
        [SerializeField]
        private GameObject cameraScreen;
        [SerializeField]
        private GameObject cameraSphere;
        [SerializeField]
        private GameObject captureText;
        private bool m_Enabled;
        private bool m_Capturing;
        private GameObject CameraScreen {
            get {
                return cameraScreen;
            }
            set {
                cameraScreen = value;
            }
        }

        private void Awake() {
            if(CameraScreen == null) {
                throw new MissingComponentException("CameraScreen not attached!");
            }
            if(cameraSphere == null) {
                throw new MissingComponentException("CameraSphere not attached!");
            }
            if(captureText == null) {
                throw new MissingComponentException("CaptureText not attached!");
            }
        }

        public bool Enabled() {
            return m_Enabled;
        }

        public bool Capturing() {
            return m_Capturing;
        }

        public void EnableCamera() {
            CameraScreen.SetActive(true);
            cameraSphere.SetActive(false);
            m_Enabled = true;
        }

        public void StartCapture() {
            m_Capturing = true;
            captureText.SetActive(true);
            VRCapture.Instance.BeginCaptureSession();
        }

        public void FinishCapture() {
            captureText.SetActive(false);
            m_Capturing = false;
            VRCapture.Instance.EndCaptureSession();

        }
    }
}
