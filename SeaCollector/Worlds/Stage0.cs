using HxInput;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Entities;
using SeaCollector.Rendering;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector.Worlds
{
    public class Stage0 : World
    {
        public GameObject GameObject0;
        public Effect Effect;
        public Texture2D Texture;

        public Stage0(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            WorldMatrix = Matrix.Identity;
            Camera = new FreeCamera(GraphicsDevice);
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
            Effect = contentManager.Load<Effect>("Effects/TextureCellShader");
            Texture = contentManager.Load<Texture2D>("Models/Hulls/sp_hul01");
            GameObject0 = new GameObject();
            GameObject0.Position = Vector3.Zero;
            GameObject0.Mesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/Hulls/hull_0.obj");
            ((FreeCamera)Camera).Position = new Vector3(1, 1, 1);
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
                ((FreeCamera)Camera).RotateX(45 * Time.Instance.DeltaSecondsF);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                ((FreeCamera)Camera).RotateX(-45 * Time.Instance.DeltaSecondsF);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                ((FreeCamera)Camera).RotateY(45 * Time.Instance.DeltaSecondsF);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                ((FreeCamera)Camera).RotateY(-45 * Time.Instance.DeltaSecondsF);
            }

            if (direction.Length() != 0)
            {
                direction.Normalize();
                direction *= 2f;
                direction *= Time.Instance.DeltaSecondsF;
                ((FreeCamera)Camera).Move(direction);
            }
            Camera.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            GraphicsDevice.Clear(new Color(78, 202, 255));
            Effect.Parameters["Texture00"]?.SetValue(Texture);
            GameObject0.Draw(GraphicsDevice, Effect, WorldMatrix, Camera.View, Camera.Projection);

            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            //_spriteBatch.Draw(_renderTarget, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            //_spriteBatch.End();
        }
    }
}