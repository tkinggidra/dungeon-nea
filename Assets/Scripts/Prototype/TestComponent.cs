using Core.Tick;
using Entity;
using Entity.Components;
using UnityEngine;

namespace Prototype
{
    public sealed class TestComponent : ITickingComponent
    {
        private AbstractEntity owner;
        private int ticks;

        public void OnAttach(AbstractEntity entity)
        {
            owner = entity;
            Debug.Log($"Component attached to entity {entity.EntityId}");
        }

        public void Tick(in TickContext context)
        {
            ticks++;
        }

        public void OnDetach()
        {
            Debug.Log("Component detached!");
            owner = null;
        }
    }
}