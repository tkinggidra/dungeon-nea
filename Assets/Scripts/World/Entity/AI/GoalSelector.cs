using System;
using System.Collections.Generic;

namespace World.Entity.AI
{
    public sealed class GoalSelector
    {
        private sealed class Entry
        {
            public Goal Goal { get; }
            public int Priority { get; }
            public bool IsRunning { get; set; }

            public Entry(int priority, Goal goal)
            {
                Priority = priority;
                Goal = goal;
            }
        }
        
        private readonly List<Entry> _goals = new();

        public void AddGoal(int priority, Goal goal)
        {
            foreach (var entry in _goals)
            {
                if (ReferenceEquals(entry.Goal, goal))
                    throw new InvalidOperationException(
                        "Goal already registered");
            }
            
            _goals.Add(new Entry(priority, goal));
            
            _goals.Sort((a, b) =>
                a.Priority.CompareTo(b.Priority));
        }

        public void Tick()
        {
            foreach (var entry in _goals)
            {
                if (entry.IsRunning && !entry.Goal.CanContinue())
                    StopGoal(entry);
            }

            foreach (var candidate in _goals)
            {
                if (candidate.IsRunning || !candidate.Goal.CanUse())
                    continue;
                
                if (!CanStart(candidate))
                    continue;

                foreach (var running in _goals)
                {
                    if (!running.IsRunning)
                        continue;
                    
                    if (!Conflicts(candidate.Goal, running.Goal))
                        continue;
                    
                    if (candidate.Priority < running.Priority)
                        StopGoal(running);
                }

                candidate.IsRunning = true;
                candidate.Goal.Start();
            }

            foreach (var entry in _goals)
            {
                if (entry.IsRunning)
                    entry.Goal.Tick();
            }
        }

        private bool CanStart(Entry candidate)
        {
            foreach (Entry running in _goals)
            {
                if (!running.IsRunning)
                    continue;
                
                if (!Conflicts(candidate.Goal, running.Goal))
                    continue;
                
                if (running.Priority <= candidate.Priority)
                    return false;
            }
            
            return true;
        }

        private static bool Conflicts(Goal goal1, Goal goal2)
        {
            return (goal1.Flags & goal2.Flags) != GoalFlag.None;
        }

        private static void StopGoal(Entry entry)
        {
            entry.IsRunning = false;
            entry.Goal.Stop();
        }
    }
}