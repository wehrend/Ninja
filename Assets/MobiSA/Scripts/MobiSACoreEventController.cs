namespace Assets.MobiSA.Scripts {
    using UnityEngine;
    using System.Collections;
    using LSL4Unity.Scripts;

    public struct MobiSACoreEventArgs
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


	public delegate void MobiSACoreEventHandler(object sender, MobiSACoreEventArgs eve);

    public class MobiSACoreEventController : MonoBehaviour {


        public event MobiSACoreEventHandler FruitCollision;
        public event MobiSACoreEventHandler BombCollision;
        public event MobiSACoreEventHandler StartGame;
        public event MobiSACoreEventHandler GameOver;


        public void OnFruitCollision(MobiSACoreEventArgs eve)
        {
            if (FruitCollision != null)
            {
                FruitCollision(this, eve);
            }
        }

        public void OnBombCollision(MobiSACoreEventArgs eve)
        {
            if (BombCollision != null)
            {
                BombCollision(this, eve);
            }
        }


        public void OnStartGame(MobiSACoreEventArgs eve)
        {
            if (StartGame != null)
            {
                StartGame(this, eve);
            }
        }

        public void OnGameOver(MobiSACoreEventArgs eve)
        {
            if (GameOver != null)
            {
                GameOver(this, eve);
            }
        }
    }
}