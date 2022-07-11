using System;
using System.Diagnostics;
using HxInput;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Entities;
using SeaCollector.Rendering;
using SeaCollector.Rendering.Cameras;
using SeaCollector.Worlds;

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

        private Player _player;
        
        private Model _model;
        
        private BillboardSystem trees;
        private BillboardSystem mokutaru;
        private Stopwatch _drawCallWatch;

        private World _world0;
        private World _world1;
        
        

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
            
            _drawCallWatch = new Stopwatch();

            _world0 = new Stage0(GraphicsDevice);
            _world1 = new Stage1(GraphicsDevice);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //_model = Content.Load<Model>("untitled");
            _gameMesh0 = GameMesh.LoadFromFile(_graphicsDeviceManager.GraphicsDevice, "Content/Models/Hulls/hull_0.obj");
            //_gameMesh0 = GameMesh.LoadFromFile(_graphicsDeviceManager.GraphicsDevice, "Content/Models/");
            _shader0 = Content.Load<Effect>("Effects/TextureCellShader");
            _texture0 = Content.Load<Texture2D>("Models/Hulls/sp_hul01");
            _gameObject0 = new GameObject();
            _gameObject0.Position = Vector3.Zero;
            _gameObject0.Mesh = _gameMesh0;

            trees = new BillboardSystem(GraphicsDevice, Content, Content.Load<Texture2D>("Textures/tree"), new Vector2(1));
            trees.Initialize(GraphicsDevice);
            trees.Mode = BillboardMode.Cylindrical;
            
            mokutaru = new BillboardSystem(GraphicsDevice, Content, Content.Load<Texture2D>("Textures/mokutaru"), new Vector2(0.1f));
            mokutaru.Initialize(GraphicsDevice);
            mokutaru.Mode = BillboardMode.Cylindrical;
            
            _player = new Player();
            _player.LoadContent(GraphicsDevice, Content);
            
            ((FreeCamera)_camera).Position = new Vector3(1, 1, 1); 
            
            _world0.LoadContent(Content);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _world0.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _world0.Draw(gameTime);
            /*var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            GraphicsDevice.Clear(new Color(78, 202, 255));
            _shader0.Parameters["Texture00"]?.SetValue(_texture0);
            _player.Draw(_graphicsDeviceManager.GraphicsDevice, _player.Effect, _world, _camera.View, _camera.Projection);
            
            trees.Draw(_camera.View, _camera.Projection, ((FreeCamera)_camera).Up, 
               Vector3.Cross(((FreeCamera)_camera).Forward, Vector3.Up));
            mokutaru.Draw(_camera.View, _camera.Projection, ((FreeCamera)_camera).Up, 
               Vector3.Cross(((FreeCamera)_camera).Forward, Vector3.Up));
            
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            //_spriteBatch.Draw(_renderTarget, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            //_spriteBatch.End();
            */
        }
    }
}