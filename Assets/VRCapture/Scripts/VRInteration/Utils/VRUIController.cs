using System;
using UnityEngine;

namespace VRCapture {
    public enum ControllerState {
        normal,
        isRay,
        isTouch
    }
    /// <summary>
    /// Configure interaction mode.
    /// </summary>
    public class VRUIController : MonoBehaviour {
        [Tooltip("Laser Thickness")]
        public float laserThickness = 0.002f;
        [Tooltip("Laser HitScale")]
        public float laserHitScale = 0.02f;
        [Tooltip("Max Hit Distance")]
        public float maxDistance = 100.0f;
        public Color color;
        public ControllerState controllerState = ControllerState.normal;
        [NonSerialized]
        public GameObject selectedObject;
        public bool isShow = true;
        /// <summary>
        /// Ray and object encounter point
        /// </summary>
        private GameObject hitPoint;
        private GameObject pointer;
        private GameObject controllerRigidBodyObject;
        private float distanceLimit;

        void Start() {
            CreatRay();
            CreateControllerRigidBody();
        }

        void Update() {
            SetBoxColliderActive();
            if(isShow) {
                RayInteraction();
            }
            if(controllerState == ControllerState.isRay) {
                pointer.gameObject.GetComponent<MeshRenderer>().enabled = isShow;
            }
            else {
                if(hitPoint.activeSelf) {
                    hitPoint.SetActive(false);
                }
            }
        }
        /// <summary>
        /// creat Ray And HitPoint
        /// </summary>
        void CreatRay() {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pointer.transform.SetParent(transform, false);
            pointer.transform.localScale = new Vector3(laserThickness, laserThickness, 100.0f);
            pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);
            pointer.SetActive(false);
            hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitPoint.transform.SetParent(transform, false);
            hitPoint.transform.localScale = new Vector3(laserHitScale, laserHitScale, laserHitScale);
            hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);
            hitPoint.SetActive(false);
            DestroyImmediate(hitPoint.GetComponent<SphereCollider>());
            DestroyImmediate(pointer.GetComponent<CapsuleCollider>());
            Material newMaterial = new Material(Shader.Find("VRCapture/LaserPointer"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
            hitPoint.GetComponent<MeshRenderer>().material = newMaterial;
        }
        void RayInteraction() {
            if(controllerState == ControllerState.isRay) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                bool bHit = Physics.Raycast(ray, out hitInfo);

                float distance = maxDistance;
                if(bHit) {
                    distance = hitInfo.distance;
                    selectedObject = hitInfo.collider.gameObject;
                }
                else {
                    selectedObject = null;
                }
                if(distanceLimit > 0.0f) {
                    distance = Mathf.Min(distance, distanceLimit);
                    bHit = true;
                }

                pointer.transform.localScale = new Vector3(laserThickness, laserThickness, distance);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);

                if(bHit) {
                    hitPoint.SetActive(true);
                    hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
                }
                else {
                    hitPoint.SetActive(false);
                }

                distanceLimit = -1.0f;
            }
        }
        /// <summary>
        /// Great Mode Controller
        /// </summary>
        private void CreateControllerRigidBody() {
            controllerRigidBodyObject = this.gameObject;
            controllerRigidBodyObject.transform.localPosition = Vector3.zero;

            CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.01f, -0.098f), new Vector3(0.04f, 0.025f, 0.15f));
            CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.009f, -0.002f), new Vector3(0.05f, 0.025f, 0.04f));
            CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.024f, 0.01f), new Vector3(0.07f, 0.02f, 0.02f));
            CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.045f, 0.022f), new Vector3(0.07f, 0.02f, 0.022f));
            CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.0625f, 0.03f), new Vector3(0.065f, 0.015f, 0.025f));
            CreateBoxCollider(controllerRigidBodyObject, new Vector3(0.045f, -0.035f, 0.005f), new Vector3(0.02f, 0.025f, 0.025f));
            CreateBoxCollider(controllerRigidBodyObject, new Vector3(-0.045f, -0.035f, 0.005f), new Vector3(0.02f, 0.025f, 0.025f));

            var createRB = controllerRigidBodyObject.AddComponent<Rigidbody>();
            createRB.mass = 100f;

            var controllerRB = controllerRigidBodyObject.GetComponent<Rigidbody>();

            controllerRB.useGravity = false;
            controllerRB.isKinematic = false;
        }

        private void CreateBoxCollider(GameObject obj, Vector3 center, Vector3 size) {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = size;
            bc.center = center;
            bc.enabled = false;
        }

        void OnTriggerEnter(Collider other) {
            if(other != null && controllerState == ControllerState.isTouch) {
                selectedObject = other.gameObject;
            }
        }
        void SetBoxColliderActive() {
            if(controllerState == ControllerState.isRay) {
                pointer.SetActive(true);
                foreach(var item in this.gameObject.GetComponents<BoxCollider>()) {
                    item.enabled = false;
                }
            }
            if(controllerState == ControllerState.isTouch) {
                pointer.SetActive(false);
                foreach(var item in this.gameObject.GetComponents<BoxCollider>()) {
                    item.enabled = true;
                }
            }
        }
        void OnTriggerExit(Collider other) {
            selectedObject = null;
        }
    }
}

