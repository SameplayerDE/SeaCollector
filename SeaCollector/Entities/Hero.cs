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

        public float _rotation = 0f;

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
                direction *= 10f;
                direction *= Time.Instance.DeltaSecondsF;
                
                var tRotation = Quaternion.Identity;
                var tPosition = Vector3.Zero;
                var tScale = Vector3.One;

                WorldMatrix.Decompose(out tScale, out tRotation, out tPosition);
                var newPosition = LocalPosition + Vector3.Transform(direction, tRotation);
                Translate(newPosition);
            }

            _rotation += mouseDelta.X * Time.Instance.DeltaSecondsF;
            Rotate(new Vector3(0, _rotation, 0));
            base.Update(gameTime);
        }
    }
}