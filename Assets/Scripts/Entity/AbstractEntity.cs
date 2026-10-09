using System;
using Core.Tick;
using Entity.Attributes;
using World;
using Entity.Components;

namespace Entity
{
    public abstract class AbstractEntity
    {
        public EntityId EntityId { get; private set; }
        public Level Level { get; private set; }
        public EntityState State { get; private set; } =
            EntityState.Unspawned;
        public bool IsActive => State == EntityState.Active;
        
        public EntityComponents Components { get; }
        public EntityAttributes Attributes { get; } = new();

        protected AbstractEntity()
        {
            Components = new EntityComponents(this);
        }

        internal void Attach(Level level, EntityId entityId)
        {
            if (State != EntityState.Unspawned)
                throw new InvalidOperationException(
                    "Entity already spawned.");

            Level = level;
            EntityId = entityId;
            State = EntityState.Active;

            OnSpawn();
        }

        internal void TickInternal(in TickContext context)
        {
            if (!IsActive)
                return;
            
            OnTick(in context);
            
            if (IsActive)
                Components.Tick(in context);
        }

        internal void MarkForRemoval()
        {
            if (State == EntityState.Active)
                State = EntityState.PendingRemoval;
        }

        internal void Detach()
        {
            if (State != EntityState.Active && 
                State != EntityState.PendingRemoval)
                return;
            
            State = EntityState.Removed;

            try
            {
                OnRemove();
            }
            finally
            {
                try
                {
                    Components.DetachAll();
                }
                finally
                {
                    Level = null;
                }
            }
        }

        public void Remove()
        {
            if (State != EntityState.Active)
                return;

            Level?.RemoveEntity(this);
        }
        
        protected virtual void OnSpawn() { }
        protected virtual void OnTick(in TickContext context) { }
        protected virtual void OnRemove() { }
        
    }
}