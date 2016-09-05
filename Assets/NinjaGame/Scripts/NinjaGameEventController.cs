using UnityEngine;
using System.Collections;

namespace Assets.NinjaGame.Scripts {

    public struct NinjaGameEventArgs
    {
		public Vector3 collision_position;
		public int score;
		public int damage;
        public int totalscore;
        public int health;

	}

	public delegate void NinjaGameEventHandler(object sender, NinjaGameEventArgs eve);

	public class NinjaGameEventController : MonoBehaviour {


        public event NinjaGameEventHandler CollisionWithFruit;
        public event NinjaGameEventHandler CollisionWithBomb;
        public event NinjaGameEventHandler UpdateScore;
        public event NinjaGameEventHandler UpdateHealth;
        public event NinjaGameEventHandler StartGame;
        public event NinjaGameEventHandler GameOver;


        public void OnUpdateHealth(NinjaGameEventArgs eve)
        {
            if (UpdateHealth != null)
            {
                UpdateHealth(this, eve);
            }
        }

        public void OnUpdateScore(NinjaGameEventArgs eve)
        {
            if (UpdateScore != null)
            {
                UpdateScore(this, eve);
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