using System;

namespace World.Entity.Combat
{
    public readonly struct DamageInfo
    {
        public double Amount { get; }
        public World.Entity.Entity Source { get; }

        public DamageInfo(double amount, World.Entity.Entity source = null)
        {
            if (double.IsNaN(amount) ||
                double.IsInfinity(amount) ||
                amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }
            
            Amount = amount;
            Source = source;
        }
    }
}