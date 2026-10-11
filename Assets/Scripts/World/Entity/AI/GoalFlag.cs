using System;

namespace World.Entity.AI
{
    [Flags]
    public enum GoalFlag
    {
        None = 0,
        Move = 1 << 0,
        Look = 1 << 1,
        Attack = 1 << 2,
    }
}