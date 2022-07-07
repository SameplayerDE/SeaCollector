using System;
using System.Collections.Generic;
using System.IO;
using HxInput;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Entities;
using SeaCollector.Rendering;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector
{
    public class Application : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch = null!;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private Random _random = new Random();
        
        private RenderTarget2D _renderTarget;
        
        private Matrix _view;
        private Matrix _projection;
        private Matrix _world;
        
        private Camera _camera;

        private GameObject _gameObject0;
        private GameMesh _gameMesh0;
        private Texture2D _texture0;
        private Effect _shader0;

        private Model _model;

        public Application()
        {
            Content.RootDirectory = "Content";
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _graphicsDeviceManager = new GraphicsDeviceManager(this);

            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
        }

        protected override void Initialize()
        {
            _graphicsDeviceManager.PreferredBackBufferHeight = 720;
            _graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.PreferredBackBufferFormat = SurfaceFormat.Color;
            _graphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24; // <-- set depth here
            _graphicsDeviceManager.ApplyChanges();

            _renderTarget = new RenderTarget2D(GraphicsDevice,  720 / 4, 1280 / 4, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            _world = Matrix.Identity;
            _camera = new FreeCamera(_graphicsDeviceManager.GraphicsDevice);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _model = Content.Load<Model>("hull_0");
            var mesh = GameMesh.LoadFromFile(_graphicsDeviceManager.GraphicsDevice, "Content/untitled.obj");
            _gameMesh0 = GameMesh.LoadFromFile(_graphicsDeviceManager.GraphicsDevice, "Content/Models/Hulls/hull_0.obj");
            _shader0 = Content.Load<Effect>("Effects/TextureCellShader");
            _texture0 = Content.Load<Texture2D>("Models/Hulls/sp_hul01");
            _gameObject0 = new GameObject();
            _gameObject0.Position = Vector3.Zero;
            _gameObject0.Mesh = _gameMesh0;

            ((FreeCamera)_camera).Position = new Vector3(1, 1, 1); 
            
            base.LoadContent();
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var effect1 in mesh.Effects)
                {
                    var effect = (BasicEffect)effect1;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
 
                mesh.Draw();
            }
        }
        
        protected override void Update(GameTime gameTime)
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
                ((FreeCamera)_camera).RotateX(45 * Time.Instance.DeltaSecondsF);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                ((FreeCamera)_camera).RotateX(-45 * Time.Instance.DeltaSecondsF);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                ((FreeCamera)_camera).RotateY(45 * Time.Instance.DeltaSecondsF);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                ((FreeCamera)_camera).RotateY(-45 * Time.Instance.DeltaSecondsF);
            }

            if (direction.Length() != 0)
            {
                direction.Normalize();
                direction *= 2f;
                direction *= Time.Instance.DeltaSecondsF;
                ((FreeCamera)_camera).Move(direction);
            }
            
            _camera.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(new Color(78, 202, 255));
            _shader0.Parameters["Texture00"]?.SetValue(_texture0);
            //_gameObject0.Draw(_graphicsDeviceManager.GraphicsDevice, _shader0, _world, _camera.View, _camera.Projection);
            DrawModel(_model, _world, _camera.View, _camera.Projection);
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            _spriteBatch.Draw(_renderTarget, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            _spriteBatch.End();

        }
    }
}