using UnityEngine;

namespace Assets.NinjaGame.EditorExtensions
{
    [ExecuteInEditMode]
    public class PositionHandle : MonoBehaviour
    {
        public Vector3 lookTarget = new Vector3(0, 2, 0);

        public void Update()
        {
            transform.LookAt(lookTarget);
        }
    }

}