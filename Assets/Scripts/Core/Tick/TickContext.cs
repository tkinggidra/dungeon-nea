namespace Core.Tick
{
    public readonly struct TickContext
    {
        public ulong TickNumber { get; }
        public double TickDelta { get; }
        
        public TickContext (ulong tickNumber, double tickDelta)
        {
            TickNumber = tickNumber;
            TickDelta = tickDelta;
        }
    }
}