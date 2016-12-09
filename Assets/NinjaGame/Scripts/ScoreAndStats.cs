using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace Assets.NinjaGame.Scripts
{
    public class ScoreAndStats : MonoBehaviour
    {
        public Text instructionText;
        public Text scoresText;
        public Scrollbar healthBar;
      //  private NinjaGame.GameInfo scores; 
        [HideInInspector]
        private NinjaGameEventController ninjaGameEvent;
        [HideInInspector]
        public NinjaGameEventArgs eve;
        // Use this for initialization
        void Start()
        {
            Debug.Log("Test");
            instructionText.text = "Touch the spheres";
            scoresText = GetComponent<Text>();
            //healthBar = GetComponent<Scrollbar>();
            ninjaGameEvent = FindObjectOfType(typeof(NinjaGameEventController)) as NinjaGameEventController;
           
            if (ninjaGameEvent == null)
            {
                Debug.LogError("The NinjaGameController needs the NinjaGameEventController script to be attached to it");
                return;
            }
            ninjaGameEvent.GameOver += new NinjaGameEventHandler(gameOver);

        }

        void Update()
        {
            scoresText.text = "Score:\n" + NinjaGame.game.totalscore.ToString();
            //healthBar.size = (float) NinjaGame.game.health / 1000f;
        }


        void gameOver(object sender, NinjaGameEventArgs eve)
        {
            instructionText.color = Color.red;
            instructionText.text = "Game Over!";
        }
    }
}