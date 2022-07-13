using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector
{
    public class World
    {
        public string Name = Guid.NewGuid().ToString();

        protected ContentManager Content;
        
        public Game Game;
        public Camera Camera;
        public Matrix WorldMatrix;
        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public World(Game game)
        {
            Content = new ContentManager(game.Services, "Content");
            Game = game;
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