using System.Numerics;
using Core.Tick;
using World.Player;

namespace World.Entity
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
            base.OnTick(in context);

            Vector2 movement = _playerInput.MovementValue;

            if (movement.LengthSquared() > 1f)
                movement = Vector2.Normalize(movement);

            float speed = 5f;
            float deltaTime = (float)context.TickDelta;

            Transform.Translate(new Vector3(
                movement.X * speed * deltaTime,
                0f,
                movement.Y * speed * deltaTime
            ));
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