using System;
using System.Numerics;
using Core.Physics;
using Core.Tick;
using World.Entity.Attributes;
using World.Entity.Components;
using World.Entity.Components.Component;
using GameLevel = World.Level.Level;

namespace World.Entity
{
    public abstract class Entity
    {
        public EntityId EntityId { get; private set; }
        public GameLevel Level { get; private set; }
        public EntityState State { get; private set; } =
            EntityState.Unspawned;
        public bool IsActive => State == EntityState.Active;

        public virtual float Width => 0.6f;
        public virtual float Depth => 0.6f;
        public AABB BoundingBox => AABB.FromCenter(
            new Vector2(Transform.Position.X, Transform.Position.Z),
            Width,
            Depth);
        
        public EntityComponents Components { get; }
        public EntityAttributes Attributes { get; } = new();
        
        public TransformComponent Transform =>
            Components.Get<TransformComponent>();

        protected Entity()
        {
            Components = new EntityComponents(this);
            Components.Add(new TransformComponent());
        }

        internal void Attach(GameLevel level, EntityId entityId)
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

        public void Move(Vector3 displacement)
        {
            Vector3 resolved = Level.ResolveMovement(
                BoundingBox,
                displacement);

            if (displacement.LengthSquared() > 0f)
            {
                UnityEngine.Debug.Log(
                    $"Position: {Transform.Position} | " +
                    $"Bounds: {BoundingBox} | " +
                    $"Requested: {displacement} | " +
                    $"Resolved: {resolved}");
            }

            Transform.Translate(resolved);
        }
        
        protected virtual void OnSpawn() { }
        protected virtual void OnTick(in TickContext context) { }
        protected virtual void OnRemove() { }
        
    }
}