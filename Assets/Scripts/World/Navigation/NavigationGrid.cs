using System;

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

        public bool IsInBounds(int x, int z)
        {
            return x >= 0 && x < Width &&
                   z >= 0 && z < Height;
        }

        public bool IsWalkable(int x, int z)
        {
            return IsInBounds(x, z) && _walkables[x, z];
        }

        public void SetWalkable(int x, int z, bool walkable)
        {
            if (!IsInBounds(x, z))
                throw new ArgumentOutOfRangeException();
            
            _walkables[x, z] = walkable;
        }
    }
}