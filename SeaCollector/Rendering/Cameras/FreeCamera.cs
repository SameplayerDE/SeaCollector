using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering.Cameras
{
    public class FreeCamera : Camera
    {

        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Translation = Vector3.Zero;
        public Vector3 Facing = Vector3.Forward;
        
        public Matrix RotationMXYZ { get { return Matrix.Multiply(Matrix.Multiply(Matrix.CreateRotationX(Rotation.X), Matrix.CreateRotationY(Rotation.Y)), Matrix.CreateRotationZ(Rotation.Z)); } }
        public Matrix RotationMXY { get { return Matrix.Multiply(Matrix.CreateRotationX(Rotation.X), Matrix.CreateRotationY(Rotation.Y)); } }
        public Matrix RotationMXZ { get { return Matrix.Multiply(Matrix.CreateRotationX(Rotation.X), Matrix.CreateRotationZ(Rotation.Z)); } }
        public Matrix RotationMYZ { get { return Matrix.Multiply(Matrix.CreateRotationY(Rotation.Y), Matrix.CreateRotationZ(Rotation.Z)); } }
        public Matrix RotationMX { get { return Matrix.CreateRotationX(Rotation.X); } }
        public Matrix RotationMY { get { return Matrix.CreateRotationY(Rotation.Y); } }
        public Matrix RotationMZ { get { return Matrix.CreateRotationZ(Rotation.Z); } }
        
        
        public FreeCamera(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            Position = new Vector3(0, 0, 0);
        }

        public void Move(Vector3 translation)
        {
            Translation += translation;
        }
        
        public void RotateX(float x)
        {
            Rotation.X += MathHelper.ToRadians(x);
            Rotation.X = Math.Clamp(Rotation.X, MathHelper.ToRadians(-89.9f), MathHelper.ToRadians(89.9f));
        }

        public void RotateZ(float z)
        {
            Rotation.Z += MathHelper.ToRadians(z);
        }
        
        public void RotateY(float y)
        {
            Rotation.Y += MathHelper.ToRadians(y);
            if (Rotation.Y < 0) Rotation.Y += MathHelper.ToRadians(360);
        }
        
        public override void Update()
        {
            Translation = Vector3.Transform(Translation, RotationMY);
            Position += Translation;
            Translation = Vector3.Zero;
            
            var front = Vector3.Transform(Vector3.Forward, RotationMXY);
            Facing = front;
            View = Matrix.CreateLookAt(Position, Position + front, Vector3.Up);
        }
    }
}