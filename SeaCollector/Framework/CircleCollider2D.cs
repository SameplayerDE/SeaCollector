using Microsoft.Xna.Framework;

namespace SeaCollector.Framework
{
    public class CircleCollider2D : GameCollider2D
    {

        public int Radius;
        
        public override bool Contains(Vector2 position)
        {
            var distanceToPosition = Vector2.Distance(position, WorldPosition);
            return distanceToPosition < Radius;
        }

        public override bool Contains(Point position)
        {
            return Contains(position.ToVector2());
        }
    }
}