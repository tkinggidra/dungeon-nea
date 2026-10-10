using System;

namespace Entity.Combat
{
    public readonly struct DamageInfo
    {
        public double Amount { get; }
        public Entity Source { get; }

        public DamageInfo(double amount, Entity source = null)
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