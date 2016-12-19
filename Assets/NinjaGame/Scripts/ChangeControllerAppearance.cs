using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;



namespace Assets.NinjaGame.Scripts
{

    public class ChangeControllerAppearance : MonoBehaviour
    {

        bool touchPressed;
        bool triggerPressed;
        GameObject go;
        GameObject hand;
        GameObject controller;
        // Use this for initialization
        void Start()
        {

            for (int i=0; i < transform.childCount; i++ )
            {
                go = transform.GetChild(i).gameObject;
                if (go.name == "Model") {
                    var controller = go;
                    controller.SetActive(true);
                }
                if (go.name == "HandCursor_edited")
                {
                    var hand = go;
                    hand.SetActive(false);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            touchPressed = GetComponentInParent<VRTK_ControllerEvents>().touchpadPressed;
            triggerPressed = GetComponentInParent<VRTK_ControllerEvents>().triggerPressed;

            if (touchPressed)
            {
                Debug.Log("StartGame");
                hand.SetActive(true);
                controller.SetActive(false);
            }

        }
    }
}