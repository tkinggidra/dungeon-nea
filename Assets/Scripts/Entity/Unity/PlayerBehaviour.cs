using UnityEngine;

namespace Entity.Unity
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private Player _player;

        public void Initialize(Player player)
        {
            _player = player;
        }

        public void SyncTransform()
        {
            var position = _player.Position;
            
            print(position);

            transform.position = new Vector3(
                position.X,
                position.Y,
                position.Z);
            
            Debug.Log(
                $"Entity: {position} | Unity: {transform.position}"
            );
        }
    }
}