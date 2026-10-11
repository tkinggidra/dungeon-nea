using System;
using System.Collections.Generic;
using Core.Tick;
using World.Entity;
using World.Player;
using GameEntity = World.Entity.Entity;
using GamePlayer = World.Entity.Player;

namespace World.Level
{
    public sealed class Level : ITickable
    {
        private readonly Dictionary<EntityId, GameEntity> _entities = new();
        private readonly List<GameEntity> _entityOrder = new();
        private readonly List<GameEntity> _tickOrder = new();
        private readonly List<GamePlayer> _players = new();
        
        private readonly List<GameEntity> _pendingAdditions = new();
        private readonly HashSet<EntityId> _pendingRemovals = new();

        private int _nextEntityId = 1;
        private bool _isTicking;
        
        private const int SimulationRefreshInterval = 5;
        
        public ulong AgeTicks { get; private set; }
        
        public float SimulationDistance { get; set; } = 64f;
        
        public int EntityCount => _entities.Count;
        public int PlayerCount => _players.Count;
        public int TickingEntityCount => _tickOrder.Count;

        public IReadOnlyList<GamePlayer> Players => _players;
        
        public bool AddEntity(GameEntity entity)
        {
            if (entity.State != EntityState.Unspawned)
                return false;
            
            if (_pendingAdditions.Contains(entity))
                return false;

            if (_isTicking)
            {
                _pendingAdditions.Add(entity);
                return true;
            }

            AddImmediately(entity);
            RefreshEntitySimulation();
            return true;
        }

        public GamePlayer AddPlayer(PlayerInput playerInput)
        {
            var player = new GamePlayer(playerInput);
            player.AddFlag(LivingEntityFlags.Invulnerable);
            
            return AddEntity(player) ? player : null;
        }

        public bool RemoveEntity(GameEntity entity)
        {
            if (!_entities.TryGetValue(entity.EntityId, out var registered) ||
                !ReferenceEquals(registered, entity))
            {
                return false;
            }

            if (_isTicking)
            {
                if (!_pendingRemovals.Add(entity.EntityId))
                    return false;
                
                entity.MarkForRemoval();
                return true;
            }
            
            RemoveImmediately(entity.EntityId);
            RefreshEntitySimulation();
            return true;
        }

        public bool TryGetEntity(EntityId entityId, out GameEntity entity)
        {
            return _entities.TryGetValue(entityId, out entity);
        }

        public bool IsEntityTicking(GameEntity entity)
        {
            return _tickOrder.Contains(entity) && 
                   !_pendingRemovals.Contains(entity.EntityId);
        }

        public void Tick(in TickContext context)
        {
            if (_isTicking)
                throw new InvalidOperationException(
                    "Cannot tick level recursively");
            
            AgeTicks++;

            if (AgeTicks == 1 ||
                AgeTicks % SimulationRefreshInterval == 0)
            {
                RefreshEntitySimulation();
            }
            
            _isTicking = true;

            try
            {
                foreach (GameEntity entity in _tickOrder)
                {
                    if (_pendingRemovals.Contains(entity.EntityId))
                        continue;

                    entity.TickInternal(in context);
                }
            }
            finally
            {
                _isTicking = false;
                ApplyPendingChanges();
            }
        }

        private void AddImmediately(GameEntity entity)
        {
            var id = new EntityId(_nextEntityId++);
            
            _entities.Add(id, entity);
            _entityOrder.Add(entity);
            
            if (entity is GamePlayer player)
                _players.Add(player);
            
            entity.Attach(this, id);
        }

        private void RemoveImmediately(EntityId entityId)
        {
            if (!_entities.TryGetValue(entityId, out var entity))
                return;
            
            _entities.Remove(entityId);
            _entityOrder.Remove(entity);
            _tickOrder.Remove(entity);
            
            if (entity is GamePlayer player) 
                _players.Remove(player);
            
            entity.Detach();
        }

        private void RefreshEntitySimulation()
        {
            _tickOrder.Clear();

            foreach (GameEntity entity in _entityOrder)
            {
                if (_pendingRemovals.Contains(entity.EntityId))
                    continue;

                if (entity is GamePlayer ||
                    IsWithinSimulationDistance(entity))
                {
                    _tickOrder.Add(entity);
                }
            }
        }

        private bool IsWithinSimulationDistance(GameEntity entity)
        {
            var radiusSquared = SimulationDistance * SimulationDistance;

            foreach (var player in _players)
            {
                if (_pendingRemovals.Contains(player.EntityId))
                    continue;
                
                if (entity.Transform.DistanceSquaredXZ(player.Transform)
                    <= radiusSquared)
                {
                    return true;
                }
            }
            
            return false;
        }

        private void ApplyPendingChanges()
        {
            bool changed = _pendingRemovals.Count > 0 ||
                           _pendingAdditions.Count > 0;
            
            foreach(EntityId entityId in _pendingRemovals)
                RemoveImmediately(entityId);
            
            _pendingRemovals.Clear();

            foreach (GameEntity entity in _pendingAdditions)
                AddImmediately(entity);
            
            _pendingAdditions.Clear();
            
            if (changed)
                RefreshEntitySimulation();
        }
    }
}