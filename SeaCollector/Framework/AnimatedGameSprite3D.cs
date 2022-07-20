using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public class AnimatedGameSprite3D : GameSprite3D
    {

        public Point CellSize;
        public Point Cell;
        
        public AnimatedGameSprite3D(GraphicsDevice graphicsDevice, Vector2 size, string textureFile, Point cellSize) : base(graphicsDevice, size, textureFile)
        {
            CellSize = cellSize;
            EffectFile = "Effects/AnimatedSpriteShader";
        }

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            base.LoadContent(graphicsDevice, contentManager);
            Effect.Parameters["UVScale"]?.SetValue(Vector2.One / Texture2D.Bounds.Size.ToVector2() * CellSize.ToVector2());
        }

        public override void Update(GameTime gameTime)
        {
            Effect.Parameters["UVOffset"]?.SetValue(Cell.ToVector2());
            base.Update(gameTime);
        }
    }
}