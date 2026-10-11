using World.Entity.Combat;
using World.Entity.Components.Component;

namespace World.Entity
{
    public abstract class LivingEntity : World.Entity.Entity
    {
        public LivingEntityFlags Flags { get; private set; }
        
        public bool IsInvulnerable =>
            HasFlag(LivingEntityFlags.Invulnerable);
        
        public bool IsDead =>
            Components.Get<HealthComponent>()?.IsDead ?? false;

        protected LivingEntity()
        {
            Components.Add(new HealthComponent());
        }
        
        public void Damage(in DamageInfo damage)
        {
            if (IsInvulnerable || IsDead)
                return;
            
            Components.Get<HealthComponent>()?.Damage(in damage);
            
            if (IsDead)
                OnDeath();
        }

        public void Heal(double amount)
        {
            Components.Get<HealthComponent>()?.Heal(amount);
        }
        
        public bool HasFlag(LivingEntityFlags flag)
        {
            return (Flags & flag) == flag;
        }

        public void AddFlag(LivingEntityFlags flag)
        {
            Flags |= flag;
        }

        public void RemoveFlag(LivingEntityFlags flag)
        {
            Flags &= ~flag;
        }
        
        protected virtual void OnDeath() { }
    }
}