using System;
using UnityEngine;
using VRCapture;
using VRCapture.Demo;

public class TooltipsManager : MonoBehaviour {
    public GameObject triggerButton;
    public GameObject gripButton;
    public GameObject touchpadButton;
    public GameObject applicationMenuButton;
    [NonSerialized]
    public bool isClickTrigger = true;
    [NonSerialized]
    public bool isClickGrip = true;
    [NonSerialized]
    public bool isClickTouchpad = true;
    [NonSerialized]
    public bool isClickApplicationMenu = true;
    [NonSerialized]
    public VRTooltipController tooltipController;
    private void Awake() {
        if(tooltipController == null) {
            tooltipController = this.GetComponentInChildren<VRTooltipController>();
        }
    }

    // Use this for initialization
    void Start() {
        isClickTrigger = true;
        isClickGrip = false;
        isClickTouchpad = false;
        isClickApplicationMenu = false;
        tooltipController.triggerText = "Click to Choose";
    }
    void Update() {
        if(triggerButton != null) {
            triggerButton.SetActive(isClickTrigger);
        }
        if(gripButton != null) {
            gripButton.SetActive(isClickGrip);
        }
        if(touchpadButton != null) {
            touchpadButton.SetActive(isClickTouchpad);
        }
        if(applicationMenuButton != null) {
            applicationMenuButton.SetActive(isClickApplicationMenu);
        }
    }
}
