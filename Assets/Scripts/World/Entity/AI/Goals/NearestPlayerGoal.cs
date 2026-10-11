using System;

namespace World.Entity.AI.Goals
{
    public sealed class NearestPlayerGoal : Goal
    {
        private readonly Mob _mob;
        
        public float DetectionRange { get; set; }
        public float LoseTargetRange { get; set; }
        
        public override GoalFlag Flags => GoalFlag.None;

        public NearestPlayerGoal(
            Mob mob,
            float detectionRange,
            float loseTargetRange)
        {
            _mob = mob;
            
            if (detectionRange < 0 || loseTargetRange < detectionRange)
                throw new ArgumentOutOfRangeException(nameof(detectionRange));
            
            DetectionRange = detectionRange;
            LoseTargetRange = loseTargetRange;
        }
        
        public override bool CanUse()
        {
            return _mob.IsActive && _mob.Level != null;
        }

        public override bool CanContinue()
        {
            return CanUse();
        }

        public override void Tick()
        {
            if (_mob.Target != null)
            {
                if (IsValidTarget(_mob.Target, LoseTargetRange))
                    return;
                
                _mob.SetTarget(null);
            }

            Player nearest = null;
            float nearestDistanceSquared =
                DetectionRange * DetectionRange;

            foreach (var player in _mob.Level.Players)
            {
                if (!IsValidTarget(player, DetectionRange))
                    continue;
                
                float distanceSquared =
                    _mob.Transform.DistanceSquaredXZ(player.Transform);

                if (distanceSquared <= nearestDistanceSquared)
                {
                    nearest = player;
                    nearestDistanceSquared = distanceSquared;
                }
            }
            
            if (nearest != null)
                _mob.SetTarget(nearest);
        }

        public override void Stop()
        {
            _mob.SetTarget(null);
        }

        private bool IsValidTarget(
            LivingEntity target,
            float range)
        {
            if (!target.IsActive || target.IsDead)
                return false;
            
            if (!ReferenceEquals(target.Level, _mob.Level))
                return false;
            
            float distanceSquared =
                _mob.Transform.DistanceSquaredXZ(target.Transform);
            
            return distanceSquared <= range * range;
        }
    }
}