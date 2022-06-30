using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using HxInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Entities;
using SeaCollector.HxPly;
using SeaCollector.Rendering;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SeaCollector
{
    public class Application : Game
    {
        public readonly string Version = "0.0.0";
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch = null!;

        private Random _random = new Random();
        
        private Effect _seaShader;
        private Effect _itemShader;
        private Effect _playerShader;
        private Effect _mapShader;

        private Matrix _view;
        private Matrix _projection;
        private Matrix _world;
        
        private ChaseCamera _camera;

        private float _fieldOfView;

        private GameMesh _ship;
        private GameMesh _sailMesh;
        private GameMesh _seaMesh;
        private GameMesh _itemMesh;
        private GameMesh _trashMesh;

        private Player _player;
        private GameObject _object;
        private GameObject _sea;

        private SoundEffect _soundEffect;
        
        private FastNoiseLite _noise = new FastNoiseLite();
        
        private FastNoiseLite _islandNoiseMain = new FastNoiseLite();
        private FastNoiseLite _islandNoiseFeatures0 = new FastNoiseLite();
        private FastNoiseLite _islandNoiseFeatures1 = new FastNoiseLite();

        private float[] _islandNoiseResultValues;

        private List<Item> _items = new List<Item>();

        private Texture2D _seaMap;
        private Texture2D _playerMap;
        private int _itemCounter = 0;
        private bool showMap = false;
        
        private ItemInstancing _instancing;

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
            
            _instancing = new ItemInstancing();
            _instancing.Initialize(GraphicsDevice);
            _instancing.Load(Content);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _ship = GameMesh.LoadFromFile(GraphicsDevice, "Content/ship.ply");
            _seaMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/sea.ply");
            _sailMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/sail.ply");
            _itemMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/cube.obj");
            _trashMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/trash.ply");
            
            _seaShader = Content.Load<Effect>("SeaShader");
            _itemShader = Content.Load<Effect>("ItemShader");
            _playerShader = Content.Load<Effect>("PlayerShader");
            _mapShader = Content.Load<Effect>("MapShader");

            _soundEffect = Content.Load<SoundEffect>("Sounds/collect");

            _playerMap = Content.Load<Texture2D>("player_Map");
            
            _player = new Player();
            _player.Mesh = _ship;

            _object = new GameObject();
            _object.Mesh = _sailMesh;

            _sea = new GameObject();
            _sea.Mesh = _seaMesh;

            _camera = new ChaseCamera(new Vector3(0, 15, 0), 
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0), GraphicsDevice);
            
            GenerateIslandNoise();
            
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _noise.SetFrequency(0.01f);
            
            _world = Matrix.Identity;
            
            _fieldOfView = 70f;

            _seaShader.Parameters["Texture"]?.SetValue(Content.Load<Texture>("waves"));

            for (var y = 0; y < 100; y++)
            {
                for (var x = 0; x < 100; x++)
                {
                    var noiseValue = _islandNoiseResultValues[x + 100 * y];
                    if (noiseValue > 0.9f)
                    {
                        _items.Add(new Item()
                            {
                                Mesh = _itemMesh,
                                Scale = new Vector3(1, 1, 1) * 0.25f,
                                Position = new Vector3(x, 0, y) * 0.5f
                            }
                        );
                    }
                }
            }

            base.LoadContent();
        }

        void updateCamera(GameTime gameTime)
        {

            // Rotate the camera
            Vector3 _cameraRotation = Vector3.Zero;
            if (Input.Instance.IsMouseKeyDown(MouseButton.Left))
            {
                _cameraRotation.Y -= Input.Instance.LatestMouseDelta.X / 10f;
                _cameraRotation.X -= Input.Instance.LatestMouseDelta.Y / 10f;
            }
            
            if (Input.Instance.LatestMouseScrollWheelDelta != 0)
            {
                _camera.PositionOffset = new Vector3(_camera.PositionOffset.X, _camera.PositionOffset.Y - Input.Instance.LatestMouseScrollWheelDelta / 100f, _camera.PositionOffset.Z);

                    
                //_cameraPosition += Vector3.Transform(Vector3.Forward, cameraRotationMatrix0) * ;
            }
            
            _camera.Rotate(_cameraRotation * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Window.Title = $"{_camera.RelativeCameraRotation.ToString()}, {_camera.TargetOffset.ToString()}";
            // Move the camera
            _camera.Move(_player.Position, _player.Rotation);
            // Update the camera
            _camera.Update();
        }
        
        private void GenerateIslandNoise()
        {
            _islandNoiseMain.SetNoiseType(FastNoiseLite.NoiseType.Cellular);

            _islandNoiseMain.SetCellularJitter(1f);
            _islandNoiseMain.SetFrequency(0.08f);
            _islandNoiseMain.SetSeed(DateTime.Now.Millisecond);
            
            _islandNoiseMain.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
            _islandNoiseMain.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);
            
            _islandNoiseMain.SetFractalType(FastNoiseLite.FractalType.None);
            
            _islandNoiseFeatures0.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _islandNoiseFeatures0.SetFrequency(0.04f);
            _islandNoiseFeatures0.SetFractalType(FastNoiseLite.FractalType.None);
            
            _islandNoiseFeatures0.SetSeed(DateTime.Now.Millisecond);
            
            _islandNoiseFeatures1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _islandNoiseFeatures1.SetFrequency(0.08f);
            _islandNoiseFeatures1.SetFractalType(FastNoiseLite.FractalType.None);
            
            _islandNoiseFeatures0.SetSeed(DateTime.Now.Millisecond);

            var texture = new Texture2D(GraphicsDevice, 100, 100);
            var data = new Color[100 * 100];
            _islandNoiseResultValues = new float[100 * 100];

            var min = 0f;
            var max = 0f;
            
            for (var y = 0; y < 100; y++)
            {
                for (var x = 0; x < 100; x++)
                {
                    var noiseMainValue = 1 - _islandNoiseMain.GetNoise(x, y);
                    var noiseFeatureValue0 = _islandNoiseFeatures0.GetNoise(x, y);

                    var noiseResult = noiseMainValue / noiseFeatureValue0 * 5;
                    noiseResult = Math.Clamp(noiseResult, 0, 1);

                    if (noiseResult < 1f)
                    {
                        //noiseResult = 0f;
                    }
                    
                    var color = new Color(noiseResult, noiseResult, noiseResult);
                    _islandNoiseResultValues[x + 100 * y] = noiseResult;

                    data[x + 100 * y] = color;
                    
                    if (noiseResult < min)
                    {
                        min = noiseResult;
                    }
                    if (noiseResult > max)
                    {
                        max = noiseResult;
                    }
                    
                }
            }

            texture.SetData(data);
            
            Stream stream = File.Create("file.png"); 
            texture.SaveAsPng( stream, texture.Width, texture.Height );
            stream.Dispose();

            _seaMap = texture;
            
        }

        protected override void Update(GameTime gameTime)
        {
            
            Input.Instance.Update(gameTime);
            
            
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            float movementSpeed = 2f;
            float rotationSpeed = 1f;

            var velocity = Vector3.Forward;
            var rotation = Vector3.Zero;
            
            //_player.Position.Y += (float)Math.Cos(totalTime + _player.Position.X + _player.Position.Y) * 0.05f;
            
            var playerRotationMatrix0 = Matrix.CreateRotationX(_player.Rotation.X) *
                                        Matrix.CreateRotationY(_player.Rotation.Y) *
                                        Matrix.CreateRotationZ(_player.Rotation.Z);
            
            var playerDirection0 = Vector3.Transform(Vector3.Forward, playerRotationMatrix0);

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //Forward
            if (keyboardState.IsKeyDown(Keys.W))
            {
                _player.ChangeSailFactor(0.2f * deltaTime);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                _player.ChangeSailFactor(-0.2f * deltaTime);
            }

            //Rotate Right
            if (keyboardState.IsKeyDown(Keys.D))
            {
                _player.ChangeSailDirection(0.1f * deltaTime);
                rotation.Y -= (rotationSpeed * deltaTime);
                
            }

            //Rotate Left
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _player.ChangeSailDirection(-0.1f * deltaTime);
                rotation.Y += (rotationSpeed * deltaTime);
                //rotation.Z += (rotationSpeed * deltaTime);
            }

            if (rotation.Length() != 0)
            {
                rotation.Normalize();
                rotation *= rotationSpeed;
                rotation *= deltaTime;
               
                _player.Rotation += rotation;
                
            }


            if (velocity.Length() != 0)
            {
                velocity.Normalize();
                velocity *= _player.SailFactor;
                velocity *= movementSpeed;
                velocity *= deltaTime;

                var rotationMatrix = Matrix.CreateRotationY(_player.Rotation.Y);

                _player.Position += Vector3.Transform(velocity, rotationMatrix);
            }

            for (var index = _items.Count - 1; index >= 0; index--)
            {
                var item = _items[index];
                var distance = Vector3.Distance(_player.Position, item.Position);
                var direction = Vector3.Subtract(_player.Position, item.Position);
                if (distance > 0.5f && distance < 2f)
                {
                    item.Position += direction * 7 * deltaTime;
                    item.Scale *= Math.Clamp(Math.Min(distance, 1f), 0.1f, 1f);
                }
                else if (distance < 0.5f)
                {
                    _items.Remove(item);
                    _itemCounter++;
                    //_player.Scale += new Vector3(0.001f, 0.001f, 0.001f);
                    //_sea.Scale += new Vector3(0.001f, 0.001f, 0.001f);
                    //_player.Scale.X = Math.Clamp(_player.Scale.X, 1, 3); 
                    //_player.Scale.Y = Math.Clamp(_player.Scale.Y, 1, 3); 
                    //_player.Scale.Z = Math.Clamp(_player.Scale.Z, 1, 3); 
                    //_soundEffect.Play(0.5f, _random.NextFloat(0, 1), 0);
                }
            }

            // ReSharper disable once HeapView.BoxingAllocation
            //Window.Title = $"{1 / (float)gameTime.ElapsedGameTime.TotalSeconds}";

            updateCamera(gameTime);
            // = _player.Position + new Vector3(0, 15, 1) + _cameraOffset;
            _sea.Position = _player.Position;
            _object.Position = _player.Position;
            _object.Scale = new Vector3(1, _player.SailFactor, 1);
            _object.Rotation = new Vector3(0, 0, 0) + _player.Rotation;
            _seaShader.Parameters["Offset"]?.SetValue(-new Vector2(_player.Position.X, -_player.Position.Z));

            _view = Matrix.CreateLookAt(
                _camera.Position,
                _player.Position,
                Vector3.Up
            );

            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(_fieldOfView),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            );

            _camera.Frustum = new BoundingFrustum(_view * _projection);

            _playerShader.Parameters["World"]?.SetValue(_world);
            _playerShader.Parameters["View"]?.SetValue(_view);
            _playerShader.Parameters["Projection"]?.SetValue(_projection);

            _playerShader.Parameters["Delta"]?.SetValue(deltaTime);
            _playerShader.Parameters["Total"]?.SetValue(totalTime);

            _playerShader.Parameters["CameraPosition"]?.SetValue(_camera.Position);
            
            _seaShader.Parameters["World"]?.SetValue(_world);
            _seaShader.Parameters["View"]?.SetValue(_view);
            _seaShader.Parameters["Projection"]?.SetValue(_projection);

            _seaShader.Parameters["Delta"]?.SetValue(deltaTime);
            _seaShader.Parameters["Total"]?.SetValue(totalTime);
            
            _seaShader.Parameters["CameraPosition"]?.SetValue(_player.Position);
            
            _itemShader.Parameters["World"]?.SetValue(_world);
            _itemShader.Parameters["View"]?.SetValue(_view);
            _itemShader.Parameters["Projection"]?.SetValue(_projection);

            _itemShader.Parameters["Delta"]?.SetValue(deltaTime);
            _itemShader.Parameters["Total"]?.SetValue(totalTime);
            
            _itemShader.Parameters["CameraPosition"]?.SetValue(_camera.Position);
            _itemShader.Parameters["PlayerPosition"]?.SetValue(_player.Position);
            _itemShader.Parameters["FogCenter"]?.SetValue(_player.Position);

            if (Input.Instance.IsKeyboardKeyDownOnce(Keys.M))
            {
                showMap = !showMap;
            }
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(78, 202, 255));

            _player.Draw(GraphicsDevice, _playerShader, _world, _view, _projection);
            _object.Draw(GraphicsDevice, _playerShader, _world, _view, _projection);
            _sea.Draw(GraphicsDevice, _seaShader, _world, _view, _projection);

            _instancing.Draw(ref _world, ref _view, ref _projection, _camera.Position, GraphicsDevice);
            
            /*for (var index = _items.Count - 1; index >= 0; index--)
            {
                var item = _items[index];
                var distance = Vector3.Distance(_player.Position, item.Position);
                if (distance < 100f)
                {
                    if (_camera.BoundingVolumeIsInView(item.Position))
                    {
                        item.Draw(GraphicsDevice, _itemShader, _world, _view, _projection);
                    }
                }
            }

            if (showMap)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _mapShader.Techniques["SpriteDrawing"].Passes[0].Apply();
                _spriteBatch.Draw(_seaMap, new Rectangle(25, 25, 200, 200), Color.White);
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(_playerMap, new Vector2(25 + _player.Position.X * 4, 25 + _player.Position.Z * 4),
                    null, Color.White, -_player.Rotation.Y, new Vector2(16, 16), 0.5f, SpriteEffects.None, 1f);
                _spriteBatch.End();
            }*/
        }
    }
}