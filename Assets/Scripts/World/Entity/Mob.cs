using Core.Tick;
using World.Entity.AI;
using World.Navigation;

namespace World.Entity
{
    public abstract class Mob : LivingEntity
    {
        public GoalSelector GoalSelector { get; } = new();
        public PathNavigation Navigation { get; }
        
        public LivingEntity Target { get; private set; }
        
        protected Mob()
        {
            Navigation = new PathNavigation(this);
        }

        public void SetTarget(LivingEntity target)
        {
            Target = target;
        }

        protected abstract void RegisterGoals();

        protected override void OnTick(in TickContext context)
        {
            base.OnTick(in context);
            
            GoalSelector.Tick();
            Navigation.Tick(in context);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            RegisterGoals();
        }
    }
}