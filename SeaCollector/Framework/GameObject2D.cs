using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SeaCollector.Rendering;

namespace SeaCollector.Framework
{
    public class GameObject2D : GameObject
    {
        public Vector2 LocalPosition;
        public Vector2 WorldPosition;

        public Vector2 LocalScale;
        public Vector2 WorldScale;

        public float LocalRotation;
        public float WorldRotation;

        public Vector2 PivotPoint;

        protected Matrix WorldMatrix;

        public GameObject2D Parent;

        public List<GameObject2D> Children;
        public bool CanDraw;

        public bool DrawInFrontOf3D;

        
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

        public GameObject2D()
        {
            LocalScale = WorldScale = Vector2.One;
            Children = new List<GameObject2D>();

            DrawInFrontOf3D = true;
            CanDraw = true;
        }

        public void AddChild(GameObject2D child)
        {
            if (!Children.Contains(child))
            {
                Children.Add(child);
                child.Parent = this;
            }
        }

        public void RemoveChild(GameObject2D child)
        {
            if (Children.Remove(child))
                child.Parent = null;
        }

        public void Rotate(float rotation)
        {
            LocalRotation = rotation;
        }

        public void Translate(float posX, float posY)
        {
            Translate(new Vector2(posX, posY));
        }

        public void Translate(Vector2 position)
        {
            LocalPosition = position;
        }

        public void Scale(float scaleX, float scaleY)
        {
            Scale(new Vector2(scaleX, scaleY));
        }

        public void Scale(Vector2 scale)
        {
            LocalScale = scale;
        }

        public virtual void Initialize()
        {
            Children.ForEach(child => child.Initialize());
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            Children.ForEach(child => child.LoadContent(contentManager));
        }

        public virtual void UnloadContent()
        {
            Children.ForEach(child => child.UnloadContent());
        }

        public virtual void Draw(RenderContext renderContext)
        {
            if (CanDraw)
                Children.ForEach(child => child.Draw(renderContext));
        }

        public virtual void Update(GameTime gameTime)
        {
            WorldMatrix =
                Matrix.CreateTranslation(new Vector3(-PivotPoint, 0)) *
                Matrix.CreateScale(new Vector3(LocalScale, 1)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(LocalRotation)) *
                Matrix.CreateTranslation(new Vector3(LocalPosition, 0));

            if (Parent != null)
            {
                WorldMatrix = Matrix.Multiply(WorldMatrix, Matrix.CreateTranslation(new Vector3(Parent.PivotPoint, 0)));
                WorldMatrix = Matrix.Multiply(WorldMatrix, Parent.WorldMatrix);
            }
            
            if (!WorldMatrix.Decompose(out var scale, out var rot, out var pos))
            {
                Debug.WriteLine("Object2D Decompose World Matrix FAILED!");
            }

            var (x, y) = Vector2.Transform(Vector2.UnitX, rot);
            WorldRotation = (float)Math.Atan2(y, x);
            WorldRotation = float.IsNaN(WorldRotation) ? 0 : MathHelper.ToDegrees(WorldRotation);

            WorldPosition = new Vector2(pos.X, pos.Y);
            WorldScale = new Vector2(scale.X, scale.Y);

            Children.ForEach(child => child.Update(gameTime));
        }
    }
}