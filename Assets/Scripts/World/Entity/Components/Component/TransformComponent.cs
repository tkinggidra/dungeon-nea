using System.Numerics;

namespace World.Entity.Components.Component
{
    public sealed class TransformComponent : IEntityComponent
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformComponent(
            Vector3 position = default,
            Quaternion rotation = default)
        {
            Position = position;
            Rotation = rotation == default
                ? Quaternion.Identity
                : Quaternion.Normalize(rotation);
        }
        
        public void SetPosition(in Vector3 position)
        {
            Position = position;
        }

        public void SetRotation(in Quaternion rotation)
        {
            Rotation = Quaternion.Normalize(rotation);
        }

        public void Translate(in Vector3 displacement)
        {
            Position += displacement;
        }

        public float DistanceSquaredTo(TransformComponent other)
        {
            return Vector3.DistanceSquared(Position, other.Position);
        }

        public float DistanceSquaredXZ(TransformComponent other)
        {
            return Vector2.DistanceSquared(
                new Vector2(Position.X, Position.Z),
                new Vector2(other.Position.X, other.Position.Z));
        }

        public void OnAttach(Entity entity) { }
        public void OnDetach() { }
    }
}