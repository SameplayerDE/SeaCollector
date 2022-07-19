using HxTime;
using Microsoft.Xna.Framework;
using SeaCollector.Framework;

namespace SeaCollector.Entities
{
    public class RotationObject3D : GameObject3D
    {
        public float SpeedX = 0f;
        public float SpeedY = 0f;
        public float SpeedZ = 0f;

        public float RotationX;
        public float RotationY;
        public float RotationZ;

        public override void Update(GameTime gameTime)
        {
            RotationX += SpeedX * Time.Instance.DeltaSecondsF;
            RotationY += SpeedY * Time.Instance.DeltaSecondsF;
            RotationZ += SpeedZ * Time.Instance.DeltaSecondsF;
            
            Rotate(RotationX, RotationY, RotationZ);
            base.Update(gameTime);
        }
    }
}