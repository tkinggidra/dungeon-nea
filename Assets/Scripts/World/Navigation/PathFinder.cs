using System;
using System.Collections.Generic;
using Core.Collection;

namespace World.Navigation
{
    public sealed class PathFinder
    {
        private const int StraightCost = 10;
        private const int DiagonalCost = 14;

        private static readonly (int X, int Z)[] Directions =
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0),
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1)
        };

        private readonly NavigationGrid _grid;

        public PathFinder(NavigationGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }
        
        public List<(int X, int Z)> FindPath(
            int startX,
            int startZ,
            int goalX,
            int goalZ)
        {
            var path = new List<(int X, int Z)>();
            
            if (!_grid.IsWalkable(startX, startZ) || !_grid.IsWalkable(goalX, goalZ))
            {
                return new List<(int, int)>();
            }
            
            var start = (X: startX, Z: startZ);
            var goal  = (X: goalX, Z: goalZ);
            
            var open = new MinHeap<(int X, int Z)>();
            var closed = new HashSet<(int X, int Z)>();
            
            var cameFrom =
                new Dictionary<(int X, int Z), (int X, int Z)>();
            
            var gScore =
                new Dictionary<(int X, int Z), int>();

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
                    int nx = current.X + direction.X;
                    int nz = current.Z + direction.Z;

                    var neighbor = (X: nx, Z: nz);

                    if (!_grid.IsWalkable(nx, nz) ||
                        closed.Contains(neighbor))
                    {
                        continue;
                    }
                    
                    bool diagonal =
                        direction.X != 0 && direction.Z != 0;

                    if (diagonal &&
                        (!_grid.IsWalkable(
                             current.X + direction.X,
                             current.Z) ||
                         !_grid.IsWalkable(current.X,
                             current.Z + direction.Z)))
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
            (int X, int Z) a,
            (int X, int Z) b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dz = Math.Abs(a.Z - b.Z);
            
            int diagonal = Math.Min(dx, dz);
            int straight = Math.Max(dx, dz) - diagonal;
            
            return diagonal * DiagonalCost + straight * StraightCost;
        }

        private static List<(int X, int Z)> ReconstructedPath(
            Dictionary<(int X, int Z), (int X, int Z)> cameFrom,
            (int X, int Z) current)
        {
            var path = new List<(int X, int Z)> { current };

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