using UnityEngine;

namespace World.Entity.Unity
{
    public sealed class EntityPose
    {
        private readonly Entity _entity;
        private readonly Transform _presentationRoot;

        private Pose _previous;
        private Pose _current;

        public EntityPose(
            Entity entity,
            Transform presentationRoot)
        {
            _entity = entity;
            _presentationRoot = presentationRoot;

            Snap();
        }

        public void Snap()
        {
            _previous = _current = ReadPose();
            Render(1f);
        }

        public void BeginTick()
        {
            _previous = _current;
        }

        public void CommitTick()
        {
            _current = ReadPose();
        }

        public void Render(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            Vector3 position = Vector3.Lerp(
                _previous.position,
                _current.position,
                alpha);
            
            Quaternion rotation = Quaternion.Slerp(
                _previous.rotation,
                _current.rotation,
                alpha);
            
            _presentationRoot.SetPositionAndRotation(position, rotation);
        }

        public Pose ReadPose()
        {
            var position = _entity.Transform.Position;
            var rotation = _entity.Transform.Rotation;
            
            return new Pose(
                new Vector3(
                    position.X,
                    position.Y,
                    position.Z
                ),
                new Quaternion(
                    rotation.X,
                    rotation.Y,
                    rotation.Z,
                    rotation.W
                )
            );
        }
    }
}