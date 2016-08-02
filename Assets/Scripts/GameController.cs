using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public float health;
    public int score;

    public void HappenWhenSwordIsGrabbed(GameObject grabbedObject)
    {
        Debug.Log("Object has been grabbed");
    }

	// Use this for initialization
	void Start () {
        health = 1000;
        score = 0;
	}

	public void OnBombCollision( GameObject BombObject ) {

	   // health -= BombObject.damage;

	}


    public void OnFruitCollision( GameObject FruitObject ) {

       // score += FruitObject.bonusPoints;

    }

}
