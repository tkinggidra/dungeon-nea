using System;
using System.Collections.Generic;
using System.Numerics;

namespace Core.Physics
{
    public sealed class SpatialHash
    {
        private readonly float _cellSize;

        private readonly List<AABB> _bounds = new();
        
        private readonly Dictionary<BlockPos, List<int>> _cells = new();

        private readonly HashSet<int> _visited = new();
        
        public int Count => _bounds.Count;

        public SpatialHash(float cellSize = 16f)
        {
            if (cellSize <= 0f ||
                float.IsNaN(cellSize) ||
                float.IsInfinity(cellSize))
            {
                throw new ArgumentOutOfRangeException(nameof(cellSize));
            }
            
            _cellSize = cellSize;
        }

        public void Add(AABB bounds)
        {
            if (bounds.Width <= 0f || bounds.Depth <= 0f)
                throw new ArgumentOutOfRangeException(nameof(bounds));

            int index = _bounds.Count;
            _bounds.Add(bounds);
            
            GetCellRange(
                bounds,
                out int minX,
                out int minZ,
                out int maxX,
                out int maxZ);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    BlockPos cell = new (x, z);

                    if (!_cells.TryGetValue(cell, out var entries))
                    {
                        entries = new();
                        _cells.Add(cell, entries);
                    }
                    
                    entries.Add(index);
                }
            }
        }

        public bool Contains(Vector2 point)
        {
            BlockPos cell = ToCell(point.X, point.Y);
            
            if (!_cells.TryGetValue(cell, out var entries))
                return false;

            foreach (int index in entries)
            {
                if (_bounds[index].Contains(point))
                    return true;
            }
            
            return false;
        }

        public bool Intersects(AABB bounds)
        {
            GetCellRange(
                bounds,
                out int minX,
                out int minZ,
                out int maxX,
                out int maxZ);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (!_cells.TryGetValue(
                            new BlockPos(x, z), out var entries))
                        continue;

                    foreach (int index in entries)
                    {
                        if (_bounds[index].Intersects(bounds))
                            return true;
                    }
                }
            }
            
            return false;
        }
        
        public void Query(AABB searchBounds, List<AABB> results)
        {
            results.Clear();
            _visited.Clear();

            GetCellRange(
                searchBounds,
                out int minX,
                out int minZ,
                out int maxX,
                out int maxZ);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (!_cells.TryGetValue(
                            new BlockPos(x, z), out var entries))
                        continue;

                    foreach (int index in entries)
                    {
                        if (!_visited.Add(index))
                            continue;
                        
                        AABB candidate = _bounds[index];
                        
                        if (candidate.Intersects(searchBounds))
                            results.Add(candidate);
                    }
                }
            }
        }

        private BlockPos ToCell(float x, float z)
        {
            return new BlockPos(
                (int)MathF.Floor(x / _cellSize),
                (int)MathF.Floor(z / _cellSize));
        }

        private void GetCellRange(
            AABB searchBounds,
            out int minX,
            out int minZ,
            out int maxX,
            out int maxZ)
        {
            minX = (int)MathF.Floor(searchBounds.MinX / _cellSize);
            minZ = (int)MathF.Floor(searchBounds.MinZ / _cellSize);
            maxX = (int)MathF.Floor(searchBounds.MaxX / _cellSize);
            maxZ = (int)MathF.Floor(searchBounds.MaxZ / _cellSize);
        }
    }
}