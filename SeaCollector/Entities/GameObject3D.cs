using Microsoft.Xna.Framework;

namespace SeaCollector.Entities
{
    public abstract class GameObject3D
    {
        public Vector3 LocalPosition;
        public Vector3 WorldPosition;

        public Quaternion LocalRotation;
        public Quaternion WorldRotation;
        
        public Vector3 LocalScale;
        public Vector3 WorldScale;

        protected Matrix Matrix;

        protected GameObject3D()
        {
            LocalScale = WorldScale = Vector3.One;
        }

    }
}