using UnityEngine;
using Valve.VR;
using System;

namespace VRCapture {
    /// <summary>
    /// This script is used to take response for SteamVR device controller.
    /// </summary>
    [RequireComponent(typeof(SteamVR_TrackedObject))]
    public class VREventController : MonoBehaviour {
        [NonSerialized]
        public Vector2 deviceAxis;
        [NonSerialized]
        public float axisAngle;
        private SteamVR_TrackedObject trackedController;
        private SteamVR_Controller.Device device;

        private readonly Vector2 mXAxis = new Vector2(1, 0);
        private readonly Vector2 mYAxis = new Vector2(0, 1);
        private bool isTrackingSwipe = false;
        private bool isCheckSwipe = false;
        private const float mAngleRange = 30;
        private const float mMinSwipeDist = 0.2f;
        private const float mMinVelocity = 4.0f;
        private Vector2 mStartPosition;
        private Vector2 endPosition;
        private float mSwipeStartTime;
        public delegate void PressTrigger();
        public delegate void PressTriggerDown();
        public delegate void PressTriggerUp();
        public delegate void PressGrip();
        public delegate void PressGripDown();
        public delegate void PressGripUp();
        public delegate void PressApplicationMenu();
        public delegate void PressApplicationMenuDown();
        public delegate void PressApplicationMenuUp();
        public delegate void PressTouchpad();
        public delegate void PressTouchpadDown();
        public delegate void PressTouchpadUp();
        public delegate void TouchPadTouch();
        public delegate void TouchPadTouchDown();
        public delegate void TouchPadTouchUp();
        public delegate void SwipeLeft();
        public delegate void SwipeRight();
        public delegate void SwipeTop();
        public delegate void SwipeBottom();

        /// <summary>
        ///  The triggering event when the trigger button is pressed
        /// </summary>
        public event PressTrigger OnPressTrigger;
        /// <summary>
        /// The triggering event when the trigger button is pressed down
        /// </summary>
        public event PressTriggerDown OnPressTriggerDown;
        /// <summary>
        /// The triggering event when the trigger button is pressed up
        /// </summary>
        public event PressTriggerUp OnPressTriggerUp;
        /// <summary>
        /// The triggering event when the Grip button is pressed
        /// </summary>
        public event PressGrip OnPressGrip;
        /// <summary>
        /// The triggering event when the Grip button is pressed down
        /// </summary>
        public event PressGripDown OnPressGripDown;
        /// <summary>
        /// The triggering event when the Grip button is pressed up
        /// </summary>
        public event PressGripUp OnPressGripUp;
        /// <summary>
        /// The triggering event when the applicationMenu button is pressed
        /// </summary>
        public event PressApplicationMenu OnPressApplicationMenu;
        /// <summary>
        /// The triggering event when the applicationMenu button is pressed down
        /// </summary>
        public event PressApplicationMenuDown OnPressApplicationMenuDown;
        /// <summary>
        /// The triggering event when the applicationMenu button is pressed up
        /// </summary>
        public event PressApplicationMenuUp OnPressApplicationMenuUp;
        /// <summary>
        /// The triggering event when the touchpad button is pressed
        /// </summary>
        public event PressTouchpad OnPressTouchpad;
        /// <summary>
        /// The triggering event when the touchpad button is pressed down
        /// </summary>
        public event PressTouchpadDown OnPressTouchpadDown;
        /// <summary>
        /// The triggering event when the touchpad button is pressed up
        /// </summary>
        public event PressTouchpadUp OnPressTouchpadUp;
        /// <summary>
        /// The triggering event when the touchpad button is touched
        /// </summary>
        public event TouchPadTouch OnTouchPadTouch;
        /// <summary>
        /// The triggering event when the touchpad button is touched up
        /// </summary>
        public event TouchPadTouchUp OnTouchPadTouchUp;
        /// <summary>
        /// The triggering event when the touchpad button is touched down
        /// </summary>
        public event TouchPadTouchDown OnTouchPadTouchDown;
        /// <summary>
        /// The triggering event when the touchpad button is Swiped left
        /// </summary>
        public event SwipeLeft OnSwipeLeft;
        /// <summary>
        /// The triggering event when the touchpad button is Swiped right
        /// </summary>
        public event SwipeRight OnSwipeRight;
        /// <summary>
        /// The triggering event when the touchpad button is Swiped up
        /// </summary>
        public event SwipeTop OnSwipeTop;
        /// <summary>
        /// The triggering event when the touchpad button is Swiped down
        /// </summary>
        public event SwipeBottom OnSwipeBottom;


        void Awake() {
            trackedController = this.GetComponent<SteamVR_TrackedObject>();
        }

        void FixedUpdate() {

            device = SteamVR_Controller.Input((int)trackedController.index);
            deviceAxis = device.GetAxis();
            axisAngle = ChangeTouchpadAxisAngle(deviceAxis);
        }
        void Update() {
            if(device != null) {
                if(device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger)) {
                    if(OnPressTrigger != null) OnPressTrigger();
                }
                if(device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger)) {
                    if(OnPressTriggerDown != null) OnPressTriggerDown();
                }
                if(device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger)) {
                    if(OnPressTriggerUp != null) OnPressTriggerUp();
                }
                if(device.GetPress(EVRButtonId.k_EButton_Grip)) {
                    if(OnPressGrip != null) { OnPressGrip(); }
                }
                if(device.GetPressDown(EVRButtonId.k_EButton_Grip)) {
                    if(OnPressGripDown != null) { OnPressGripDown(); }
                }
                if(device.GetPressUp(EVRButtonId.k_EButton_Grip)) {
                    if(OnPressGripUp != null) { OnPressGripUp(); }
                }
                if(device.GetPress(EVRButtonId.k_EButton_ApplicationMenu)) {
                    if(OnPressApplicationMenu != null) OnPressApplicationMenu();
                }
                if(device.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu)) {
                    if(OnPressApplicationMenuDown != null) OnPressApplicationMenuDown();
                }
                if(device.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu)) {
                    if(OnPressApplicationMenuUp != null) OnPressApplicationMenuUp();
                }
                if(device.GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    if(OnPressTouchpad != null) OnPressTouchpad();
                }
                if(device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    if(OnPressTouchpadDown != null) OnPressTouchpadDown();
                }
                if(device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    if(OnPressTouchpadUp != null) OnPressTouchpadUp();
                }
                if(device.GetTouch(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    if(OnTouchPadTouch != null) OnTouchPadTouch();
                }
                if(device.GetTouchUp(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    if(OnTouchPadTouchUp != null) OnTouchPadTouchUp();
                }
                if(device.GetTouchDown(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    if(OnTouchPadTouchDown != null) OnTouchPadTouchDown();
                }
                if((int)trackedController.index != -1 && device.GetTouchDown(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    isTrackingSwipe = true;
                    mStartPosition = new Vector2(device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).x, device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).y);
                    mSwipeStartTime = Time.time;
                }
                else if(device.GetTouchUp(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
                    isTrackingSwipe = false;
                    isTrackingSwipe = true;
                    isCheckSwipe = true;
                }
                else if(isTrackingSwipe) {
                    endPosition = new Vector2(device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).x, device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).y);
                }
                if(isCheckSwipe) {
                    isCheckSwipe = false;
                    float deltaTime = Time.time - mSwipeStartTime;
                    Vector2 swipeVector = endPosition - mStartPosition;
                    float velocity = swipeVector.magnitude / deltaTime;
                    if(velocity > mMinVelocity && swipeVector.magnitude > mMinSwipeDist) {
                        swipeVector.Normalize();
                        float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
                        angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                        if(angleOfSwipe < mAngleRange) {
                            if(OnSwipeRight != null) OnSwipeRight();
                        }
                        else if((180.0f - angleOfSwipe) < mAngleRange) {
                            if(OnSwipeLeft != null) OnSwipeLeft();
                        }
                        else {
                            angleOfSwipe = Vector2.Dot(swipeVector, mYAxis);
                            angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                            if(angleOfSwipe < mAngleRange) {
                                if(OnSwipeTop != null) OnSwipeTop();
                            }
                            else if((180.0f - angleOfSwipe) < mAngleRange) {
                                if(OnSwipeBottom != null) OnSwipeBottom();
                            }
                        }
                    }
                }

            }
        }
        /// <summary>
        /// Control handle vibration method 
        /// </summary>
        /// <param name="index"> Vibration numerical</param>
        public void HapticPulse(int index) {
            device.TriggerHapticPulse(ushort.Parse(index.ToString()));
        }
        /// <summary>
        /// Judgment of sliding Angle method
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private float ChangeTouchpadAxisAngle(Vector2 axis) {
            float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
            angle = 90.0f - angle;
            if(angle < 0) {
                angle += 360.0f;
            }
            return 360 - angle;
        }
    }
}
