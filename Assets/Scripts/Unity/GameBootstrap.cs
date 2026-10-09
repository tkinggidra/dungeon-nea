using Core.Tick;
using Core.Tick.Unity;
using Prototype;
using UnityEngine;
using World;

namespace Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private TickSchedulerBehaviour tickSchedulerBehaviour;
        
        private TickScheduler _tickScheduler;
        private Level _level;

        private void Awake()
        {
            _tickScheduler = new TickScheduler();
            _level = new Level();

            _level.AddEntity(new TestEntity());
            
            _tickScheduler.Register(_level);
            tickSchedulerBehaviour.Initialize(_tickScheduler);
        }
    }
}