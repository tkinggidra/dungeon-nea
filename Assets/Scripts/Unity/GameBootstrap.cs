using Core.Tick;
using Core.Tick.Unity;
using UnityEngine;

namespace Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private TickSchedulerBehaviour tickSchedulerBehaviour;
        
        private TickScheduler tickScheduler;

        private void Awake()
        {
            tickScheduler = new TickScheduler();
            tickSchedulerBehaviour.Initialize(tickScheduler);
        }
    }
}