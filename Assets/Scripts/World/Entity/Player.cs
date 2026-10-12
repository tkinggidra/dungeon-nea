using Core;
using Core.Tick;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using World.Navigation;
using World.Player;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace World.Entity
{
    public class Player : LivingEntity
    {
        private PlayerInput _playerInput;
        private readonly PathNavigation _navigation;

        private bool _releaseAfterNavigation;
        public bool IsUnderScriptedControl { get; private set; }

        public Player(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _navigation = new PathNavigation(this);
        }

        public bool MoveToBlock(BlockPos destination, bool releaseAfter = false)
        {
            if (!IsActive || IsDead)
                return false;

            BlockPos start = BlockPos.FromWorldPosition(
                Transform.Position);

            var path = Level.PathFinder.FindPath(
                start,
                destination);
            
            if (path.Count == 0)
                return false;
            
            _navigation.SetPath(path);
            
            _navigation.SetFinalDestination(new Vector3(
                destination.X,
                Transform.Position.Y,
                destination.Z));
            
            _releaseAfterNavigation = releaseAfter;
            IsUnderScriptedControl = true;
            
            return true;
        }

        public void ReleaseControl()
        {
            _navigation.ClearPath();
            _releaseAfterNavigation = false;
            IsUnderScriptedControl = false;
        }
        
        protected override void OnTick(in TickContext context)
        {
            base.OnTick(in context);
            
            bool dash = _playerInput.Dash;
            _playerInput.Dash = false;
            
            Debug.Log(dash);

            if (dash && !IsUnderScriptedControl)
            {
                MoveToBlock(new BlockPos(20, 20), releaseAfter: true);
            }

            if (IsUnderScriptedControl)
            {
                _navigation.Tick(in context);

                if (_releaseAfterNavigation && !_navigation.HasPath)
                    ReleaseControl();
                
                return;
            }

            Vector2 movement = _playerInput.MovementValue;

            if (movement.LengthSquared() > 1f)
                movement = Vector2.Normalize(movement);

            float speed = 5f;
            float deltaTime = (float)context.TickDelta;

            Move(new Vector3(
                movement.X * speed * deltaTime,
                0,
                movement.Y * speed * deltaTime));
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