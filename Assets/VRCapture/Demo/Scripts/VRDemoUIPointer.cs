
namespace VRCapture.Demo {

    public class VRDemoUIPointer : VRUIPointer {

        private bool isClick = false;
        private VREventController eventController;
        private VRUIController vrUIController;
        public override void Awake() {
            base.Awake();
            if(eventController == null) {
                eventController = this.GetComponent<VREventController>();
            }
            if(vrUIController == null) {
                vrUIController = this.GetComponent<VRUIController>();
            }
        }

        public override bool ButtonClick() {
            return isClick;
        }

        void OnEnable() {
            if(eventController != null) {
                eventController.OnPressTriggerDown += OnPressTriggerDown;
                eventController.OnPressTriggerUp += OnPressTriggerUp;
            }
        }

        private void OnPressTriggerUp() {
            isClick = false;
        }

        private void OnPressTriggerDown() {
            isClick = true;
        }

        void OnDisable() {
            if(eventController != null) {
                eventController.OnPressTriggerDown -= OnPressTriggerDown;
                eventController.OnPressTriggerUp -= OnPressTriggerUp;
            }
        }
    }
}