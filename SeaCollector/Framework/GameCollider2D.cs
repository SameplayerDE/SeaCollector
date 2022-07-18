using Microsoft.Xna.Framework;

namespace SeaCollector.Framework
{
    public abstract class GameCollider2D : GameObject2D
    {
        public abstract bool Contains(Vector2 position);
        public abstract bool Contains(Point position);
    }
}