using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Entities;
using SeaCollector.Framework;

namespace SeaCollector.Rendering.Cameras
{
    public class BallCamera : Camera
    {

        public GameObject3D Target;
        public float Distance = 5f;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Offset = new Vector3(0, 2.5f, 0);
        
        public BallCamera(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Update()
        {
            var tRotation = Quaternion.Identity;
            var tPosition = Vector3.Zero;
            var tScale = Vector3.One;

            Target.WorldMatrix.Decompose(out tScale, out tRotation, out tPosition);

            Position = Offset + tPosition + Target.WorldMatrix.Backward * Distance;
            //Facing = Vector3.Transform(Vector3.Forward, RotationMXY);
            //Forward = Vector3.Transform(Vector3.Forward, RotationMY);
            //Up = Vector3.Transform(Vector3.Up, RotationMXYZ);
            View = Matrix.CreateLookAt(Position, Target.WorldMatrix.Translation, Vector3.Up);
        }
    }
}