using UnityEngine;
using System.Collections;
using VRCapture;
using System;
using VRCapture.Demo;

public class UIControllerManager : MonoBehaviour {
    public ControllerState controllerState = ControllerState.normal;
    [Tooltip("Shoot camera")]
    public VRRayCameraManager cameraManagerToRay;
    [Tooltip("Default shooting point")]
    public VRChangCameraPoint point;
    public VRTouchCameraManager touchCameraManager;
    private GameObject cameras;
    private bool isTouch = false;
    private VRTeleport teleport;
    private bool isCamera;
    private VRUIController vrUIController;
    private TooltipsManager tooltipsManager;
    protected VRRadialMenu menu;
    [NonSerialized]
    public VREventController eventController;

    void Awake() {
        if(vrUIController == null) {
            vrUIController = this.transform.GetComponent<VRUIController>();
        }
        if(tooltipsManager == null) {
            tooltipsManager = this.GetComponent<TooltipsManager>();
        }

        if(eventController == null) {
            eventController = this.GetComponent<VREventController>();
        }
        isCamera = false;
        Application.runInBackground = true;

        menu = this.transform.GetComponentInChildren<VRRadialMenu>();
        if(teleport == null) {
            teleport = this.GetComponent<VRTeleport>();
        }
    }
    private void Start() {
        if(eventController != null) {
            tooltipsManager = eventController.GetComponent<TooltipsManager>();
            tooltipsManager.isClickTrigger = true;
            if(controllerState == ControllerState.isRay) {
                tooltipsManager.isClickTrigger = true;
                tooltipsManager.tooltipController.triggerText = "Click Choose Camera";
            }
            else if(controllerState == ControllerState.isTouch) {
                tooltipsManager.isClickTrigger = false;
            }
        }
        VRCapture.VRCapture.Instance.RegisterSessionCompleteDelegate(HandleCaptureFinish);
        Application.runInBackground = true;
    }
    void OnEnable() {
        if(eventController != null) {
            eventController.OnPressApplicationMenuDown += OnPressApplicationMenuDown;
            eventController.OnPressTrigger += OnPressTrigger;
            eventController.OnPressTouchpadUp += OnPressTouchpadUp;
            eventController.OnSwipeLeft += OnSwipeLeft;
            eventController.OnSwipeRight += OnSwipeRight;
            eventController.OnPressTriggerUp += OnPressTriggerUp;
            eventController.OnPressTouchpad += OnPressTouchpad;

            eventController.OnTouchPadTouch += OnTouchPadTouch;
            eventController.OnTouchPadTouchUp += OnTouchPadTouchUp;
            eventController.OnPressTouchpadDown += OnPressTouchpadDown;
        }
    }

    private void OnPressApplicationMenuDown() {
        if(controllerState == ControllerState.isTouch) {

            if(!touchCameraManager.Capturing()) {
                touchCameraManager.StartCapture();
                tooltipsManager.tooltipController.appMenuText = "End Capture";
                isTouch = false;
            }
            else {
                touchCameraManager.FinishCapture();
                tooltipsManager.tooltipController.appMenuText = "Begin Capture";
                isTouch = true;
            }
        }
        else if(controllerState == ControllerState.isRay) {
            if(!cameraManagerToRay.Capturing()) {
                cameraManagerToRay.StartCapture();
                tooltipsManager.tooltipController.appMenuText = "End Capture";
            }
            else {
                cameraManagerToRay.FinishCapture();
                tooltipsManager.tooltipController.appMenuText = "Begin Capture";
            }
        }

    }

    private void OnPressTouchpadUp() {
        if(controllerState == ControllerState.isRay) {
            if(eventController.axisAngle != 0) {
                menu.InteractButton(eventController.axisAngle, ButtonEvent.unclick);
                isCamera = false;
                tooltipsManager.isClickTouchpad = false;
                tooltipsManager.isClickApplicationMenu = true;
                tooltipsManager.tooltipController.appMenuText = "Click Capture";
            }
        }
        else if(controllerState == ControllerState.isTouch) {
            if(tooltipsManager != null) {
                tooltipsManager.isClickTouchpad = false;
                tooltipsManager.isClickTrigger = true;
                tooltipsManager.tooltipController.triggerText = "Touch Choose Camera";
            }
            if(teleport != null) {
                teleport.SureDownPoint();
            }
        }
    }

    private void OnPressTrigger() {
        if(controllerState == ControllerState.isRay) {
            if(vrUIController.selectedObject != null) {
                if(vrUIController.selectedObject.GetComponent<Camera>() != null) {
                    if(!cameraManagerToRay.Enabled()) {
                        cameraManagerToRay.EnableCamera();
                    }
                    point.ChangCameraPoint();
                    isCamera = true;
                    tooltipsManager.isClickTrigger = false;
                    tooltipsManager.isClickTouchpad = true;
                    tooltipsManager.tooltipController.touchpadText = "Touch Change Point";
                }
            }
        }
        else if(controllerState == ControllerState.isTouch) {
            if(vrUIController.selectedObject != null) {
                if(vrUIController.selectedObject.GetComponent<Camera>() != null) {
                    cameras = vrUIController.selectedObject;
                    cameras.transform.parent = this.transform;
                    touchCameraManager.EnableCamera();
                    isTouch = true;
                }
            }
        }
    }

    private void OnPressTouchpadDown() {
        if(controllerState == ControllerState.isRay) {
            if(eventController.axisAngle != 0) {
                menu.InteractButton(eventController.axisAngle, ButtonEvent.click);
            }
        }
    }

    private void OnTouchPadTouchUp() {
        if(controllerState == ControllerState.isRay) {
            menu.StopTouching();
            menu.DisableMenu(false);
        }
    }

    private void OnTouchPadTouch() {
        if(controllerState == ControllerState.isRay) {
            if(isCamera) {
                menu.EnableMenu();
                if(eventController.axisAngle != 0) {
                    menu.InteractButton(eventController.axisAngle, ButtonEvent.hoverOn);
                    tooltipsManager.tooltipController.touchpadText = "Click Choose Point";
                }
            }
        }
    }

    private void OnSwipeRight() {
        if(controllerState == ControllerState.isTouch) {
            if(isTouch) {
                cameras.transform.Rotate(Vector3.down, 10);
                eventController.HapticPulse(3000);
            }
        }
    }

    private void OnSwipeLeft() {
        if(controllerState == ControllerState.isTouch) {
            if(isTouch) {
                cameras.transform.Rotate(Vector3.up, 10);
                eventController.HapticPulse(3000);
            }
        }
    }

    private void OnPressTouchpad() {
        if(controllerState == ControllerState.isTouch) {
            if(teleport != null) {
                teleport.SeachDownPoint();
            }
        }
    }

    private void OnPressTriggerUp() {
        if(controllerState == ControllerState.isTouch) {
            if(cameras != null) {
                cameras.transform.parent = null;
            }
            tooltipsManager.isClickTrigger = false;
            tooltipsManager.isClickApplicationMenu = true;
            tooltipsManager.tooltipController.appMenuText = "Click Capture";
        }
    }

    void OnDisable() {
        if(eventController != null) {
            eventController.OnPressApplicationMenuDown -= OnPressApplicationMenuDown;
            eventController.OnPressTrigger -= OnPressTrigger;
            eventController.OnPressTouchpadUp -= OnPressTouchpadUp;
            eventController.OnSwipeLeft -= OnSwipeLeft;
            eventController.OnSwipeRight -= OnSwipeRight;
            eventController.OnPressTouchpad -= OnPressTouchpad;
            eventController.OnPressTriggerUp -= OnPressTriggerUp;
            eventController.OnTouchPadTouch -= OnTouchPadTouch;
            eventController.OnTouchPadTouchUp -= OnTouchPadTouchUp;
            eventController.OnPressTouchpadDown -= OnPressTouchpadDown;
        }
    }
    void HandleCaptureFinish() {
        print("Capture Finish");
    }
}

