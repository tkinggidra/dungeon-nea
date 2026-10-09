using Core.Tick;

namespace Entity.Components
{
    public interface ITickingComponent : IEntityComponent
    {
        void Tick(in TickContext context);
    }
}