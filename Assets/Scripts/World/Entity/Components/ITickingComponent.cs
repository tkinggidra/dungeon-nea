using Core.Tick;

namespace World.Entity.Components
{
    public interface ITickingComponent : IEntityComponent
    {
        void Tick(in TickContext context);
    }
}