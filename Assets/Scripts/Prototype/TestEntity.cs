using Core.Tick;
using Entity;
using UnityEngine;

namespace Prototype
{
    public sealed class TestEntity : AbstractEntity
    {
        private int age;

        protected override void OnSpawn()
        {
            Debug.Log($"Entity {EntityId} spawned!");
            Components.Add(new TestComponent());
        }

        protected override void OnTick(in TickContext context)
        {
            age++;

            if (age % 20 == 0)
                Debug.Log($"Entity {EntityId} is {age} ticks old.");
            
            if (age >= 100)
                Remove();
        }

        protected override void OnRemove()
        {
            Debug.Log($"Entity {EntityId} removed!");
        }
    }
}