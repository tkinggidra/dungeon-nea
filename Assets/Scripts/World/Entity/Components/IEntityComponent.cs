namespace World.Entity.Components
{
    public interface IEntityComponent
    {
        void OnAttach(World.Entity.Entity entity);
        void OnDetach();
    }
}