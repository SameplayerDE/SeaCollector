using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public class GameSpriteFont : GameObject2D
    {
        public string SpriteFontFile;
        public string Text = string.Empty;
        
        public SpriteFont SpriteFont;
        public float Depth;
        public Color Color;
        public SpriteEffects Effect;
        public Rectangle? DrawRect;

        public float Width => (float)SpriteFont.MeasureString(Text).X;
        public float Height => (float)SpriteFont.MeasureString(Text).Y;
        
        public GameSpriteFont(string spriteFontFile)
        {
            SpriteFontFile = spriteFontFile;
            Color = Color.White;
            Effect = SpriteEffects.None;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            SpriteFont = contentManager.Load<SpriteFont>(SpriteFontFile);
        }

        public override void Draw(RenderContext renderContext)
        {
            if (!CanDraw)
                return;
            renderContext.SpriteBatch.DrawString(SpriteFont, Text, WorldPosition, Color, MathHelper.ToRadians(WorldRotation), Vector2.Zero, WorldScale, Effect, Depth);
            base.Draw(renderContext);
        }
    }
}