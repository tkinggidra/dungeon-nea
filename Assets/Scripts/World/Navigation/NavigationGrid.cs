using System;
using Core;

namespace World.Navigation
{
    public sealed class NavigationGrid
    {
        private readonly bool[,] _walkables;
        
        public int Width { get; }
        public int Height { get; }

        public NavigationGrid(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();
            
            Width = width;
            Height = height;
            
            _walkables = new bool[width, height];
        }

        public bool IsInBounds(BlockPos position)
        {
            return position.X >= 0 && position.X < Width &&
                   position.Z >= 0 && position.Z < Height;
        }

        public bool IsWalkable(BlockPos position)
        {
            return IsInBounds(position) && _walkables[position.X, position.Z];
        }

        public void SetWalkable(BlockPos position, bool walkable)
        {
            if (!IsInBounds(position))
                throw new ArgumentOutOfRangeException();
            
            _walkables[position.X, position.Z] = walkable;
        }
    }
}