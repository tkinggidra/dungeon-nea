namespace Entity.Attributes
{
    public enum AttributeOperation
    {
        Add,
        MultiplyBase,
        MultiplyTotal,
    }
    
    public sealed class AttributeModifier
    {
        public string Id { get; }
        public double Amount { get; }
        public AttributeOperation Operation { get; }

        public AttributeModifier(
            string id,
            double amount,
            AttributeOperation operation)
        {
            Id = id;
            Amount = amount;
            Operation = operation;
        }
    }
}