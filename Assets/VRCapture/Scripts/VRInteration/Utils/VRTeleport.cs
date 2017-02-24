using UnityEngine;
using System.Collections.Generic;

namespace VRCapture {
    /// <summary>
    /// This script implemented functionality of teleport in VR scene.
    /// </summary>
    public class VRTeleport : MonoBehaviour {
        /// <summary>
        /// can Load On flat
        /// </summary>
        public bool canLoadOnflat = true;
        /// <summary>
        /// can load max Angle
        /// </summary>
        [Range(0, 60)]
        public float maxLoadAngle = 30f;
        /// <summary>
        /// the line childrenPosition max distance
        /// </summary>
        public float maxDistance = 5f;
        /// <summary>
        /// linerenderer width
        /// </summary>
        public float lineTelepontWidth = 0.05f;
        public Color canTeleportColor = Color.blue;
        public Color unTeleportColor = Color.red;
        public GameObject vrTeleportSimple;
        public GameObject vrPlayAreaSimple;
        private GameObject vrTeleportItem;
        private GameObject vrPlayerAreaitem;
        private Transform vrCamera;
        private Transform vrPlayArea;
        private LineRenderer vrTeleportLine;
        private Vector3 teleportPoint;
        private bool isCanTeleport = false;
        private bool teleportActive;
        public bool TeleportActive {
            get {
                return teleportActive;
            }
        }

        void Awake() {
            SteamVR_Camera steamCamera = this.transform.parent.gameObject.GetComponentInChildren<SteamVR_Camera>();
            if(steamCamera == null) {
                vrCamera = Camera.main.transform;
            }
            else {
                vrCamera = steamCamera.transform;
            }
            if(vrCamera == null) {
                Debug.LogError("vrCamera is null");
                enabled = false;
                return;
            }
            SteamVR_PlayArea steamPlayArea = this.transform.parent.gameObject.GetComponent<SteamVR_PlayArea>();
            if(steamPlayArea == null) {
                vrPlayArea = transform.parent;
            }
            else {
                vrPlayArea = steamPlayArea.transform;
            }
            if(vrPlayArea == null) {
                Debug.LogError("vrplayarea is null");
                enabled = false;
                return;
            }

        }

        void Start() {

            InitTeleporter();
        }
        /// <summary>
        /// init teleporter Object
        /// </summary>
        void InitTeleporter() {
            GameObject vrTeleportObject = new GameObject(string.Format("[{0}]VrTeleport", gameObject.name));
            vrTeleportObject.transform.localScale = vrPlayArea.localScale;
            GameObject vrTeleportLineObject = new GameObject(string.Format("[{0}]VrTeleportLin", gameObject.name));
            vrTeleportLineObject.transform.SetParent(vrTeleportObject.transform);
            vrTeleportLine = vrTeleportLineObject.AddComponent<LineRenderer>();
            vrTeleportLine.SetWidth(lineTelepontWidth * vrPlayArea.localScale.magnitude, lineTelepontWidth * vrPlayArea.localScale.magnitude);
            vrTeleportLine.SetColors(canTeleportColor, canTeleportColor);
            vrTeleportLine.material = new Material(Shader.Find("Sprites/Default"));
            vrTeleportLine.enabled = false;
            if(vrPlayAreaSimple != null) {
                vrPlayerAreaitem = Instantiate(vrPlayAreaSimple, Vector3.zero, Quaternion.identity) as GameObject;
                vrPlayerAreaitem.transform.SetParent(vrTeleportObject.transform);
                vrPlayerAreaitem.transform.rotation = vrPlayArea.rotation;
                vrPlayerAreaitem.SetActive(false);
            }
            if(vrTeleportSimple != null) {
                vrTeleportItem = Instantiate(vrTeleportSimple, Vector3.zero, Quaternion.identity) as GameObject;
                vrTeleportItem.transform.SetParent(vrTeleportObject.transform);
                vrTeleportItem.SetActive(false);
            }
        }

        private void OnPressTouchpadUp() {
            SureDownPoint();
        }

        private void OnPressTouchpad() {
            SeachDownPoint();
        }
        public void SeachDownPoint() {
            vrTeleportLine.enabled = true;
            teleportActive = true;
            if(TeleportActive && isCanTeleport) {
                if(vrTeleportItem != null) vrTeleportItem.SetActive(true);
                if(vrPlayerAreaitem != null) vrPlayerAreaitem.SetActive(true);
            }
        }
        public void SureDownPoint() {
            vrTeleportLine.enabled = false;
            if(vrPlayerAreaitem != null) vrPlayerAreaitem.SetActive(false);
            if(vrTeleportItem != null) vrTeleportItem.SetActive(false);
            if(TeleportActive && isCanTeleport) {
                Vector3 camSpot = new Vector3(vrCamera.position.x, 0, vrCamera.position.z);
                Vector3 roomSpot = new Vector3(vrPlayArea.position.x, 0, vrPlayArea.position.z);
                Vector3 offset = roomSpot - camSpot;
                vrPlayArea.position = teleportPoint + offset;
            }
            teleportActive = false;
        }
        void Update() {
            if(!TeleportActive) return;
            UpdateTeleportLine();
        }
        /// <summary>
        /// Update Line
        /// </summary>
        void UpdateTeleportLine() {
            List<Vector3> positions = new List<Vector3>();
            Quaternion currentRotation = transform.rotation;
            Vector3 currentPosition = transform.position;
            positions.Add(currentPosition);
            Vector3 lastPostion = transform.position - transform.forward;
            Vector3 currentDirection = transform.forward;
            Vector3 downForward = new Vector3(transform.forward.x * 0.01f, -1, transform.forward.z * 0.01f);
            RaycastHit hit = new RaycastHit();
            float allDistance = 0;
            bool isFirstArray = true;
            int i = 0;
            while(i < 500) {
                i++;
                Quaternion forDownRotate = Quaternion.LookRotation(downForward);
                currentRotation = Quaternion.RotateTowards(currentRotation, forDownRotate, 1f);
                Ray ray = new Ray(currentPosition, currentPosition - lastPostion);
                float rayLenght = (maxDistance * 0.05f) * vrPlayArea.localScale.magnitude;
                if(currentRotation == forDownRotate) {
                    isFirstArray = false;
                }
                bool hitObject = false;
                hitObject = Physics.Raycast(ray, out hit, rayLenght);
                if(hitObject) {
                    if(isFirstArray) {
                        allDistance += (currentPosition - hit.point).magnitude;
                        positions.Add(hit.point);
                    }
                    break;
                }
                currentDirection = currentRotation * Vector3.forward;
                lastPostion = currentPosition;
                currentPosition += currentDirection * rayLenght;

                if(isFirstArray) {
                    allDistance += rayLenght;
                    positions.Add(currentPosition);
                }
            }
            if(isFirstArray) {
                teleportPoint = positions[positions.Count - 1];
            }
            isCanTeleport = CanTeleporter(hit);
            if(isCanTeleport) {
                vrTeleportLine.SetColors(canTeleportColor, canTeleportColor);
                if(vrPlayerAreaitem != null) {
                    vrPlayerAreaitem.SetActive(true);
                    Vector3 camPoint = new Vector3(vrCamera.position.x, 0, vrCamera.position.z);
                    Vector3 playAreaPoint = new Vector3(vrPlayArea.position.x, 0, vrPlayArea.position.z);
                    Vector3 offset = playAreaPoint - camPoint;
                    vrPlayerAreaitem.transform.position = (teleportPoint + offset) + hit.normal * 0.05f;
                }
            }
            else {
                vrTeleportLine.SetColors(unTeleportColor, unTeleportColor);
                if(vrPlayerAreaitem != null) {
                    vrPlayerAreaitem.SetActive(false);
                }
                if(vrTeleportItem != null) {
                    vrTeleportItem.SetActive(false);
                }
            }

            if(vrTeleportItem != null) {
                vrTeleportItem.transform.position = teleportPoint + (hit.normal * 0.05f);
                if(hit.normal == Vector3.zero) {
                    vrTeleportItem.transform.rotation = Quaternion.identity;
                }
                else {
                    vrTeleportItem.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
            }
            vrTeleportLine.SetVertexCount(positions.Count);
            vrTeleportLine.SetPositions(positions.ToArray());
        }
        /// <summary>
        /// Determine whether can transfer
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool CanTeleporter(RaycastHit hit) {
            if(hit.transform == null) return false;
            if(canLoadOnflat) {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if(angle > maxLoadAngle)
                    return false;
            }
            return true;
        }
    }

}
