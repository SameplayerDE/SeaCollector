using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering.Cameras
{
    public class FixedPerspectiveCamera : BaseCamera
    {

        public float FieldOfView = 70f;
        
        public FixedPerspectiveCamera(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            var presentationParameters = graphicsDevice.PresentationParameters;
            var aspectRatio = (float)256 /  192;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfView), aspectRatio, 0.1f, 300f);
        }
    }
}