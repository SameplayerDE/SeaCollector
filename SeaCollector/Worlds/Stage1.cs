using HxInput;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Rendering;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector.Worlds
{
    public class Stage1 : World
    {
        
        public BillboardSystem TreeBillboardSystem;
        
        public Stage1(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            WorldMatrix = Matrix.Identity;
            Camera = new StationaryCamera(GraphicsDevice);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            TreeBillboardSystem = new BillboardSystem(GraphicsDevice, contentManager, contentManager.Load<Texture2D>("Textures/tree"), new Vector2(1));
            TreeBillboardSystem.Initialize(GraphicsDevice);
            TreeBillboardSystem.Mode = BillboardMode.Cylindrical;
        }

        public override void Update(GameTime gameTime)
        {
            Input.Instance.Update(gameTime);
            Time.Instance.Update(gameTime);

            var direction = new Vector3();
            
            if (Input.Instance.IsKeyboardKeyDown(Keys.W))
            {
                direction += Vector3.Forward;
            }
            if (Input.Instance.IsKeyboardKeyDown(Keys.A))
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
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                direction += Vector3.Up;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                direction += Vector3.Down;
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                ((StationaryCamera)Camera).RotateX(45 * Time.Instance.DeltaSecondsF);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                ((StationaryCamera)Camera).RotateX(-45 * Time.Instance.DeltaSecondsF);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                ((StationaryCamera)Camera).RotateY(45 * Time.Instance.DeltaSecondsF);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                ((StationaryCamera)Camera).RotateY(-45 * Time.Instance.DeltaSecondsF);
            }

            if (direction.Length() != 0)
            {
                direction.Normalize();
                direction *= 2f;
                direction *= Time.Instance.DeltaSecondsF;
                //((StationaryCamera)Camera).Move(direction);
            }
            Camera.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            TreeBillboardSystem.Draw(Camera.View, Camera.Projection, Camera.Up, 
                Vector3.Cross(Camera.Forward, Vector3.Up));
        }
    }
}