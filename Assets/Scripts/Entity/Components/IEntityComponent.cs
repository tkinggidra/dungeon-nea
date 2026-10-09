namespace Entity.Components
{
    public interface IEntityComponent
    {
        void OnAttach(AbstractEntity entity);
        void OnDetach();
    }
}