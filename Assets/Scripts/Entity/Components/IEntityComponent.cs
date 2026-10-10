namespace Entity.Components
{
    public interface IEntityComponent
    {
        void OnAttach(Entity entity);
        void OnDetach();
    }
}