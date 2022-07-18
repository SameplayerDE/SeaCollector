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
            Projection = Matrix.CreateOrthographic(1280, 720, NearPlane, FarPlane);
        }

        public virtual void BuildViewMatrix()
        {
            float x, y, z;
            
            //WorldRotation.Deconstruct(out x, out y, out z, out var w);

            /*var nRotation =
                Matrix.CreateRotationX(WorldRotation.X) *
                Matrix.CreateRotationY(WorldRotation.Y) *
                Matrix.CreateRotationZ(WorldRotation.Z);

            var nRotationXY =
                Matrix.CreateRotationX(WorldRotation.X) *
                Matrix.CreateRotationY(WorldRotation.Y);
            
            var lookAt = Vector3.Transform(Vector3.Forward, nRotation);
            lookAt.Normalize();

            Facing = Vector3.Transform(Vector3.Forward, nRotation);
            Forward = Vector3.Transform(Vector3.Forward, nRotationXY);
            Up = Vector3.Transform(Vector3.Up, nRotation);*/
            
            var lookAt = Vector3.Transform(Vector3.Forward, WorldRotation);
            lookAt.Normalize();

            Facing = Vector3.Transform(Vector3.Forward, WorldRotation);
            Forward = Vector3.Transform(Vector3.Forward, WorldRotation);
            Up = Vector3.Transform(Vector3.Up, WorldRotation);
            
            View = Matrix.CreateLookAt(WorldPosition, WorldPosition + lookAt, Vector3.Up);
            
        }

        private Vector3 ToEuler(Quaternion quaternion)
        {
            var result = Vector3.Zero;
            
            quaternion.Deconstruct(out var qx, out var qy, out var qz, out var qw);
            var q = quaternion;
            
            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;
            
            result.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (sqz  + sqw));     // Yaw 
            result.X = (float)Math.Asin(2f * ( q.X * q.Z - q.W * q.Y ) );                             // Pitch 
            result.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (sqy + sqz));      // Roll 
            return result;
        }
        
        public static Vector3 ToEulerAngles(Quaternion q)
        {
            var angles = Vector3.Zero;

            // roll (x-axis rotation)
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw (z-axis rotation)
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            BuildViewMatrix();
        }
    }
}