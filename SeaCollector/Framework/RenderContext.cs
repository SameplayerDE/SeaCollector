using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector.Framework
{
    public class RenderContext
    {
        public SpriteBatch SpriteBatch;
        public GraphicsDevice GraphicsDevice;
        public GameTime GameTime;
        public BaseCamera Camera;
        public Matrix View;
        public Matrix Projection;
        public Vector3 Up;
        public Vector3 Right;
    }
}