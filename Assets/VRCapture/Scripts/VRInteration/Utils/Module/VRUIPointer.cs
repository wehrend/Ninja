using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VRCapture {

    public struct UIPointerEventArgs {
        public bool isActive;
        public GameObject currentTarget;
        public GameObject previousTarget;
    }

    public delegate void UIPointerEventHandler(object sender, UIPointerEventArgs e);
    /// <summary>
    /// This script is used to take response for interaction event, wrapper for EventSystem.
    /// </summary>
    public class VRUIPointer : MonoBehaviour {
        public enum ActivationMethods {
            HoldButton,
            ToggleButton,
            AlwaysOn
        }

        public ActivationMethods activationMode = ActivationMethods.HoldButton;
        [HideInInspector]
        public PointerEventData pointerEventData;
        [HideInInspector]
        public GameObject hoveringElement;
        public event UIPointerEventHandler UIPointerElementEnter;

        public event UIPointerEventHandler UIPointerElementExit;

        private bool pointerClicked = false;
        private bool beamEnabledState = false;
        private bool lastPointerPressState = false;

        public virtual void OnUIPointerElementEnter(UIPointerEventArgs e) {
            if(UIPointerElementEnter != null) {
                UIPointerElementEnter(this, e);
            }
        }

        public virtual void OnUIPointerElementExit(UIPointerEventArgs e) {
            if(UIPointerElementExit != null) {
                UIPointerElementExit(this, e);

                if(!e.isActive && e.previousTarget) {
                    pointerEventData.pointerPress = e.previousTarget;
                }
            }
        }

        public UIPointerEventArgs SetUIPointerEvent(GameObject currentTarget, GameObject lastTarget = null) {
            UIPointerEventArgs e;
            e.isActive = PointerActive();
            e.currentTarget = currentTarget;
            e.previousTarget = lastTarget;
            return e;
        }

        public VRInputModule SetEventSystem(EventSystem eventSystem) {
            if(!eventSystem) {
                Debug.LogError("A VRUIPoint requires an EventSystem");
                return null;
            }

            var standaloneInputModule = eventSystem.gameObject.GetComponent<StandaloneInputModule>();
            if(standaloneInputModule.enabled) {
                standaloneInputModule.enabled = false;
            }

            var eventSystemInput = eventSystem.GetComponent<VRInputModule>();
            if(!eventSystemInput) {
                eventSystemInput = eventSystem.gameObject.AddComponent<VRInputModule>();
                eventSystemInput.Initialise();
            }

            return eventSystemInput;
        }

        public void SetWorldCanvas(Canvas canvas) {
            var defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            var customRaycaster = canvas.gameObject.GetComponent<VRUIGraphicRaycaster>();
            if(!customRaycaster) {
                customRaycaster = canvas.gameObject.AddComponent<VRUIGraphicRaycaster>();
            }

            if(defaultRaycaster && defaultRaycaster.enabled) {
                customRaycaster.ignoreReversedGraphics = defaultRaycaster.ignoreReversedGraphics;
                customRaycaster.blockingObjects = defaultRaycaster.blockingObjects;
                defaultRaycaster.enabled = false;
            }

            var canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
            if(!canvas.gameObject.GetComponent<BoxCollider>()) {
                var canvasBoxCollider = canvas.gameObject.AddComponent<BoxCollider>();
                canvasBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, 0.01f);
                canvasBoxCollider.center = new Vector3(0f, 0f, -0.005f);
            }

            if(!canvas.gameObject.GetComponent<Image>()) {
                canvas.gameObject.AddComponent<Image>().color = Color.clear;
            }
        }

        public bool PointerActive() {
            if(activationMode == ActivationMethods.AlwaysOn) {
                return true;
            }
            else if(activationMode == ActivationMethods.HoldButton) {
                return true;
            }
            else {
                pointerClicked = false;
                if(ButtonClick() && !lastPointerPressState) {
                    pointerClicked = true;
                }
                lastPointerPressState = ButtonClick();

                if(pointerClicked) {
                    beamEnabledState = !beamEnabledState;
                }

                return beamEnabledState;
            }
        }

        public virtual bool ButtonClick() {
            return false;
        }

        public virtual void Awake() {
            ConfigureEventSystem();
            ConfigureWorldCanvases();
            pointerClicked = false;
            lastPointerPressState = false;
            beamEnabledState = false;
        }
        /// <summary>
        /// Configure eventsystem property
        /// </summary>
        private void ConfigureEventSystem() {
            var eventSystem = FindObjectOfType<EventSystem>();
            var eventSystemInput = SetEventSystem(eventSystem);

            pointerEventData = new PointerEventData(eventSystem);
            eventSystemInput.pointers.Add(this);
        }

        private void ConfigureWorldCanvases() {
            foreach(var canvas in FindObjectsOfType<Canvas>()) {
                SetWorldCanvas(canvas);
            }
        }
    }
}

