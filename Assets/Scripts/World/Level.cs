using System.Collections.Generic;
using Core.Tick;
using Entity;

namespace World
{
    public sealed class Level : ITickable
    {
        private readonly Dictionary<EntityId, AbstractEntity> _entities = new();
        private readonly List<AbstractEntity> _tickOrder = new();
        
        private readonly List<AbstractEntity> _pendingAdditions = new();
        private readonly HashSet<EntityId> _pendingRemovals = new();

        private int nextEntityId = 1;
        private bool isTicking;
        
        public ulong AgeTicks { get; private set; }
        
        public int EntityCount => _entities.Count;

        public bool AddEntity(AbstractEntity entity)
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

        public bool RemoveEntity(AbstractEntity entity)
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

        public bool TryGetEntity(EntityId entityId, out AbstractEntity entity)
        {
            return _entities.TryGetValue(entityId, out entity);
        }

        public void Tick(in TickContext context)
        {
            AgeTicks++;
            isTicking = true;

            try
            {
                foreach (AbstractEntity entity in _tickOrder)
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

        private void AddImmediately(AbstractEntity entity)
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

            foreach (AbstractEntity entity in _pendingAdditions)
                AddImmediately(entity);
            
            _pendingAdditions.Clear();
        }
    }
}