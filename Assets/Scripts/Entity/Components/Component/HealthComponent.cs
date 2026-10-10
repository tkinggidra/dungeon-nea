
using System;
using Entity.Attributes;
using Entity.Combat;

namespace Entity.Components.Component
{
    public sealed class HealthComponent : IEntityComponent
    {
        private Entity _owner;
        
        public double CurrentHealth { get; private set; }

        public double MaxHealth =>
            _owner.Attributes.Get(AttributeTypes.MaxHealth);

        public bool IsDead => CurrentHealth <= 0;
        
        public void OnAttach(Entity entity)
        {
            _owner = entity;
            
            if (!_owner.Attributes.Has(AttributeTypes.MaxHealth))
                _owner.Attributes.Add(AttributeTypes.MaxHealth);

            CurrentHealth = MaxHealth;
        }

        public void OnDetach()
        {
            _owner = null;
        }

        public void Damage(in DamageInfo damage)
        {
            if (IsDead)
                return;
            
            CurrentHealth = Math.Max(
                0,
                CurrentHealth - damage.Amount);
        }

        public void Heal(double amount)
        {
            if (double.IsNaN(amount) ||
                double.IsInfinity(amount) ||
                amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }
            
            if (IsDead)
                return;

            CurrentHealth = Math.Min(
                CurrentHealth + amount,
                MaxHealth);
        }
    }
}