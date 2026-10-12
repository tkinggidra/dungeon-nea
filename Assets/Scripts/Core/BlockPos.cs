using System;
using System.Numerics;

namespace Core
{
    public readonly struct BlockPos
    {
        public int X { get; }
        public int Z { get; }

        public BlockPos(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static BlockPos FromWorldPosition(Vector3 position)
        {
            return new BlockPos(
                (int)MathF.Round(position.X),
                (int)MathF.Round(position.Z));
        }

        public BlockPos Offset(int x, int z)
        {
            return new BlockPos(X + x, Z + z);
        }

        public int DistanceSquared(BlockPos other)
        {
            int dx =  X - other.X;
            int dz =  Z - other.Z;
            
            return dx * dx + dz * dz;
        }

        public bool IsAdjacent(BlockPos other)
        {
            int dx = Math.Abs(X - other.X);
            int dz = Math.Abs(Z - other.Z);
            
            return dx <= 1  && dz <= 1 && (dx != 0 || dz != 1);
        }

        public bool Equals(BlockPos other)
        {
            return other.X == X && other.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is BlockPos other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }

        public static bool operator ==(BlockPos left, BlockPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockPos left, BlockPos right)
        {
            return !left.Equals(right);
        }
        
        public override string ToString()
        {
            return $"BlockPos({X}, {Z})";
        }
    }
}