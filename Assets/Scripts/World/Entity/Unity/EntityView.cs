using System;
using System.Collections.Generic;
using Core.Tick;
using UnityEngine;
using GameLevel = World.Level.Level;

namespace World.Entity.Unity
{
    public sealed class EntityView : IDisposable
    {
        private readonly GameLevel _level;
        private readonly TickScheduler _tickScheduler;
        private readonly EntityRenderer _entityRenderer;

        private readonly PlayerBehaviour _playerPrefab;
        private readonly EntityBehaviour _entityPrefab;
        private readonly Transform _parent;
        
        private readonly Dictionary<Entity, EntityBehaviour> _views = new();

        public EntityView(
            GameLevel level,
            TickScheduler tickScheduler,
            EntityRenderer entityRenderer,
            PlayerBehaviour playerPrefab,
            EntityBehaviour entityPrefab,
            Transform parent)
        {
            _level = level;
            _tickScheduler = tickScheduler;
            _entityRenderer = entityRenderer;
            _playerPrefab = playerPrefab;
            _entityPrefab = entityPrefab;
            _parent = parent;

            _level.EntityAdded += OnEntityAdded;
            _level.EntityRemoved += OnEntityRemoved;
        }

        private void OnEntityAdded(Entity entity)
        {
            if (_views.ContainsKey(entity))
                return;
            
            EntityBehaviour prefab = entity is Player
                ? _playerPrefab
                : _entityPrefab;
            
            EntityBehaviour view = UnityEngine.Object.Instantiate(prefab, _parent);
            
            view.Initialize(entity, _tickScheduler);
            
            _views.Add(entity, view);
            _entityRenderer.Register(view);
        }

        private void OnEntityRemoved(Entity entity)
        {
            if (!_views.TryGetValue(entity, out EntityBehaviour view))
                return;
            
            _entityRenderer.Unregister(view);
            _views.Remove(entity);
            
            UnityEngine.Object.Destroy(view.gameObject);
        }

        public void Dispose()
        {
            _level.EntityAdded -= OnEntityAdded;
            _level.EntityRemoved -= OnEntityRemoved;

            foreach (var view in _views.Values)
            {
                _entityRenderer.Unregister(view);
                UnityEngine.Object.Destroy(view.gameObject);
            }
            
            _views.Clear();
        }
    }
}