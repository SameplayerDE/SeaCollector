using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Entities;

namespace SeaCollector.Rendering.Cameras
{

    public class BaseCamera : GameObject3D
    {
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        public BaseCamera(GraphicsDevice graphicsDevice)
        {
            Projection = Matrix.CreateOrthographic(1280, 720, 0.1f, 300);
        }

        public virtual void BuildViewMatrix()
        {
            var lookAt = Vector3.Transform(Vector3.Forward, WorldRotation);
            lookAt.Normalize();

            View = Matrix.CreateLookAt(WorldPosition, (WorldPosition + lookAt), Vector3.Up);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            BuildViewMatrix();
        }
    }
}