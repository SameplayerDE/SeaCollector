using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using HxInput;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Framework;
using SeaCollector.Scenes;
using SeaCollector.Worlds;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace SeaCollector
{
    public class Application : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch = null!;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private Random _random = new Random();

        private RenderTarget2D _renderTarget;
        public Rectangle RenderTargetRectangle;
        private Point _preferedScreenSize;


        public Application()
        {
            Content.RootDirectory = "Content";
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _graphicsDeviceManager = new GraphicsDeviceManager(this);

            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

            _preferedScreenSize = new Point(256 * 4, 192 * 4);
        }

        protected override void Initialize()
        {
            _graphicsDeviceManager.PreferredBackBufferWidth = _preferedScreenSize.X;
            _graphicsDeviceManager.PreferredBackBufferHeight = _preferedScreenSize.Y;
            _graphicsDeviceManager.PreferredBackBufferFormat = SurfaceFormat.Color;
            _graphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24; // <-- set depth here
            _graphicsDeviceManager.HardwareModeSwitch = false;
            _graphicsDeviceManager.PreferMultiSampling = false;
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.ApplyChanges();

            Window.AllowUserResizing = true;
            //Window.ClientSizeChanged += OnResize;

            _renderTarget = new RenderTarget2D(GraphicsDevice, _preferedScreenSize.X, _preferedScreenSize.Y, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            RenderTargetRectangle = new Rectangle(0, 0, _preferedScreenSize.X, _preferedScreenSize.Y);

            GameSceneManager.Instance.RenderContext.GraphicsDevice = GraphicsDevice;
            GameSceneManager.Instance.Add(new MainMenu(this));
            GameSceneManager.Instance.Add(new DemoScene(this));
            GameSceneManager.Instance.Initialize();
            
            GameSceneManager.Instance.Stage("menu");
            GameSceneManager.Instance.Grab();

            //PerformScreenFit();

            base.Initialize();
        }

        public void ToggleFullscreen()
        {
            _graphicsDeviceManager.IsFullScreen = !_graphicsDeviceManager.IsFullScreen;
            _graphicsDeviceManager.ApplyChanges();
        }

        public void PerformScreenFit()
        {
            var outputAspect = Window.ClientBounds.Width / (float) Window.ClientBounds.Height;
            var preferredAspect = _preferedScreenSize.X / (float) _preferedScreenSize.Y;

            Rectangle dst;
            if (outputAspect <= preferredAspect)
            {
                // output is taller than it is wider, bars on top/bottom
                int presentHeight = (int) ((Window.ClientBounds.Width / preferredAspect) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                dst = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                // output is wider than it is tall, bars left/right
                int presentWidth = (int) ((Window.ClientBounds.Height * preferredAspect) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                dst = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }

            RenderTargetRectangle = dst;

            GameSceneManager.Instance.RenderContext.RenderTargetRectangle = RenderTargetRectangle;
            GameSceneManager.Instance.RenderContext.RenderTargetScale = Vector2
                .Divide(RenderTargetRectangle.Size.ToVector2(), _preferedScreenSize.ToVector2()).Length();
            GameSceneManager.Instance.RenderContext.Camera.BuildViewMatrix();
        }

        private void OnResize(object sender, EventArgs e)
        {
            var rectangle = GraphicsDevice.PresentationParameters.Bounds;
            var width = rectangle.Width;
            var height = rectangle.Height;

            var outputAspect = Window.ClientBounds.Width / (float) Window.ClientBounds.Height;
            var preferredAspect = _preferedScreenSize.X / (float) _preferedScreenSize.Y;

            var factorWidth = width / (float) _preferedScreenSize.X;
            var factorHeight = height / (float) _preferedScreenSize.Y;

            Rectangle dst;
            if (outputAspect <= preferredAspect)
            {
                // output is taller than it is wider, bars on top/bottom
                int presentHeight = (int) ((Window.ClientBounds.Width / preferredAspect) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                dst = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                // output is wider than it is tall, bars left/right
                int presentWidth = (int) ((Window.ClientBounds.Height * preferredAspect) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                dst = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }

            /*if (width >= height)
            {
                _renderTargetRectangle.Width = (int)(_preferedScreenSize.X / factorWidth);
                _renderTargetRectangle.Height = height;
            }
            else
            {
                _renderTargetRectangle.Width = width;
                _renderTargetRectangle.Height = (int)(_preferedScreenSize.Y / factorHeight);
            }*/

            //_renderTargetRectangle.X = (width - _renderTargetRectangle.Width) / 2;
            //_renderTargetRectangle.Y = (height - _renderTargetRectangle.Height) / 2;

            RenderTargetRectangle = dst;

            GameSceneManager.Instance.RenderContext.RenderTargetRectangle = RenderTargetRectangle;
            GameSceneManager.Instance.RenderContext.RenderTargetScale = Vector2
                .Divide(RenderTargetRectangle.Size.ToVector2(), _preferedScreenSize.ToVector2()).Length();
            GameSceneManager.Instance.RenderContext.Camera.BuildViewMatrix();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GameSceneManager.Instance.RenderContext.SpriteBatch = _spriteBatch;
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            HxInput.Input.Instance.Update(gameTime);
            Time.Instance.Update(gameTime);
            
            if (HxInput.Input.Instance.IsKeyboardKeyDownOnce(Keys.Escape))
            {
                Exit();
            }
            
            if (HxInput.Input.Instance.IsKeyboardKeyDownOnce(Keys.F3))
            {
                ToggleFullscreen();
            }

            GameSceneManager.Instance.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GameSceneManager.Instance.Draw();
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            _spriteBatch.Draw(_renderTarget, RenderTargetRectangle, Color.White);
            _spriteBatch.End();
        }
    }
}