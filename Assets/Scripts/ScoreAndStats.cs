using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ScoreAndStats : MonoBehaviour {

    public Text instructionText;
    public Text scoresText;
    public int scores;

	// Use this for initialization
	void Start () {
        instructionText.text = "Swing the Sword. \n And delete the catch the blue spheres.";
        scores = 0;

	}
	
	// Update is called once per frame
	void Update () {
       instructionText.text = "Swing the Sword. \n And delete the catch the blue spheres.";
       scoresText.text = "Score:\n" + scores.ToString();


    }
}
