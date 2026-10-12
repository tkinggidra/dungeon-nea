using System;
using System.Collections.Generic;
using System.Numerics;
using Core.Physics;

namespace World.Level.Collision
{
    public sealed class CollisionResolver
    {
        private const int SearchIterations = 20;

        private readonly List<AABB> _solids = new();
        private readonly List<AABB> _floors = new();
        
        private readonly List<AABB> _uncovered = new();
        private readonly List<AABB> _remaining = new();

        public Vector2 Resolve(
            AABB bounds,
            Vector2 movement,
            LevelGeometry geometry)
        {
            float x = ResolveAxis(
                bounds,
                movement.X,
                geometry,
                xAxis: true);
            
            bounds = bounds.Move(new Vector2(x, 0f));
            
            float z = ResolveAxis(
                bounds,
                movement.Y,
                geometry,
                xAxis: false);
            
            return new Vector2(x, z);
        }

        private float ResolveAxis(
            AABB bounds,
            float movement,
            LevelGeometry geometry,
            bool xAxis)
        {
            if (movement == 0f) 
                return 0f;
            
            const float epsilon = 0.0001f;
            
            Vector2 offset = xAxis 
                ? new Vector2(movement, 0f)
                : new Vector2(0f, movement);
            
            AABB sweptBounds = bounds.ExpandTowards(offset);
            
            geometry.GetCollision(sweptBounds, _solids);

            foreach (AABB solid in _solids)
            {
                if (xAxis)
                {
                    if (bounds.MinZ >= solid.MaxZ - epsilon ||
                        bounds.MaxZ <= solid.MinZ + epsilon)
                        continue;

                    if (movement > 0f &&
                        bounds.MaxX <= solid.MinX + epsilon)
                    {
                        float distance = MathF.Max(
                            0f,
                            solid.MinX - bounds.MaxX);

                        movement = MathF.Min(movement, distance);
                    }
                    else if (movement < 0f &&
                             bounds.MinX >= solid.MaxX - epsilon)
                    {
                        float distance = MathF.Min(
                            0f,
                            solid.MaxX - bounds.MinX);

                        movement = MathF.Max(movement, distance);
                    }
                }
                else
                {
                    if (bounds.MinX >= solid.MaxX - epsilon ||
                        bounds.MaxX <= solid.MinX + epsilon)
                        continue;

                    if (movement > 0f &&
                        bounds.MaxZ <= solid.MinZ + epsilon)
                    {
                        float distance = MathF.Max(
                            0f,
                            solid.MinZ - bounds.MaxZ);

                        movement = MathF.Min(movement, distance);
                    }
                    else if (movement < 0f &&
                             bounds.MinZ >= solid.MaxZ - epsilon)
                    {
                        float distance = MathF.Min(
                            0f,
                            solid.MaxZ - bounds.MinZ);

                        movement = MathF.Max(movement, distance);
                    }
                }
            }
            
            if (movement == 0f)
                return 0f;
            
            offset = xAxis
                ? new Vector2(movement, 0f)
                : new Vector2(0f, movement);
            
            sweptBounds = bounds.ExpandTowards(offset);
            
            geometry.GetFloors(sweptBounds, _floors);
            const float floorEpsilon = 0.0001f;
            
            if (IsCoveredByFloors(sweptBounds.Inflate(-floorEpsilon)))
                return movement;

            float low = 0f;
            float high = 1f;

            for (int i = 0; i < SearchIterations; i++)
            {
                float middle = (low + high) * 0.5f;
                
                Vector2 partialMovement = offset * middle;
                AABB partialSweep =
                    bounds.ExpandTowards(partialMovement);
                
                if (IsCoveredByFloors(partialSweep.Inflate(-floorEpsilon)))
                    low = middle;
                else
                    high = middle;
            }

            return movement * low;
        }

        private bool IsCoveredByFloors(AABB bounds)
        {
            _uncovered.Clear();
            _remaining.Clear();
            
            _uncovered.Add(bounds);
            
            List<AABB> uncovered =  _uncovered;
            List<AABB> remaining = _remaining;

            foreach (AABB floor in _floors)
            {
                remaining.Clear();

                foreach (AABB region in uncovered)
                    Subtract(region, floor, remaining);
                
                (uncovered, remaining) = (remaining, uncovered);

                if (uncovered.Count == 0)
                    return true;
            }
            
            return uncovered.Count == 0;
        }

        private static void Subtract(
            AABB region,
            AABB floor,
            List<AABB> results)
        {
            float minX = MathF.Max(region.MinX, floor.MinX);
            float minZ = MathF.Max(region.MinZ, floor.MinZ);
            float maxX = MathF.Min(region.MaxX, floor.MaxX);
            float maxZ = MathF.Min(region.MaxZ, floor.MaxZ);

            if (minX >= maxX || minZ >= maxZ)
            {
                results.Add(region);
                return;
            }

            if (region.MinX < minX)
            {
                results.Add(new AABB(
                    region.MinX,
                    region.MinZ,
                    minX,
                    region.MaxZ));
            }

            if (maxX < region.MaxX)
            {
                results.Add(new AABB(
                    maxX,
                    region.MinZ,
                    region.MaxX,
                    region.MaxZ));
            }

            if (region.MinZ < minZ)
            {
                results.Add(new AABB(
                    minX,
                    region.MinZ,
                    maxX,
                    minZ));
            }

            if (maxZ < region.MaxZ)
            {
                results.Add(new AABB(
                    minX,
                    maxZ,
                    maxX,
                    region.MaxZ));
            }
        }
    }
}