using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public static class GameMath
    {
        public static Ray CalculateRay(Vector2 mouseLocation, Matrix view, 
            Matrix projection, Viewport viewport)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 0.0f),
                projection,
                view,
                Matrix.Identity);
 
            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 1.0f),
                projection,
                view,
                Matrix.Identity);
 
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
 
            return new Ray(nearPoint, direction);
        }
        
        public static float? IntersectDistance(BoundingSphere sphere, Vector2 mouseLocation,
            Matrix view, Matrix projection, Viewport viewport)
        {
            Ray mouseRay = CalculateRay(mouseLocation, view, projection, viewport);
            return mouseRay.Intersects(sphere);
        }
        
        public static bool Intersects(Vector2 mouseLocation, 
            BoundingSphere sphere, Matrix world, 
            Matrix view, Matrix projection, 
            Viewport viewport)
        {
            sphere = sphere.Transform(world);
            float? distance = IntersectDistance(sphere, mouseLocation, view, projection, viewport);
 
            if (distance != null)
            {
                return true;
            }
 
            return false;
        }
        
    }
}