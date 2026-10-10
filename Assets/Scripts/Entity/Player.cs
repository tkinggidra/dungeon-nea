using System.Numerics;
using Core.Tick;
using World.Player;

namespace Entity
{
    public class Player : LivingEntity
    {
        private PlayerInput _playerInput;
        
        public Vector3 Position;

        public Player(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }
        
        protected override void OnTick(in TickContext context)
        {
            base.OnTick(context);

            Vector2 movement = _playerInput.MovementValue;

            Vector3 direction = new(
                movement.X,
                0f,
                movement.Y);
            
            Position += direction * 5 * (float)context.TickDelta;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
        }

        protected override void OnDeath()
        {
            base.OnDeath();
        }
    }
}