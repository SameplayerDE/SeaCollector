using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Input;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector.Rendering
{
    public class RenderContext
    {
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public GameTime GameTime { get; set; }
        public Camera Camera { get; set; }
        public InputManager Input { get; set; }
    }
}