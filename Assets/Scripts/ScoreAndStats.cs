using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ScoreAndStats : MonoBehaviour {

    
    public Text instructionText;
    public Text scoresText;
    public Scrollbar healthBar;
    private float health;
    private GameController gameController;
    private int score;
	// Use this for initialization
	void Start() {
        gameController =  FindObjectOfType(typeof(GameController)) as GameController;
       
    }
	
	// Update is called once per frame
	void Update () {
        if (gameController != null)
        {
            instructionText.text = "Swing the Sword.\nCatch the blue spheres, and avoid the black bombs";
            score = gameController.getScores();
            scoresText.text = "Score:\n" + score.ToString();
            health = gameController.getHealth();
            healthBar.size = health / 1000f;
        }
        else
        {
            Debug.LogError("GameController not found!");
        }


    }
}
