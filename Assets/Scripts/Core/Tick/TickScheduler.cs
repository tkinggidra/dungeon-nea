using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Tick
{
    public sealed class TickScheduler
    {
        private const int TicksPerSecond = 20;
        private const double TickInterval = 1.0 / TicksPerSecond;

        private const int MaxTicksPerFrame = 5;
        
        private readonly List<ITickable> _tickables = new();
        private readonly HashSet<ITickable> _registered = new();

        private readonly List<ITickable> _pendingAdditions = new();
        private readonly HashSet<ITickable> _pendingRemovals = new();

        private double _accumulator;
        private bool _isTicking;

        public ulong TickNumber { get; private set; }
        public ulong DroppedTicks { get; private set; }

        public double InterpolationAlpha =>
            Math.Max(0.0, Math.Min(1.0, _accumulator / TickInterval));

        public void Advance(double deltaTime)
        {
            if (double.IsNaN(deltaTime) ||
                double.IsInfinity(deltaTime) ||
                deltaTime < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            } 
            
            if (_isTicking)
                throw new InvalidOperationException(
                    "Scheduler is already ticking.");
            
            _accumulator += deltaTime;
            int ticksThisFrame = 0;

            while (_accumulator >= TickInterval &&
                   ticksThisFrame < MaxTicksPerFrame)
            {
                _accumulator -= TickInterval;
                RunTick();
                ticksThisFrame++;
            }

            if (_accumulator >= TickInterval)
            {
                ulong dropped = (ulong)(_accumulator / TickInterval);
                
                DroppedTicks = dropped;
                _accumulator %= TickInterval;
            }
        }

        public bool Register(ITickable tickable)
        {
            if (!_isTicking)
                return Add(tickable);
            
            if (_pendingRemovals.Remove(tickable))
                return true;

            if (_registered.Contains(tickable) ||
                _pendingAdditions.Contains(tickable))
            {
                return false;
            }
            
            _registered.Add(tickable);
            return true;
        }

        public bool Unregister(ITickable tickable)
        {
            if (!_isTicking)
                return Remove(tickable);
            
            if (_pendingAdditions.Remove(tickable))
                return true;
            
            if  (!_registered.Contains(tickable))
                return false;
            
            return _registered.Remove(tickable);
        }

        private void RunTick()
        {
            TickNumber++;
            
            var context = new TickContext(
                TickNumber,
                TickInterval);
            
            _isTicking = true;

            try
            {
                foreach (ITickable tickable in _tickables)
                {
                    if (_pendingRemovals.Contains(tickable))
                        continue;

                    tickable.Tick(in context);
                }
            }
            finally
            {
                _isTicking = false;
                ApplyPendingChanges();
            }
        }

        private bool Add(ITickable tickable)
        {
            if (!_registered.Add(tickable))
                return false;
            
            _tickables.Add(tickable);
            return true;
        }

        private bool Remove(ITickable tickable)
        {
            if (!_registered.Remove(tickable))
                return false;
            
            _tickables.Remove(tickable);
            return true;
        }

        private void ApplyPendingChanges()
        {
            foreach (ITickable tickable in _pendingRemovals)
                Remove(tickable);
            
            _pendingRemovals.Clear();
            
            foreach (ITickable tickable in _pendingAdditions)
                Add(tickable);
            
            _pendingAdditions.Clear();
        }
    }
}
