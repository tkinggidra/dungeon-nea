using System.Collections.Generic;
using System.Numerics;
using Core;
using Core.Physics;

namespace World.Level
{
    public sealed class LevelGeometry
    {
        private readonly SpatialHash _floors = new();
        private readonly SpatialHash _solids = new();

        public void AddFloor(AABB bounds)
        {
            _floors.Add(bounds);
        }

        public void AddSolid(AABB bounds)
        {
            _solids.Add(bounds);
        }

        public bool HasFloor(Vector2 position)
        {
            return _floors.Contains(position);
        }

        public bool IsWalkable(BlockPos position)
        {
            Vector2 center = new(position.X, position.Z);
            
            return _floors.Contains(center) &&
                   !_solids.Contains(center);
        }

        public bool IntersectsSolid(AABB bounds)
        {
            return _solids.Intersects(bounds);
        }

        public void GetCollision(
            AABB searchBounds,
            List<AABB> results)
        {
            _solids.Query(searchBounds, results);
        }

        public void GetFloors(
            AABB searchBounds,
            List<AABB> results)
        {
            _floors.Query(searchBounds, results);
        }
    }
}