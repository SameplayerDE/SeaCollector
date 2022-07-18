using Microsoft.Xna.Framework;

namespace SeaCollector.Framework
{
    public class RectangleCollider2D : GameCollider2D
    {

        public int Width;
        public int Height;
        
        public override bool Contains(Vector2 position)
        {
            return new Rectangle(WorldPosition.ToPoint(), new Point(Width, Height)).Contains(position);
        }

        public override bool Contains(Point position)
        {
            return Contains(position.ToVector2());
        }
    }
}