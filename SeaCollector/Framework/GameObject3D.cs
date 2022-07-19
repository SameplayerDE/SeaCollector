using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public class GameObject3D : GameObject
    {
        public Vector3 LocalPosition;
        public Vector3 WorldPosition;

        public Quaternion LocalRotation;
        public Quaternion WorldRotation;

        public Vector3 LocalScale;
        public Vector3 WorldScale;

        public bool FixedRotationY = false;
        public bool FixedRotationX = false;
        public bool FixedRotationZ = false;
        public bool FixedRotation = false;
        
        public GameObject3D Parent;
        public List<GameObject3D> Children;

        public Matrix WorldMatrix;

        
        public GameScene Scene
        {
            get
            {
                if (_scene != null) return _scene;
                if (Parent != null) return Parent.Scene;
                return null;
            }

            set { _scene = value; }
        }
        
        
        public GameObject3D()
        {
            Children = new List<GameObject3D>();
            LocalScale = WorldScale = Vector3.One;
        }

        public void AddChild(GameObject3D child)
        {
            if (Children.Contains(child)) return;
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(GameObject3D child)
        {
            if (Children.Remove(child))
            {
                child.Parent = null;
            }
        }

        public void Translate(Vector3 translation)
        {
            LocalPosition = translation;
        }

        public void Translate(float x, float y, float z)
        {
            LocalPosition = new Vector3(x, y, z);
        }

        public void Scale(Vector3 scale)
        {
            LocalScale = scale;
        }

        public void Scale(float x, float y, float z)
        {
            LocalScale = new Vector3(x, y, z);
        }

        public void Rotate(Vector3 rotation)
        {
            var (x, y, z) = rotation;
            Rotate(x, y, z);
        }
        
        public virtual void Rotate(float x, float y, float z)
        {
            var nRotation =
                Matrix.CreateRotationX(x) *
                Matrix.CreateRotationY(y) *
                Matrix.CreateRotationZ(z);
            LocalRotation = Quaternion.CreateFromRotationMatrix(nRotation);
            //LocalRotation = new Vector3(x, y, z);
        }
        
        public void Rotate(Quaternion rotation)
        {
            LocalRotation = rotation;
        }

        /*public void Rotate(float pitch, float yaw, float roll)
        {
            LocalRotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), MathHelper.ToRadians(roll));
        }*/

        public virtual void Initialize()
        {
            Children.ForEach(child => child.Initialize());
        }

        public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            Children.ForEach(child => child.LoadContent(graphicsDevice, contentManager));
        }
        
        public virtual void UnloadContent()
        {
            Children.ForEach(child => child.UnloadContent());
        }

        public virtual void Update(GameTime gameTime)
        {
            
            var nRotation =
                Matrix.CreateRotationX(LocalRotation.X) *
                Matrix.CreateRotationY(LocalRotation.Y) *
                Matrix.CreateRotationZ(LocalRotation.Z);
            //
            
            WorldMatrix = Matrix.CreateFromQuaternion(LocalRotation) *
                          Matrix.CreateScale(LocalScale) *
                          Matrix.CreateTranslation(LocalPosition);
            
            if (Parent != null)
            {
                var matrix = Matrix.Multiply(WorldMatrix, Parent.WorldMatrix);
                
                if (!matrix.Decompose(out var scale, out var rotation, out var position))
                    Debug.WriteLine("Object3D Decompose World Matrix FAILED!");

                var pRotation =
                    Matrix.CreateRotationX(Parent.LocalRotation.X) *
                    Matrix.CreateRotationY(Parent.LocalRotation.Y) *
                    Matrix.CreateRotationZ(Parent.LocalRotation.Z);

                if (FixedRotationX)
                {
                    var quaternion = rotation;
                    quaternion.X = LocalRotation.X;
                    quaternion.Normalize();
                    rotation = quaternion;
                }
                
                if (FixedRotationY)
                {
                    var quaternion = rotation;
                    quaternion.Y = LocalRotation.Y;
                    quaternion.Normalize();
                    rotation = quaternion;
                }
                
                if (FixedRotationZ)
                {
                    var quaternion = rotation;
                    quaternion.Z = LocalRotation.Z;
                    quaternion.Normalize();
                    rotation = quaternion;
                }
                
                if (FixedRotation)
                {
                    WorldMatrix = Matrix.CreateFromQuaternion(LocalRotation) *
                                  Matrix.CreateScale(scale) *
                                  Matrix.CreateTranslation(position);
                    WorldPosition = position;
                    WorldScale = scale;
                    WorldRotation = LocalRotation;
                }
                else
                {
                    WorldPosition = position;
                    WorldScale = scale;
                    WorldRotation = rotation;
                    WorldMatrix = matrix;
                }
                
                
            }
            else
            {
                WorldPosition = LocalPosition;
                WorldScale = LocalScale;
                WorldRotation = LocalRotation;
            }

            Children.ForEach(child => child.Update(gameTime));
        }
        
        private Vector3 ToEuler(Quaternion quaternion)
        {
            var result = Vector3.Zero;
            
            quaternion.Normalize();
            quaternion.Deconstruct(out var qx, out var qy, out var qz, out var qw);
            
            result.X = (float)Math.Atan2(-2*(qy*qz-qw*qx), qw*qw-qx*qx-qy*qy+qz*qz);
            result.Y = (float)Math.Asin(2*(qx*qz + qw*qy));
            result.Z = (float)Math.Atan2(-2*(qx*qy-qw*qz), qw*qw+qx*qx-qy*qy-qz*qz);

            return result;
        }
        
        public static Vector3 ToEulerAngles(Quaternion q)
        {
            var angles = Vector3.Zero;

            // roll (x-axis rotation)
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.Z = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.X = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.X = (float)Math.Asin(sinp);
            }

            // yaw (z-axis rotation)
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Y = -(float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public virtual void Draw(RenderContext renderContext)
        {
            Children.ForEach(child => child.Draw(renderContext));
        }
    }
}