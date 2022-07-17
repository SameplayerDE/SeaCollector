using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Framework
{
    public abstract class GameObject3D
    {
        public Vector3 LocalPosition;
        public Vector3 WorldPosition;

        public Quaternion LocalRotation;
        public Quaternion WorldRotation;

        public Vector3 LocalScale;
        public Vector3 WorldScale;

        public GameObject3D Parent;
        public List<GameObject3D> Children;

        public Matrix WorldMatrix;


        protected GameObject3D()
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
            var nRotation =
                Matrix.CreateRotationX(x) *
                Matrix.CreateRotationY(y) *
                Matrix.CreateRotationZ(z);
            LocalRotation = Quaternion.CreateFromRotationMatrix(nRotation);
        }
        
        public void Rotate(float x, float y, float z)
        {
            var nRotation =
                Matrix.CreateRotationX(x) *
                Matrix.CreateRotationY(y) *
                Matrix.CreateRotationZ(z);
            LocalRotation = Quaternion.CreateFromRotationMatrix(nRotation);
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
            WorldMatrix = Matrix.CreateFromQuaternion(LocalRotation) *
                          Matrix.CreateScale(LocalScale) *
                          Matrix.CreateTranslation(LocalPosition);
            
            if (Parent != null)
            {
                WorldMatrix = Matrix.Multiply(WorldMatrix, Parent.WorldMatrix);

                if (!WorldMatrix.Decompose(out var scale, out var rotation, out var position))
                    Debug.WriteLine("Object3D Decompose World Matrix FAILED!");

                WorldPosition = position;
                WorldScale = scale;
                WorldRotation = rotation;
            }
            else
            {
                WorldPosition = LocalPosition;
                WorldScale = LocalScale;
                WorldRotation = LocalRotation;
            }

            Children.ForEach(child => child.Update(gameTime));
        }

        public virtual void Draw(RenderContext renderContext)
        {
            Children.ForEach(child => child.Draw(renderContext));
        }
    }
}