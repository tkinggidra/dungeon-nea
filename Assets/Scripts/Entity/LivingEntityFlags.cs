using System;

namespace Entity
{
    [Flags]
    public enum LivingEntityFlags
    {
        None = 0,
        Invulnerable = 1 << 0,
    }
}