
using Core.Tick;
using Entity;
using Entity.Attributes;
using UnityEngine;

using GameAttributes = Entity.Attributes.Attributes;

namespace Prototype
{
    public sealed class TestEntity : AbstractEntity
    {
        private int age;

        protected override void OnSpawn()
        {
            Debug.Log($"Entity {EntityId} spawned!");

            // Register attributes
            Attributes.Add(GameAttributes.MaxHealth);
            Attributes.Add(GameAttributes.MovementSpeed);

            // Set base health
            Attributes.Set(GameAttributes.MaxHealth, 100);

            Debug.Log(
                $"Base health: {Attributes.Get(GameAttributes.MaxHealth)}"
            );

            // Flat +20 health
            Attributes.AddModifier(
                GameAttributes.MaxHealth,
                new AttributeModifier(
                    "flat_bonus",
                    20,
                    AttributeOperation.Add
                )
            );

            Debug.Log(
                $"After +20: {Attributes.Get(GameAttributes.MaxHealth)}"
            );

            // Multiply total by 1.5
            Attributes.AddModifier(
                GameAttributes.MaxHealth,
                new AttributeModifier(
                    "health_boost",
                    0.5,
                    AttributeOperation.MultiplyTotal
                )
            );

            Debug.Log(
                $"After 50% boost: {Attributes.Get(GameAttributes.MaxHealth)}"
            );

            // Remove flat modifier
            Attributes.RemoveModifier(
                GameAttributes.MaxHealth,
                "flat_bonus"
            );

            Debug.Log(
                $"After removing +20: {Attributes.Get(GameAttributes.MaxHealth)}"
            );

            // Remove multiplier
            Attributes.RemoveModifier(
                GameAttributes.MaxHealth,
                "health_boost"
            );

            Debug.Log(
                $"Restored health: {Attributes.Get(GameAttributes.MaxHealth)}"
            );
        }

        protected override void OnTick(in TickContext context)
        {
            age++;

            if (age >= 100)
                Remove();
        }

        protected override void OnRemove()
        {
            Debug.Log($"Entity {EntityId} removed!");
        }
    }
}
