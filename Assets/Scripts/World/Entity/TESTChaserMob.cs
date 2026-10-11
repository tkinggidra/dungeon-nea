using World.Entity.AI.Goals;
using World.Navigation;

namespace World.Entity
{
    public sealed class TESTChaserMob : Mob
    {
        private readonly PathFinder _pathFinder;

        public TESTChaserMob(PathFinder pathFinder)
        {
            _pathFinder = pathFinder;
        }

        protected override void RegisterGoals()
        {
            GoalSelector.AddGoal(
                0,
                new NearestPlayerGoal(this, 150f, 200f)
            );

            GoalSelector.AddGoal(
                1,
                new ChaseTargetGoal(this, _pathFinder)
                {
                    StopDistance = 1.5f
                }
            );
        }
    }
}