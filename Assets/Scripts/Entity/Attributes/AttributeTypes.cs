namespace Entity.Attributes
{
    public static class AttributeTypes
    {
        public static readonly EntityAttribute MaxHealth =
            new("max_health", 100, 1, 100_000);
        
        public static readonly EntityAttribute MovementSpeed =
            new("movement_speed", 100, 0, 400);
        
        public static readonly EntityAttribute AttackDamage =
            new("attack_damage", 1, 0, 100_000);
        
        public static readonly EntityAttribute Defense =
            new("defense", 0, 0, 100_000);
    }
}