using System;
using System.Collections.Generic;
using Core.Tick;

namespace Entity.Components
{
    public sealed class EntityComponents
    {
        private readonly Entity _owner;
        
        private readonly Dictionary<Type, IEntityComponent> _components = new();
        private readonly List<ITickingComponent> _tickingComponents = new();
        
        public int Count => _components.Count;

        public EntityComponents(Entity owner)
        {
            this._owner = owner;
        }

        public bool Add<T>(T component) where T : class, IEntityComponent
        {
            Type type = component.GetType();
            
            if (_components.ContainsKey(type))
                return false;
            
            _components.Add(type, component);
            
            ITickingComponent ticking = component as ITickingComponent;
            
            if (ticking != null)
                _tickingComponents.Add(ticking);

            try
            {
                component.OnAttach(_owner);
            }
            catch
            {
                _components.Remove(type);
                
                if (ticking != null)
                    _tickingComponents.Remove(ticking);
                
                throw;
            }
            
            return true;
        }

        public T Get<T>() where T : class, IEntityComponent
        {
            return TryGet<T>(out T component) ? component : null;
        }

        public bool TryGet<T>(out T component)
            where T : class, IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out var found))
            {
                component = (T)found;
                return true;
            }
            
            component = null;
            return false;
        }
        
        public bool Has<T>() where T : class, IEntityComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public bool Remove<T>() where T : class, IEntityComponent
        {
            if (!_components.TryGetValue(typeof(T), out var component))
                return false;
            
            _components.Remove(typeof(T));
            
            if (component is ITickingComponent ticking)
                _tickingComponents.Remove(ticking);
            
            component.OnDetach();
            return true;
        }

        internal void Tick(in TickContext context)
        {
            ITickingComponent[] snapshot = _tickingComponents.ToArray();

            foreach (ITickingComponent component in snapshot)
            {
                if (_tickingComponents.Contains(component))
                    component.Tick(context);
            }
        }

        internal void DetachAll()
        {
            IEntityComponent[] snapshot = new IEntityComponent[_components.Count];
            _components.Values.CopyTo(snapshot, 0);

            foreach (IEntityComponent component in snapshot)
            {
                Type type = component.GetType();
                
                if (_components.TryGetValue(type, out var current) &&
                    ReferenceEquals(current, component))
                {
                    _components.Remove(type);
                        
                    if (component is ITickingComponent ticking)
                        _tickingComponents.Remove(ticking);
                        
                    component.OnDetach();
                }
            }
        }
    }
}