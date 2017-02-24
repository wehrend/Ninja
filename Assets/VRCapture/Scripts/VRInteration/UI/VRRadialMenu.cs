using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRCapture {
    /// <summary>
    /// This script is setting radial menu attribute, and binding keys to listener events. 
    /// </summary>
    [ExecuteInEditMode]
    public class VRRadialMenu : MonoBehaviour {
        public List<RadialMenuButton> positionButton;
        /// <summary>
        /// Menu option key
        /// </summary>
        public GameObject positionPrefab;
        /// <summary>
        /// Menu picture is rotated
        /// </summary>
        public bool rotateIcons;

        public bool isShown;
        /// <summary>
        /// Whether or not to hide
        /// </summary>
        public bool hideOnRelease;
        /// <summary>
        /// Do not click the display menu
        /// </summary>
        public bool executeOnUnclick;
        /// <summary>
        /// RadialMenu Color
        /// </summary>
        public Color buttonColor = Color.white;
        [Range(0, 1599)]
        [Tooltip("Particle size of contact vibration")]
        public ushort baseHapticStrength;
        [Range(0, 359)]
        [Tooltip("Adjust the size of the rotation offset")]
        public float offsetRotation;
        [Range(0f, 1f)]
        [Tooltip("Thickness of ring menu")]
        public float buttonThickness = 0.5f;

        public List<GameObject> menuButtons;

        private void Awake() {
            if(Application.isPlaying) {
                if(!isShown) {
                    transform.localScale = Vector3.zero;
                }
                if(menuButtons.Count == 0) {
                    RegenerateButtons();
                }
            }
        }
        private int ButtonIndex = -1;
        private int CurrentTouch = -1;

        void Update() {
            if(CurrentTouch != -1) {
                positionButton[CurrentTouch].OnHold.Invoke();
            }
        }

        public void InteractButton(float angle, ButtonEvent btnevt) {
            float buttonAngle = 360f / positionButton.Count;
            angle = Mod((angle + offsetRotation), 360);
            int buttonID = (int)Mod(((angle + (buttonAngle / 2f)) / buttonAngle), positionButton.Count);
            var pointer = new PointerEventData(EventSystem.current);

            if(ButtonIndex != buttonID && ButtonIndex != -1) {
                ExecuteEvents.Execute(menuButtons[ButtonIndex], pointer, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(menuButtons[ButtonIndex], pointer, ExecuteEvents.pointerExitHandler);
                positionButton[ButtonIndex].OnHoverExit.Invoke();

                if(executeOnUnclick && CurrentTouch != -1) {
                    ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerDownHandler);
                    TryHapticPulse((ushort)(baseHapticStrength * 1.666f));
                }
            }
            if(btnevt == ButtonEvent.click) {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerDownHandler);
                CurrentTouch = buttonID;
                if(!executeOnUnclick) {
                    positionButton[buttonID].OnClick.Invoke();
                    TryHapticPulse((ushort)(baseHapticStrength * 2.5f));
                }
            }
            else if(btnevt == ButtonEvent.unclick) {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerUpHandler);
                CurrentTouch = -1;

                if(executeOnUnclick) {
                    positionButton[buttonID].OnClick.Invoke();
                    TryHapticPulse((ushort)(baseHapticStrength * 2.5f));
                }
            }
            else if(btnevt == ButtonEvent.hoverOn && ButtonIndex != buttonID) {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerEnterHandler);
                positionButton[buttonID].OnHoverEnter.Invoke();
                TryHapticPulse(baseHapticStrength);
            }
            ButtonIndex = buttonID;
        }

        private float Mod(float a, float b) {
            return a - b * Mathf.Floor(a / b);
        }

        private void TryHapticPulse(ushort strength) {
            if(strength > 0 && GetComponentInParent<SteamVR_TrackedObject>() != null) {
                SteamVR_Controller.Input((int)GetComponentInParent<SteamVR_TrackedObject>().index).TriggerHapticPulse(strength);
            }
        }
        /// <summary>
        /// Stop Touch RadialMenu
        /// </summary>
        public void StopTouching() {
            if(ButtonIndex != -1) {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(menuButtons[ButtonIndex], pointer, ExecuteEvents.pointerExitHandler);
                positionButton[ButtonIndex].OnHoverExit.Invoke();
                ButtonIndex = -1;
            }
        }
        /// <summary>
        /// Show menu
        /// </summary>
        public void EnableMenu() {
            if(!isShown) {
                isShown = true;
                StopCoroutine("TweenMenuScale");
                StartCoroutine("TweenMenuScale", isShown);
            }
        }
        /// <summary>
        /// Hide menu
        /// </summary>
        /// <param name="Close"></param>
        public void DisableMenu(bool Close) {
            if(isShown && (hideOnRelease || Close)) {
                isShown = false;
                StopCoroutine("TweenMenuScale");
                StartCoroutine("TweenMenuScale", isShown);
            }
        }

        /// <summary>
        /// Set the display of the ring menu
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        private IEnumerator TweenMenuScale(bool show) {
            float targetScale = 0;
            Vector3 dir = -1 * Vector3.one;
            if(show) {
                targetScale = 1;
                dir = Vector3.one;
            }
            int i = 0;
            while(i < 250 && ((show && transform.localScale.x < targetScale) || (!show && transform.localScale.x > targetScale))) {
                transform.localScale += dir * Time.deltaTime * 4f;
                yield return true;
                i++;
            }
            transform.localScale = dir * targetScale;
            StopCoroutine("TweenMenuScale");
        }
        /// <summary>
        /// Generate button corresponding to the ring menu
        /// </summary>
        public void RegenerateButtons() {
            RemoveAllButtons();
            for(int i = 0; i < positionButton.Count; i++) {
                GameObject newButton = Instantiate(positionPrefab);
                newButton.transform.SetParent(transform);
                newButton.transform.localScale = Vector3.one;
                newButton.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                newButton.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                VRDrawUICircle circle = newButton.GetComponent<VRDrawUICircle>();

                if(buttonThickness == 1f) {
                    circle.fill = true;
                }
                else {
                    circle.thickness = (int)(buttonThickness * (GetComponent<RectTransform>().rect.width / 2f));
                }
                int fillPerc = (int)(100f / positionButton.Count);
                circle.fillPercent = fillPerc;
                circle.color = buttonColor;

                float angle = ((360 / positionButton.Count) * i) + offsetRotation;
                newButton.transform.localEulerAngles = new Vector3(0, 0, angle);
                newButton.layer = 4;
                newButton.transform.localPosition = Vector3.zero;
                if(circle.fillPercent < 55) {
                    float angleRad = (angle * Mathf.PI) / 180f;
                    Vector2 angleVector = new Vector2(-Mathf.Cos(angleRad), -Mathf.Sin(angleRad));
                    newButton.transform.localPosition += (Vector3)angleVector;
                }


                GameObject buttonIcon = newButton.GetComponentInChildren<VRDrawUICircle>().gameObject;
                if(positionButton[i].ButtonIcon == null) {
                    buttonIcon.SetActive(true);
                }
                else {
                    buttonIcon.GetComponent<Image>().sprite = positionButton[i].ButtonIcon;
                    buttonIcon.transform.localPosition = new Vector2(-1 * ((newButton.GetComponent<RectTransform>().rect.width / 2f) - (circle.thickness / 2f)), 0);
                    float scale1 = Mathf.Abs(circle.thickness);
                    float R = Mathf.Abs(buttonIcon.transform.localPosition.x);
                    float bAngle = (359f * circle.fillPercent * 0.01f * Mathf.PI) / 180f;
                    float scale2 = (R * 2 * Mathf.Sin(bAngle / 2f));
                    if(circle.fillPercent > 24) {
                        scale2 = float.MaxValue;
                    }

                    float iconScale = Mathf.Min(scale1, scale2);
                    buttonIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(iconScale, iconScale);
                    if(!rotateIcons) {
                        buttonIcon.transform.eulerAngles = GetComponentInParent<Canvas>().transform.eulerAngles;
                    }
                }
                menuButtons.Add(newButton);

            }
        }
        /// <summary>
        /// clear All Button
        /// </summary>
        private void RemoveAllButtons() {
            if(menuButtons == null) {
                menuButtons = new List<GameObject>();
            }
            for(int i = 0; i < menuButtons.Count; i++) {
                if(menuButtons[i] != null) {
                    DestroyImmediate(menuButtons[i], true);
                }
            }
            menuButtons = new List<GameObject>();
        }
    }

    [System.Serializable]
    public class RadialMenuButton {
        public Sprite ButtonIcon;

        public UnityEvent OnClick;
        public UnityEvent OnHold;
        public UnityEvent OnHoverEnter;
        public UnityEvent OnHoverExit;
    }

    public enum ButtonEvent {
        hoverOn,
        hoverOff,
        click,
        unclick
    }
}