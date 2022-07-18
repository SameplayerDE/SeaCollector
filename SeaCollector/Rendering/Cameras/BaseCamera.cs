using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Entities;
using SeaCollector.Framework;

namespace SeaCollector.Rendering.Cameras
{

    public class BaseCamera : GameObject3D
    {
        public Matrix View;
        public Matrix Projection;
        
        public float NearPlane = 0.1f;
        public float FarPlane = 1000f;

        public Vector3 Facing = Vector3.Forward;
        public Vector3 Forward = Vector3.Forward;
        public Vector3 Up = Vector3.Up;
        
        public BaseCamera(GraphicsDevice graphicsDevice)
        {
            Projection = Matrix.CreateOrthographic(256, 192, NearPlane, FarPlane);
        }

        public virtual void BuildViewMatrix()
        {
            var lookAt = Vector3.Transform(Vector3.Forward, WorldRotation);
            lookAt.Normalize();

            var quaternionY = WorldRotation;
            quaternionY.X = 0;
            quaternionY.Z = 0;
            quaternionY.Normalize();

            Facing = Vector3.Transform(Vector3.Forward, WorldRotation);
            Forward = Vector3.Transform(Vector3.Forward, quaternionY);
            Up = Vector3.Transform(Vector3.Up, WorldRotation);
            
            View = Matrix.CreateLookAt(WorldPosition, WorldPosition + lookAt, Vector3.Up);
            
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            BuildViewMatrix();
        }
    }
}