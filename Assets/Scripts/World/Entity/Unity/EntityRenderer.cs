using System.Collections.Generic;

namespace World.Entity.Unity
{
    public sealed class EntityRenderer
    {
        private readonly List<EntityBehaviour> _entityBehaviours = new();

        public void Register(EntityBehaviour entityBehaviour)
        {
            if (_entityBehaviours.Contains(entityBehaviour))
                return;
            
            _entityBehaviours.Add(entityBehaviour);
        }

        public void Unregister(EntityBehaviour entityBehaviour)
        {
            _entityBehaviours.Remove(entityBehaviour);
        }

        public void Render(float alpha)
        {
            foreach (var entityBehaviour in _entityBehaviours)
                entityBehaviour.Render(alpha);
        }
    }
}