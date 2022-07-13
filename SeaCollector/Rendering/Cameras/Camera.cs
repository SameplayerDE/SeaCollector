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
        public float FarPlane = 1000f;
        
        public Vector3 Facing = Vector3.Forward;
        public Vector3 Forward = Vector3.Forward;
        public Vector3 Up = Vector3.Up;
        
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
        
        public void GeneratePerspectiveProjectionMatrix(int width, int height)
        {
            var aspectRatio = (float)width / height;
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(FieldOfView), aspectRatio, NearPlane, FarPlane);
        }
        public virtual void Update()
        {
        }
    }
}