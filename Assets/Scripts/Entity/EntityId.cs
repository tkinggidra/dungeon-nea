using System;

namespace Entity
{
    public readonly struct EntityId : IEquatable<EntityId>
    {
        public int Value { get; }

        public EntityId(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            
            Value = value;
        }
        
        public bool Equals(EntityId other) => Value == other.Value;
        public override bool Equals(object obj) =>
            obj is EntityId other && Equals(other);
        
        public override int GetHashCode() => Value;
        
        public override string ToString() => Value.ToString();
        
        public static bool operator ==(EntityId left, EntityId right) =>
            left.Equals(right);

        public static bool operator !=(EntityId left, EntityId right) =>
            !left.Equals(right);
    }
}