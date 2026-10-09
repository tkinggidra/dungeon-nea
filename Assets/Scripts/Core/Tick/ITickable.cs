namespace Core.Tick
{
    public interface ITickable
    {
        void Tick(in TickContext context);
    }
}