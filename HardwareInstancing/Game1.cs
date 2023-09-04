using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HardwareInstancing
{
    public class Game1 : Game
    {
        private Instancing _instancing;

        public readonly string Version = "0.0.0";
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch = null!;

        private Matrix _view;
        private Matrix _projection;
        private Matrix _world;

        private Vector3 _cameraPosition;
        private Vector3 _cameraRotation;
        private float _fieldOfView;

        public Game1()
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

            _instancing = new Instancing();
            _instancing.Initialize(GraphicsDevice);
            _instancing.Load(Content);

            _fieldOfView = 48f;
            _world = Matrix.Identity;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            const float movementSpeed = 2f;
            const float rotationSpeed = 0.5f;

            var velocity = Vector3.Zero;
            var rotation = Vector3.Zero;

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (keyboardState.IsKeyUp(Keys.LeftShift))
            {
                //Forward
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    velocity.Z -= 1f;
                }

                //Right
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    velocity.X += 1f;
                }

                //Backwards
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    velocity.Z += 1f;
                }

                //Left
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    velocity.X -= 1f;
                }

                //Up
                if (keyboardState.IsKeyDown(Keys.PageUp))
                {
                    velocity.Y += 1f;
                }

                //Down
                if (keyboardState.IsKeyDown(Keys.PageDown))
                {
                    velocity.Y -= 1f;
                }
            }
            else
            {
                //Up
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    rotation.X += 1f;
                }

                //Down
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    rotation.X -= 1f;
                }

                //Rotate Right
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    rotation.Y -= 1f;
                }

                //Rotate Left
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    rotation.Y += 1f;
                }
            }

            if (rotation.Length() != 0)
            {
                rotation.Normalize();
                rotation *= rotationSpeed;
                rotation *= deltaTime;

                _cameraRotation += rotation;
            }


            if (velocity.Length() != 0)
            {
                velocity.Normalize();
                velocity *= movementSpeed;
                velocity *= deltaTime;

                var rotationMatrix = Matrix.CreateRotationY(_cameraRotation.Y);

                _cameraPosition += Vector3.Transform(velocity, rotationMatrix);
            }

            var cameraRotationMatrix = Matrix.CreateRotationX(_cameraRotation.X) *
                                       Matrix.CreateRotationY(_cameraRotation.Y) *
                                       Matrix.CreateRotationZ(_cameraRotation.Z);
            var cameraDirection = Vector3.Transform(Vector3.Forward, cameraRotationMatrix);

            _view = Matrix.CreateLookAt(
                _cameraPosition,
                _cameraPosition + cameraDirection,
                Vector3.Up
            );

            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(_fieldOfView),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            );
        }

        protected override void Draw(GameTime gameTime)
        {
            var (x, y) = GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2;
            
            _instancing.Draw(ref _view, ref _projection, GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}