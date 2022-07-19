using System;
using Accessibility;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Framework;

namespace SeaCollector.Entities
{
    public class Hero : GameObject3D
    {
        public GameMeshObject GameMeshObject;

        public float _rotationX = 0f;
        public float _destrotationY = 0f;
        public float _rotationY = 0f;

        public override void Initialize()
        {
            GameMeshObject = new GameMeshObject("Textures/Link/main_red", "Effects/TextureCellShader","Content/Models/link.obj");
            GameMeshObject.Scale(0.8f, 0.8f, 0.8f);
            AddChild(GameMeshObject);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var direction = new Vector3();
            var mouseDelta = HxInput.Input.Instance.LatestMouseDelta;

            if (HxInput.Input.Instance.IsKeyboardKeyDown(Keys.W))
            {
                direction += Vector3.Forward;
            }
            if (HxInput.Input.Instance.IsKeyboardKeyDown(Keys.A))
            {
                direction += Vector3.Left;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                direction += Vector3.Right;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                direction += Vector3.Backward;
            }

            if (direction.Length() != 0)
            {
                direction.Normalize();
                _destrotationY = (float)Math.Atan2(-direction.X, -direction.Z);
                direction *= 3f;
                direction *= Time.Instance.DeltaSecondsF;
                
                var tRotation = Quaternion.Identity;
                var tPosition = Vector3.Zero;
                var tScale = Vector3.One;
                direction = Vector3.Transform(Vector3.Forward, WorldRotation);
                direction.Normalize();
                direction *= 3f;
                direction *= Time.Instance.DeltaSecondsF;
                
                Rotate(Quaternion.CreateFromAxisAngle(Vector3.Up, _rotationY));
                WorldMatrix.Decompose(out tScale, out tRotation, out tPosition);
                var newPosition = LocalPosition + direction;
                //var newPosition = LocalPosition + direction;
                Translate(newPosition);
            }

            _rotationY = MathHelper.WrapAngle(_rotationY);
            _rotationY = CurveAngle(_rotationY, _destrotationY, 0.09f);
            //if (_rotationY < 0) _rotationY += MathHelper.ToRadians(360);
            //_rotationY -= mouseDelta.X * Time.Instance.DeltaSecondsF;
            //_rotationX -= mouseDelta.Y * Time.Instance.DeltaSecondsF;
            
            
            //_rotationX = Math.Clamp(_rotationX, MathHelper.ToRadians(-89.9f), MathHelper.ToRadians(89.9f));
            
            
            base.Update(gameTime);
        }
        
        private float CurveAngle(float from, float to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            Vector2 fromVector = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toVector = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 currentVector = Slerp(fromVector, toVector, step);

            return (float)Math.Atan2(currentVector.Y, currentVector.X);
        }

        private Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            double theta = Math.Acos(Vector2.Dot(from, to));
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            return (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;
        }
        
    }
}