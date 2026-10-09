using System;

namespace Entity.Attributes
{
    public sealed class EntityAttribute
    {
        public string Id { get; }
        public double DefaultValue { get; }
        public double MinValue { get; }
        public double MaxValue { get; }

        public EntityAttribute(
            string id,
            double defaultValue,
            double minValue,
            double maxValue)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(
                    "Attribute id cannot be empty");
            
            if (minValue > maxValue)
                throw new ArgumentException("Min value cannot be greater than max value");
            
            if (double.IsNaN(defaultValue) || defaultValue < minValue || defaultValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(defaultValue));
            
            Id = id;
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public double Clamp(double value)
        {
            return Math.Max(MinValue, Math.Min(MaxValue, value));
        }
    }
}