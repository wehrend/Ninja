using UnityEngine;
using System.IO;
using System.Xml;

namespace VRCapture {
    public enum CameraControllerType {
        none,
        followCamera,
        PosingCamera
    }
    /// <summary>
    /// Used for setup positions of camera, and save position information in local cache.
    /// </summary>
    public class VRChangCameraPoint : MonoBehaviour {
        /// <summary>
        /// Recording camera
        /// </summary>
        public Camera cameras;
        /// <summary>
        /// Shooting followed by the object
        /// </summary>
        public Transform target = null;
        private float distance;
        private float keepDistance;
        /// <summary>
        /// The camera moving damping
        /// </summary>
        public float smooth = 0.5f;
        private float distanceHeight = 2f;
        private bool isHightToCamera = false;
        Vector3 oldDistance;

        void Start() {
            oldDistance = target.transform.position - transform.position;
            keepDistance = Vector3.Distance(this.transform.position, target.transform.position);
        }

        void FixedUpdate() {
            distance = Vector3.Distance(this.transform.position, target.transform.position);
        }

        void LateUpdate() {
            if(distance > keepDistance) {
                transform.position =
                    Vector3.Lerp(
                        transform.position,
                        (target.transform.position - oldDistance),
                        Time.deltaTime * smooth);
            }
            if(target.transform.position.y > transform.position.y) {
                isHightToCamera = true;
            }
            else if(transform.position.y >= (target.transform.position.y + distanceHeight)) {
                isHightToCamera = false;
            }
            if(isHightToCamera) {
                transform.position =
                    Vector3.Lerp(transform.position,
                    new Vector3(
                        transform.position.x,
                        target.transform.position.y + distanceHeight,
                        transform.position.z),
                    Time.deltaTime * smooth);
            }
        }

        public void ChangCameraPoint() {
            OnButtonChangePosCilck(cameras, transform);
        }
        /// <summary>
        /// To change the position of the camera after the selected
        /// </summary>
        /// <param name="camera">Recording camera </param>
        /// <param name="Pos">Player setting recording camera information</param>
        private void OnButtonChangePosCilck(Camera camera, Transform Pos) {
            camera.transform.SetParent(this.transform, false);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.position = Pos.position;
            camera.transform.SetParent(this.transform, false);
            camera.transform.localPosition = Vector3.zero;
            camera.GetComponent<BoxCollider>().enabled = false;
        }
        /// <summary>
        /// Record position information with XML and store it
        /// </summary>
        public void CreatXML() {
            this.transform.parent = null;
            string filepath = System.IO.Path.GetFullPath(string.Format(@"{0}/", "Cache"));
            if(!Directory.Exists(filepath)) {
                Directory.CreateDirectory(filepath);
            }
            Vector3 distanceVe3 = target.position - this.transform.position;
            float hightBetween = this.transform.position.y - target.position.y;
            string filepaths = filepath + "FollowCameraSpot.xml";
            if(!File.Exists(filepaths)) {
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement root = xmlDoc.CreateElement("transforms");
                XmlElement elmNew = xmlDoc.CreateElement("position");
                XmlElement rotationX = xmlDoc.CreateElement("x");
                rotationX.InnerText = distanceVe3.x.ToString();
                XmlElement rotationY = xmlDoc.CreateElement("y");
                rotationY.InnerText = distanceVe3.y.ToString();
                XmlElement rotationZ = xmlDoc.CreateElement("z");
                rotationZ.InnerText = distanceVe3.z.ToString();
                XmlElement distanceY = xmlDoc.CreateElement("DistanceY");
                distanceY.InnerText = hightBetween.ToString();

                elmNew.AppendChild(rotationX);
                elmNew.AppendChild(rotationY);
                elmNew.AppendChild(rotationZ);
                root.AppendChild(elmNew);
                xmlDoc.AppendChild(root);
                xmlDoc.Save(filepaths);
            }
            else {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filepaths);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("transforms").ChildNodes;
                foreach(XmlElement xe in nodeList) {
                    if(xe.Name == "position") {
                        foreach(XmlElement x1 in xe.ChildNodes) {
                            if(x1.Name == "x") {
                                x1.InnerText = distanceVe3.x.ToString();
                            }
                            else if(x1.Name == "y") {
                                x1.InnerText = distanceVe3.y.ToString();
                            }
                            else if(x1.Name == "z") {
                                x1.InnerText = distanceVe3.z.ToString();
                            }
                        }
                    }
                    else if(xe.Name == "DistanceY") {
                        xe.InnerText = hightBetween.ToString();
                    }
                }
                xmlDoc.Save(filepaths);
            }
        }
    }
}
