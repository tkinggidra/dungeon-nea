using System;
using System.Collections.Generic;

namespace Entity.Attributes
{
    public sealed class EntityAttributes
    {
        private sealed class Entry
        {
            public double BaseValue;
            public readonly Dictionary<string, AttributeModifier> Modifiers = new();

            public Entry(double baseValue)
            {
                BaseValue = baseValue;
            }
        }
        
        private readonly Dictionary<EntityAttribute, Entry> _attributes = new();

        public bool Has(EntityAttribute attribute)
        {
            return _attributes.ContainsKey(attribute);
        }

        public bool Add(EntityAttribute attribute)
        {
            if (_attributes.ContainsKey(attribute))
                return false;
            
            _attributes.Add(attribute, new Entry(attribute.DefaultValue));
            return true;
        }

        public double Get(EntityAttribute attribute)
        {
            if (!_attributes.TryGetValue(attribute, out var entry))
                throw new KeyNotFoundException(
                    "Attribute not found");
            
            double baseValue = entry.BaseValue;
            double additions = 0;
            double baseMultiplier = 0;
            double totalMultiplier = 1;

            foreach (var modifier in entry.Modifiers.Values)
            {
                switch (modifier.Operation)
                {
                    case AttributeOperation.Add:
                        additions += modifier.Amount;
                        break;
                    
                    case AttributeOperation.MultiplyBase:
                        baseMultiplier += modifier.Amount;
                        break;
                    
                    case AttributeOperation.MultiplyTotal:
                        totalMultiplier *= 1 + modifier.Amount;
                        break;
                }
            }
            
            double value = (baseValue + additions)
                + baseValue * baseMultiplier;
            
            value *= totalMultiplier;
            
            return attribute.Clamp(value);
        }

        public void Set(EntityAttribute attribute, double value)
        {
            if (!_attributes.TryGetValue(attribute, out var entry))
                throw new KeyNotFoundException(
                    "Attribute not found");

            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            entry.BaseValue = attribute.Clamp(value);
        }

        public bool AddModifier(
            EntityAttribute attribute,
            AttributeModifier modifier)
        {
            if (!_attributes.TryGetValue(attribute, out var entry))
                return false;
            
            if (entry.Modifiers.ContainsKey(modifier.Id))
                return false;
            
            entry.Modifiers.Add(modifier.Id, modifier);
            return true;
        }

        public bool RemoveModifier(
            EntityAttribute attribute,
            string modifierId)
        {
            return _attributes.TryGetValue(attribute, out var entry)
                   && entry.Modifiers.Remove(modifierId); 
        }
    }
}