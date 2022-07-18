using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public class GameSprite : GameObject2D
    {
        public string TextureFile;
        
        public Texture2D Texture2D;
        public float Depth;
        public Color Color;
        public SpriteEffects Effect;
        public Rectangle? DrawRect;

        public float Width => (float)Texture2D.Width;
        public float Height => (float)Texture2D.Height;
        
        public GameSprite(string textureFile)
        {
            TextureFile = textureFile;
            Color = Color.White;
            Effect = SpriteEffects.None;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            Texture2D = contentManager.Load<Texture2D>(TextureFile);
        }

        public override void Draw(RenderContext renderContext)
        {
            if (!CanDraw)
                return;
            renderContext.SpriteBatch.Draw(Texture2D, WorldPosition, DrawRect, Color, MathHelper.ToRadians(WorldRotation), Vector2.Zero, WorldScale, Effect, Depth);
            base.Draw(renderContext);
        }
    }
}