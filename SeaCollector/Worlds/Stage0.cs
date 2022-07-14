using System;
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
        public GameObject GameObject1;
        public Effect Effect;
        public Texture2D Texture;
        public Model Model;

        public Stage0(Game game) : base(game)
        {
            
        }

        protected override void Init()
        {
            base.Init();
            Camera = new BallCamera(GraphicsDevice);
            
            GameObject0 = new GameObject();
            GameObject0.Position = Vector3.Zero;
            GameObject0.Mesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/link.obj");
            GameObject0.Scale /= 2;
            
            GameObject1 = new GameObject();
            GameObject1.Position = Vector3.Zero;
            GameObject1.Mesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/Hulls/hull_0.obj");
            
            ((BallCamera)Camera).Position = new Vector3(1, 1, 1);
            ((BallCamera)Camera).Target = GameObject0;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Model = Content.Load<Model>("untitled");
            Effect = Content.Load<Effect>("Effects/TextureCellShader");
            Texture = Content.Load<Texture2D>("Textures/Link/main_red");
        }

        public override void UnloadContent()
        {
            Content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            var direction = new Vector3();
            var mouseDelta = Input.Instance.LatestMouseDelta;

            if (HxInput.Input.Instance.IsKeyboardKeyDown(Keys.W))
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

            if (GameObject0.Position.Y > 0)
            {
                GameObject0.Position.Y -= 9.81f * Time.Instance.DeltaSecondsF;
                GameObject0.Position.Y = Math.Clamp(GameObject0.Position.Y, 0, 100);
            }
            
            if (direction.Length() != 0)
            {
                direction.Normalize();
                direction *= 2f;
                direction *= Time.Instance.DeltaSecondsF;
                
                var tRotation = Quaternion.Identity;
                var tPosition = Vector3.Zero;
                var tScale = Vector3.One;

                GameObject0.Matrix.Decompose(out tScale, out tRotation, out tPosition);
                
                GameObject0.Position += Vector3.Transform(direction, tRotation);
            }

            GameObject0.Rotation.Y += mouseDelta.X * Time.Instance.DeltaSecondsF;

            GameObject0.Update(gameTime);
            GameObject1.Update(gameTime);
            Camera.Update();
            
            Console.WriteLine(GameObject0.Position.ToString());
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
            GameObject1.Draw(GraphicsDevice, Effect, WorldMatrix, Camera.View, Camera.Projection);
            
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = WorldMatrix;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                }
 
                mesh.Draw();
            }
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            //_spriteBatch.Draw(_renderTarget, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            //_spriteBatch.End();
        }
    }
}

/*
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
Camera.Update();*/