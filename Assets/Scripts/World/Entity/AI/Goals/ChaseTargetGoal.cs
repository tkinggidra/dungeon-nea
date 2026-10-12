using System;
using System.Numerics;
using Core;
using World.Navigation;

namespace World.Entity.AI.Goals
{
    public sealed class ChaseTargetGoal : Goal
    {
        private readonly Mob _mob;
        private readonly PathFinder _pathFinder;

        private bool _hasValidRoute;

        private BlockPos? _lastTargetCell;
        
        public override GoalFlag Flags => GoalFlag.Move;

        public float StopDistance { get; set; } = 1.5f;

        public ChaseTargetGoal(Mob mob, PathFinder pathFinder)
        {
            _mob = mob;
            _pathFinder = pathFinder;
        }

        public override bool CanUse()
        {
            return HasValidTarget() && !IsWithinStoppingDistance();
        }

        public override bool CanContinue()
        {
            return CanUse();
        }

        public override void Start()
        {
            _lastTargetCell = null;
            UpdatePath();
        }

        public override void Tick()
        {
            if (!HasValidTarget())
                return;

            if (IsWithinStoppingDistance())
            {
                _mob.Navigation.ClearPath();
                return;
            }

            UpdatePath();
        }

        public override void Stop()
        {
            _mob.Navigation.ClearPath();
            _lastTargetCell = null;
            _hasValidRoute = false;
        }

        private void UpdatePath()
        {
            Vector3 targetPosition = _mob.Target.Transform.Position;
            
            var targetCell = BlockPos.FromWorldPosition(targetPosition);

            if (_lastTargetCell != targetCell)
            {
                Vector3 mobPosition = _mob.Transform.Position;
                var startCell = BlockPos.FromWorldPosition(mobPosition);

                var path = _pathFinder.FindPath(
                    startCell,
                    targetCell);
                
                _mob.Navigation.SetPath(path);
                
                _hasValidRoute = path.Count > 0;
                
                _lastTargetCell = targetCell;
                
                if (path.Count == 0)
                    return;
            }

            if (_hasValidRoute)
            {
                _mob.Navigation.SetFinalDestination(targetPosition);
            }
        }

        private bool HasValidTarget()
        {
            LivingEntity target = _mob.Target;
            
            return _mob.IsActive &&
                   target is { IsActive: true, IsDead: false } &&
                   ReferenceEquals(target.Level, _mob.Level);
        }

        private bool IsWithinStoppingDistance()
        {
            if (_mob.Target == null)
                return false;

            float distanceSquared =
                _mob.Transform.DistanceSquaredXZ(
                    _mob.Target.Transform);
            
            return distanceSquared <= StopDistance * StopDistance;
        }
    }
}