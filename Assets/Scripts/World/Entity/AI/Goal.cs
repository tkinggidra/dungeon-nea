namespace World.Entity.AI
{
    public abstract class Goal
    {
        public virtual GoalFlag Flags => GoalFlag.None;
        
        public abstract bool CanUse();
        
        public virtual bool CanContinue()
        {
            return CanUse();
        }
        
        public virtual void Start() { }
        public virtual void Tick() { }
        public virtual void Stop() { }
    }
}