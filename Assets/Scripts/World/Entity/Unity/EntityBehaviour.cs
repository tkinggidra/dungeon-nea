using Core.Tick;
using UnityEngine;

namespace World.Entity.Unity
{
    public class EntityBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform presentationRoot;
        
        private TickScheduler _scheduler;
        private EntityPose _pose;
        
        public Entity Entity { get; private set; }

        public virtual void Initialize(
            Entity entity,
            TickScheduler scheduler)
        {
            if (_scheduler != null)
                Unsubscribe();
            
            Entity = entity;
            _scheduler = scheduler;
            
            if (presentationRoot == null)
                presentationRoot = transform;

            _pose = new EntityPose(
                Entity,
                presentationRoot);

            _scheduler.BeforeTick += BeginTick;
            _scheduler.AfterTick += CommitTick;
        }

        private void BeginTick()
        {
            _pose?.BeginTick();
        }

        private void CommitTick()
        {
            _pose?.CommitTick();
        }

        public void Render(float alpha)
        {
            _pose?.Render(alpha);
        }

        public void Snap()
        {
            _pose?.Snap();
        }

        private void Unsubscribe()
        {
            _scheduler.BeforeTick -= BeginTick;
            _scheduler.AfterTick -= CommitTick;
        }

        protected virtual void OnDestroy()
        {
            if (_scheduler != null)
                Unsubscribe();
        }
    }
}