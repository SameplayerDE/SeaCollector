using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public class RectangleCollider2D : GameCollider2D
    {

        public Texture2D Texture;
        public int Width;
        public int Height;

        public override void LoadContent(ContentManager contentManager)
        {
            Texture = contentManager.Load<Texture2D>("Textures/pixel");
            base.LoadContent(contentManager);
        }

        public override bool Contains(Vector2 position)
        {
            var size = new Vector2(Width, Height);
            size *= GameSceneManager.Instance.RenderContext.RenderTargetScale;
            var location = WorldPosition;
            location *= GameSceneManager.Instance.RenderContext.RenderTargetScale;
            location += GameSceneManager.Instance.RenderContext.RenderTargetRectangle.Location.ToVector2();
            
            return new Rectangle(location.ToPoint(), size.ToPoint()).Contains(position);
        }

        public override bool Contains(Point position)
        {
            return Contains(position.ToVector2());
        }

        public override void Draw(RenderContext renderContext)
        {
            //if (!CanDraw) return;
            
            var size = new Vector2(Width, Height);
            size *= GameSceneManager.Instance.RenderContext.RenderTargetScale;
            var location = WorldPosition;
            location *= GameSceneManager.Instance.RenderContext.RenderTargetScale;
            location += GameSceneManager.Instance.RenderContext.RenderTargetRectangle.Location.ToVector2();
            
            var rect = new Rectangle(location.ToPoint(), size.ToPoint());
            
            renderContext.SpriteBatch.Draw(Texture, rect, Color.White);
            base.Draw(renderContext);
        }
    }
}