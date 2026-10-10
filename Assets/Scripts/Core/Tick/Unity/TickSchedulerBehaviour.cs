using System;
using UnityEngine;

namespace Core.Tick.Unity
{
    public class TickSchedulerBehaviour : MonoBehaviour
    {
        public TickScheduler Scheduler { get; private set; }

        public void Initialize(TickScheduler scheduler)
        {
            if (Scheduler != null)
                throw new InvalidOperationException(
                    "The tick scheduler has already been initialized.");
            
            Scheduler = scheduler ??
                        throw new ArgumentNullException(nameof(scheduler)); ;
        }

    }
}