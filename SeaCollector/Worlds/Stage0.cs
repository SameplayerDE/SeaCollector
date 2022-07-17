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
        public Hero GameObject0;
        public BaseCamera Cam;
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

            GameObject0 = new Hero();
            GameObject0.Initialize();
            
            Cam = new FixedPerspectiveCamera(GraphicsDevice);
            Cam.Translate(new Vector3(0, 2, 2));
            
            GameObject0.AddChild(Cam);
            
            GameObject1 = new GameObject();
            GameObject1.Position = Vector3.Zero;
            GameObject1.Mesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/Hulls/hull_0.obj");
            
            ((BallCamera)Camera).Position = new Vector3(1, 1, 1);
            ((BallCamera)Camera).Target = GameObject0;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            GameObject0.LoadContent(GraphicsDevice, Content);
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
            Cam.Update(gameTime);
            GameObject0.Update(gameTime);
            GameObject1.Update(gameTime);
            Camera.Update();
            
            Console.WriteLine(GameObject0.LocalPosition.ToString());
        }

        public override void Draw(GameTime gameTime)
        {
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            GraphicsDevice.Clear(new Color(78, 202, 255));
            Effect.Parameters["Texture00"]?.SetValue(Texture);
            //GameObject0.Draw(GraphicsDevice, Effect, WorldMatrix, Cam.View, Cam.Projection);
            GameObject1.Mesh.Draw(GraphicsDevice, Effect, WorldMatrix, Cam.View, Cam.Projection);
            
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = WorldMatrix;
                    effect.View = Cam.View;
                    effect.Projection = Cam.Projection;
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