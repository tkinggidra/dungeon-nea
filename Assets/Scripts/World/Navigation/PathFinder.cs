using System;
using System.Collections.Generic;
using Core;
using Core.Collection;

namespace World.Navigation
{
    public sealed class PathFinder
    {
        private const int StraightCost = 10;
        private const int DiagonalCost = 14;

        private static readonly BlockPos[] Directions =
        {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0),
            new(1, 1),
            new(1, -1),
            new(-1, 1),
            new(-1, -1)
        };

        private readonly NavigationGrid _grid;

        public PathFinder(NavigationGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }
        
        public List<BlockPos> FindPath(
            BlockPos start,
            BlockPos goal)
        {
            var path = new List<BlockPos>();
            
            if (!_grid.IsWalkable(start) || !_grid.IsWalkable(goal))
            {
                return path;
            }
            
            var open = new PriorityQueue<BlockPos>();
            var closed = new HashSet<BlockPos>();
            
            var cameFrom =
                new Dictionary<BlockPos, BlockPos>();
            
            var gScore =
                new Dictionary<BlockPos, int>();

            gScore[start] = 0;
            open.Enqueue(start, Heuristic(start, goal));

            while (open.Count > 0)
            {
                var current = open.Dequeue();
                
                if (!closed.Add(current))
                    continue;
                
                if (current == goal)
                    return ReconstructedPath(cameFrom, current);

                foreach (var direction in Directions)
                {
                    BlockPos neighbor = current.Offset(
                        direction.X, direction.Z);

                    if (!_grid.IsWalkable(neighbor) ||
                        closed.Contains(neighbor))
                    {
                        continue;
                    }
                    
                    bool diagonal =
                        direction.X != 0 && direction.Z != 0;

                    if (diagonal &&
                        (!_grid.IsWalkable(current.Offset(direction.X, 0)) ||
                         !_grid.IsWalkable(current.Offset(0, direction.Z))))
                    {
                        continue;
                    }
                    
                    int movementCost = diagonal
                        ? DiagonalCost
                        : StraightCost;
                    
                    int tentativeG = 
                        gScore[current] + movementCost;

                    if (gScore.TryGetValue(
                            neighbor, out int existingG) &&
                        tentativeG >= existingG)
                    {
                        continue;
                    }
                    
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    
                    int fScore =
                        tentativeG + Heuristic(neighbor, goal);
                    
                    open.Enqueue(neighbor, fScore);
                }
            }

            return path;
        }

        private static int Heuristic(
            BlockPos a,
            BlockPos b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dz = Math.Abs(a.Z - b.Z);
            
            int diagonal = Math.Min(dx, dz);
            int straight = Math.Max(dx, dz) - diagonal;
            
            return diagonal * DiagonalCost + straight * StraightCost;
        }

        private static List<BlockPos> ReconstructedPath(
            Dictionary<BlockPos, BlockPos> cameFrom,
            BlockPos current)
        {
            var path = new List<BlockPos> { current };

            while (cameFrom.TryGetValue(current, out var previous))
            {
                current = previous;
                path.Add(current);
            }
            
            path.Reverse();
            return path;
        }
    }
}