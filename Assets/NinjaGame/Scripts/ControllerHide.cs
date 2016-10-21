using UnityEngine;
using System.Collections;
using VRTK;

namespace Assets.NinjaGame.Scripts
{
    public class ControllerHide : MonoBehaviour
    {


        ControllerInteractionEventHandler touchpadAxisChanged;
        VRTK_ControllerActions actions;
        VRTK_ControllerEvents events;
        Rigidbody thumb; 
     
        // Use this for initialization
        void Start()
        {

            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

    
            events = GetComponent<VRTK_ControllerEvents>();
            actions = GetComponent<VRTK_ControllerActions>();
            touchpadAxisChanged = new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouched);
            events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchReleased);
            Debug.Log("Event handler installed");
            thumb = new Rigidbody();
        
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
        
            var controllerEvents = (VRTK_ControllerEvents)sender;
            /* if (moveOnButtonPress != VRTK_ControllerEvents.ButtonAlias.Undefined && !controllerEvents.IsButtonPressed(moveOnButtonPress))
             {
                 touchAxis = Vector2.zero;
                 return;
             }*/
            thumb.transform.position = Camera.main.ScreenToWorldPoint(e.touchpadAxis);
        }



        private void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
        {
            actions.ToggleHighlightTouchpad(true, Color.blue, 0.5f);
            actions.SetControllerOpacity(0.0f);
        }

        private void DoTouchpadTouchReleased(object sender, ControllerInteractionEventArgs e)
        {
            actions.ToggleHighlightTouchpad(false);
            if (!events.AnyButtonPressed())
            {
                actions.SetControllerOpacity(1f);
            }
        }



    }
}