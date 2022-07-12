using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Rectangle _renderTargetRectangle;

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

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            _renderTarget = new RenderTarget2D(GraphicsDevice, 240, 160, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            _renderTargetRectangle = new Rectangle(0, 0, 240, 160);

            _drawCallWatch = new Stopwatch();

            _world0 = new Stage0(GraphicsDevice);
            _world1 = new Stage1(GraphicsDevice);

            base.Initialize();
        }

        private void OnResize(object sender, EventArgs e)
        {
            var rectangle = GraphicsDevice.PresentationParameters.Bounds;
            var width = rectangle.Width;
            var height = rectangle.Height;
#if DEBUG
            Console.WriteLine(rectangle.ToString());
#endif
            if (width >= height)
            {
                _renderTargetRectangle.Width = width;
                _renderTargetRectangle.Height = 160 * (width / 240);
            }
            else
            {
                _renderTargetRectangle.Height = height;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _world0.LoadContent(Content);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _world0.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(new Color(78, 202, 255));
            _world0.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            _spriteBatch.Draw(_renderTarget, _renderTargetRectangle, Color.White);
            _spriteBatch.End();
        }
    }
}