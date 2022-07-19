using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public class RectangleCollider2D : GameCollider2D
    {
        public int Width;
        public int Height;
        
        public override bool Contains(Vector2 position)
        {
            var size = new Vector2(Width, Height);
            var location = WorldPosition;
            
            //size *= GameSceneManager.Instance.RenderContext.RenderTargetScale;
            //location += GameSceneManager.Instance.RenderContext.RenderTargetRectangle.Location.ToVector2();
            //location *= GameSceneManager.Instance.RenderContext.RenderTargetScale;
            //location += GameSceneManager.Instance.RenderContext.RenderTargetRectangle.Location.ToVector2();
            
            return new Rectangle(location.ToPoint(), size.ToPoint()).Contains(position);
        }

        public override bool Contains(Point position)
        {
            return Contains(position.ToVector2());
        }
    }
}