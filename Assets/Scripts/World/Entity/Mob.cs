using Core.Tick;
using World.Entity.AI;

namespace World.Entity
{
    public abstract class Mob : LivingEntity
    {
        public GoalSelector GoalSelector { get; } = new();
        
        protected Mob()
        {
            
        }

        protected abstract void RegisterGoals();

        protected override void OnTick(in TickContext context)
        {
            base.OnTick(in context);
            
            GoalSelector.Tick();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            RegisterGoals();
        }
    }
}