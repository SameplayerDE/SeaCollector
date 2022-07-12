using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector
{
    public class World
    {
        public string Name = Guid.NewGuid().ToString();

        public GraphicsDevice GraphicsDevice;
        public Camera Camera;
        public Matrix WorldMatrix;

        public World(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            WorldMatrix = Matrix.Identity;
        }
        
        public virtual void LoadContent(ContentManager contentManager)
        {
            throw new NotImplementedException();
        }

        public virtual void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public virtual void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        
        public virtual void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}