using Microsoft.Xna.Framework;

namespace SeaCollector.Framework
{
    public class CircleCollider2D : GameCollider2D
    {

        public int Radius;
        
        public override bool Contains(Vector2 position)
        {
            var radius = (float)Radius;
            radius *= GameSceneManager.Instance.RenderContext.RenderTargetScale;

            var center = WorldPosition;
            center += GameSceneManager.Instance.RenderContext.RenderTargetRectangle.Location.ToVector2();

            var distanceToPosition = Vector2.Distance(position, center);
            return distanceToPosition < radius;
        }

        public override bool Contains(Point position)
        {
            return Contains(position.ToVector2());
        }
    }
}