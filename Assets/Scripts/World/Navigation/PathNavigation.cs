using System;
using System.Collections.Generic;
using System.Numerics;
using Core.Tick;
using GameEntity = World.Entity.Entity;

namespace World.Navigation
{
    public sealed class PathNavigation
    {
        private readonly GameEntity _entity;

        private readonly List<(int X, int Z)> _path = new();
        private int _waypointIndex;
        private Vector3? _finalDestination;
        
        public float Speed { get; set; } = 5f;

        public bool HasPath => _waypointIndex < _path.Count;

        public PathNavigation(GameEntity entity)
        {
            _entity = entity;
        }

        public void SetPath(IReadOnlyList<(int X, int Z)> path)
        {
            ClearPath();
            
            for (int i = 0; i < path.Count; i++)
                _path.Add(path[i]);
        
        }

        public void ClearPath()
        {
            _path.Clear();
            _waypointIndex = 0;
            _finalDestination = null;
        }

        public void SetFinalDestination(Vector3 destination)
        {
            _finalDestination = destination;
        }

        public void Tick(in TickContext context)
        {
            if (!_entity.IsActive)
                return;

            float remainingMovement =
                Speed * (float)context.TickDelta;

            while (HasPath && remainingMovement > 0f)
            {
                Vector3 position = _entity.Transform.Position;
                Vector3 destination;
                
                bool followingWaypoint = _waypointIndex < _path.Count;

                if (followingWaypoint)
                {
                    var waypoint = _path[_waypointIndex];
                    
                    destination = new Vector3(
                        waypoint.X,
                        position.Y,
                        waypoint.Z);
                }
                else if (_finalDestination.HasValue)
                {
                    Vector3 final = _finalDestination.Value;
                    
                    destination = new Vector3(
                        final.X,
                        position.Y,
                        final.Z);
                }
                else
                {
                    break;
                }
                
                Vector3 displacement = destination - position;
                float distance = displacement.Length();

                if (distance <= 0.001f)
                {
                    if (followingWaypoint)
                    {
                        _waypointIndex++;
                        continue;
                    }

                    break;
                }

                float movement = Math.Min(distance, remainingMovement);
                
                _entity.Transform.Translate(
                    displacement / distance * movement);
                
                remainingMovement -= movement;
                
                if (followingWaypoint && movement >= distance)
                    _waypointIndex++;
            }
        }
    }
}