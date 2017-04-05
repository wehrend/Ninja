using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.NinjaGame.Scripts
{
    public class MainMenuController : MonoBehaviour
    {
        private GameObject mainMenu;
        private Button startButton;
        private Button quitButton;
        private static ExperimentSceneController esc;

        // Use this for initialization
        void Awake()
        {
            esc = GameObject.Find("[ExperimentSceneController]").GetComponent<ExperimentSceneController>();
            mainMenu = this.transform.Find("MainMenu").gameObject;
            startButton = mainMenu.transform.Find("Start").GetComponent<Button>();
            quitButton = mainMenu.transform.Find("Quit").GetComponent<Button>();

            AddButtonEvent(startButton);
            AddButtonEvent(quitButton);

        }


        void AddButtonEvent(Button button)
        {
            button.onClick.AddListener(delegate ()
            {
                ButtonClick(button);
            });
        }

        void ButtonClick(Button buttonName)
        {
            if (buttonName != null)
            {
                switch (buttonName.name)
                {
                    case "Start":
                        Start();
                        break;
                    case "ChooseConfig":
                        ChooseConfig();
                        break;
                    case "Quit":
                        Exit();
                        break;
                }
            }
        }


        void Update()
        {
            if (Input.GetKey(KeyCode.KeypadEnter))
                Start();
        } 

        private void Start()
        {
            esc.startGame = true;
        }

        private void ChooseConfig()
        {
            
        }

        void Exit()
        {
            Application.Quit();
        }

    }
}
