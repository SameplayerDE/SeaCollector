using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering.Cameras
{
    public abstract class Camera
    {
        public float FieldOfView = 45f;
        
        public Matrix View;
        public Matrix Projection;

        public float NearPlane = 0.1f;
        public float FarPlane = 10000f;
        
        protected GraphicsDevice GraphicsDevice;
 
        public Camera(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GeneratePerspectiveProjectionMatrix();
        }
        
        public void GeneratePerspectiveProjectionMatrix()
        {
            var presentationParameters = GraphicsDevice.PresentationParameters;
            var aspectRatio = (float)presentationParameters.BackBufferWidth / presentationParameters.BackBufferHeight;
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(FieldOfView), aspectRatio, NearPlane, FarPlane);
        }
        public virtual void Update()
        {
        }
    }
}