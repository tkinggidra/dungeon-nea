using System.Collections.Generic;
using Core.Tick;
using Entity;
using World.Player;

namespace World
{
    public sealed class Level : ITickable
    {
        private readonly Dictionary<EntityId, Entity.Entity> _entities = new();
        private readonly List<Entity.Entity> _tickOrder = new();
        
        private readonly List<Entity.Entity> _pendingAdditions = new();
        private readonly HashSet<EntityId> _pendingRemovals = new();

        private int nextEntityId = 1;
        private bool isTicking;
        
        public ulong AgeTicks { get; private set; }
        
        public int EntityCount => _entities.Count;

        public bool AddEntity(Entity.Entity entity)
        {
            if (entity.State != EntityState.Unspawned)
                return false;
            
            if (_pendingAdditions.Contains(entity))
                return false;

            if (isTicking)
            {
                _pendingAdditions.Add(entity);
                return true;
            }

            AddImmediately(entity);
            return true;
        }

        public Entity.Player AddPlayer(PlayerInput playerInput)
        {
            var player = new Entity.Player(playerInput);
            player.AddFlag(LivingEntityFlags.Invulnerable);
            
            var playerAdded = AddEntity(player);
            return playerAdded ? player : null;
        }

        public bool RemoveEntity(Entity.Entity entity)
        {
            if (!_entities.ContainsKey(entity.EntityId))
                return false;

            if (isTicking)
            {
                if (!_pendingRemovals.Add(entity.EntityId))
                    return false;
                
                entity.MarkForRemoval();
                return true;
            }
            
            RemoveImmediately(entity.EntityId);
            return true;
        }

        public bool TryGetEntity(EntityId entityId, out Entity.Entity entity)
        {
            return _entities.TryGetValue(entityId, out entity);
        }

        public void Tick(in TickContext context)
        {
            AgeTicks++;
            isTicking = true;

            try
            {
                foreach (Entity.Entity entity in _tickOrder)
                {
                    if (_pendingRemovals.Contains(entity.EntityId))
                        continue;

                    entity.TickInternal(in context);
                }
            }
            finally
            {
                isTicking = false;
                ApplyPendingChanges();
            }
        }

        private void AddImmediately(Entity.Entity entity)
        {
            var id = new EntityId(nextEntityId++);
            
            _entities.Add(id, entity);
            _tickOrder.Add(entity);
            
            entity.Attach(this, id);
        }

        private void RemoveImmediately(EntityId entityId)
        {
            if (!_entities.TryGetValue(entityId, out var entity))
                return;
            
            _entities.Remove(entityId);
            _tickOrder.Remove(entity);
            
            entity.Detach();
        }

        private void ApplyPendingChanges()
        {
            foreach(EntityId entityId in _pendingRemovals)
                RemoveImmediately(entityId);
            
            _pendingRemovals.Clear();

            foreach (Entity.Entity entity in _pendingAdditions)
                AddImmediately(entity);
            
            _pendingAdditions.Clear();
        }
    }
}