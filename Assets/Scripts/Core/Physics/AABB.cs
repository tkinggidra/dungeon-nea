using System;
using System.Numerics;

namespace Core.Physics
{
    public readonly struct AABB
    {
        public float MinX { get; }
        public float MinZ { get; }
        public float MaxX { get; }
        public float MaxZ { get; }

        public float Width => MaxX - MinX;
        public float Depth => MaxZ - MinZ;

        public AABB(
            float minX,
            float minZ,
            float maxX,
            float maxZ)
        {
            if (minX > maxX || minZ > maxZ)
                throw new ArgumentException(
                    "min > max");
            
            MinX = minX;
            MinZ = minZ;
            MaxX = maxX;
            MaxZ = maxZ;
        }

        public static AABB FromCenter(
            Vector2 center,
            float width,
            float depth)
        {
            if (width < 0f || depth < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(width));
            
            float halfWidth = width * 0.5f;
            float halfDepth = depth * 0.5f;
            
            return new AABB(
                center.X - halfWidth,
                center.Y - halfDepth,
                center.X + halfWidth,
                center.Y + halfDepth);
        }

        public bool Intersects(AABB other)
        {
            return MinX < other.MaxX &&
                   MaxX > other.MinX &&
                   MinZ < other.MaxZ &&
                   MaxZ > other.MinZ;
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= MinX &&
                   point.X <= MaxX &&
                   point.Y >= MinZ  &&
                   point.Y <= MaxZ;
         }

        public AABB Move(Vector2 offset)
        {
            return new AABB(
                MinX + offset.X,
                MinZ + offset.Y,
                MaxX + offset.X,
                MaxZ + offset.Y);
        }

        public AABB ExpandTowards(Vector2 movement)
        {
            return new AABB(
                movement.X < 0f ? MinX + movement.X : MinX,
                movement.Y < 0f ? MinZ + movement.Y : MinZ,
                movement.X > 0f ? MaxX + movement.X : MaxX,
                movement.Y > 0f ? MaxZ + movement.Y : MaxZ);
        }

        public AABB Inflate(float amount)
        {
            return new AABB(
                MinX - amount,
                MinZ - amount,
                MaxX + amount,
                MaxZ + amount);
        }

        public override string ToString()
        {
            return $"AABB(({MinX}, {MinZ}) -> ({MaxX}, {MaxZ})";
        }
    }
}