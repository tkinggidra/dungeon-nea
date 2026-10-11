using Core.Tick;

namespace World.Entity
{
    public abstract class Mob : LivingEntity
    {
        protected Mob()
        {
            
        }

        protected abstract void RegisterGoals();

        protected override void OnTick(in TickContext context)
        {
            base.OnTick(in context);
        }
    }
}