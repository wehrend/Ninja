namespace Assets.NinjaGame.Scripts {
    using UnityEngine;
    using System.Collections;
    using LSL4Unity.Scripts;

    public struct NinjaGameEventArgs
    {
        //track event at collision (has tio be abstracted to VREFs IMarkerStream)
        LSLMarkerStream collisionEvent;
	/*	public Vector3 collisionPosition;
        public Quaternion collisionRotationQuats;
        public Vector3 hmdPositionAtCollision;
        public Quaternion hmdRotationAtCllisionQuats;*/
        public int score;
		public int damage;
        public int totalscore;
        public int health;

	}


	public delegate void NinjaGameEventHandler(object sender, NinjaGameEventArgs eve);

    public class NinjaGameEventController : MonoBehaviour {


        public event NinjaGameEventHandler FruitCollision;
        public event NinjaGameEventHandler BombCollision;
        public event NinjaGameEventHandler UpdateScore;
        public event NinjaGameEventHandler UpdateHealth;
        public event NinjaGameEventHandler StartGame;
        public event NinjaGameEventHandler GameOver;


        public void OnFruitCollision(NinjaGameEventArgs eve)
        {
            if (FruitCollision != null)
            {
                FruitCollision(this, eve);
            }
        }

        public void OnBombCollision(NinjaGameEventArgs eve)
        {
            if (BombCollision != null)
            {
                BombCollision(this, eve);
            }
        }


        public void OnStartGame(NinjaGameEventArgs eve)
        {
            if (StartGame != null)
            {
                StartGame(this, eve);
            }
        }

        public void OnGameOver(NinjaGameEventArgs eve)
        {
            if (GameOver != null)
            {
                GameOver(this, eve);
            }
        }
    }
}