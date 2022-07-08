using System;
using System.Collections.Generic;
using System.IO;
using HxInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Entities;
using SeaCollector.Rendering;

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
        private Effect _diffuseShader;
        private Effect _islandShader;

        private Matrix _view;
        private Matrix _projection;
        private Matrix _world;
        
        private ChaseCamera _camera;

        private bool gen = false;
        
        private float _fieldOfView;

        private GameMesh _ship;
        private GameMesh _sailMesh;
        private GameMesh _seaMesh;
        private GameMesh _itemMesh;
        private GameMesh _seaPlaneMesh;

        private Player _player;
        private GameObject _object;
        private GameObject _sea;
        private GameObject _seaPlane;

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
        private bool _noiseInit = false;
        
        private ItemInstancing _instancing;
        private Terrain _terrain;

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

            _islandNoiseMain.SetSeed(DateTime.Now.Millisecond);
            
            GenerateIslandNoise(0, 0);
            
            _instancing = new ItemInstancing(_islandNoiseResultValues);
            _instancing.Initialize(GraphicsDevice);
            _instancing.Load(Content);

            _terrain = new Terrain();
            _terrain.Generate(GraphicsDevice, 100, 100);
            _terrain.SetHeight(_islandNoiseResultValues);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _ship = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/ship.ply");
            _seaMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/sea.ply");
            _sailMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/sail.ply");
            _itemMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/cube.obj");
            _seaPlaneMesh = GameMesh.LoadFromFile(GraphicsDevice, "Content/Models/sea_plane.ply");
            
            _seaShader = Content.Load<Effect>("Effects/SeaShader");
            _itemShader = Content.Load<Effect>("Effects/ItemShader");
            _playerShader = Content.Load<Effect>("Effects/PlayerShader");
            _mapShader = Content.Load<Effect>("Effects/MapShader");
            _diffuseShader = Content.Load<Effect>("Effects/DiffuseShader");
            _islandShader = Content.Load<Effect>("Effects/IslandShader");

            _soundEffect = Content.Load<SoundEffect>("Sounds/collect");

            _playerMap = Content.Load<Texture2D>("Textures/player_Map");

            _seaPlane = new GameObject();
            _seaPlane.Mesh = _seaPlaneMesh;
            
            _player = new Player();
            _player.Mesh = _ship;

            _object = new GameObject();
            _object.Mesh = _sailMesh;

            _sea = new GameObject();
            _sea.Mesh = _seaMesh;

            _camera = new ChaseCamera(new Vector3(0, 15, 0), 
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0), GraphicsDevice);
            
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _noise.SetFrequency(0.01f);
            
            _world = Matrix.Identity;
            
            _fieldOfView = 70f;

            _seaShader.Parameters["Texture"]?.SetValue(Content.Load<Texture>("Textures/waves"));
            _mapShader.Parameters["Texture01"]?.SetValue(Content.Load<Texture>("Textures/water_map"));

            /*for (var y = 0; y < 100; y++)
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
            }*/

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
            _camera.Move(new Vector3(_player.Position.X, 0, _player.Position.Z), _player.Rotation);
            // Update the camera
            _camera.Update();
        }
        
        private void GenerateIslandNoise(float chunkX, float chunkZ)
        {
            if (!_noiseInit)
            {
                InitNoiseSettings();
            }
            

            var min = 0f;
            var max = 0f;
            
            for (var y = 0; y < 100; y++)
            {
                for (var x = 0; x < 100; x++)
                {
                    var noiseResult = 1f;
                    
                    //var noiseMainValue = -_islandNoiseMain.GetNoise(x + 100 * chunkX, y + 100 * chunkZ);
                    var noiseMainValue = -_islandNoiseMain.GetNoise(x + chunkX, y + chunkZ);
                    var noiseFeatureValue0 = _islandNoiseFeatures0.GetNoise(x, y);

                    noiseResult = noiseMainValue;
                    
                    //noiseResult = noiseMainValue / noiseFeatureValue0 * 5;
                    //noiseResult = Math.Clamp(noiseResult, 0, 1);

                    if (noiseResult < 0.2f)
                    {
                        //noiseResult = -0.1f;
                    }
                    else
                    {
                        //noiseResult = 1f;
                    }
                    
                    var color = new Color(noiseResult, noiseResult, noiseResult);
                    _islandNoiseResultValues[x + 100 * y] = noiseResult;

                    //data[x + 100 * y] = color;
                    
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

            
            /*texture.SetData(data);
            
            Stream stream = File.Create("file.png"); 
            texture.SaveAsPng( stream, texture.Width, texture.Height );
            stream.Dispose();

            _seaMap = texture;*/
            
        }

        private void InitNoiseSettings()
        {
            _islandNoiseMain.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

            _islandNoiseMain.SetCellularJitter(1f);
            _islandNoiseMain.SetFrequency(0.02f);
            _islandNoiseMain.SetFractalOctaves(20);
            _islandNoiseMain.SetFractalType(FastNoiseLite.FractalType.FBm);
            
            
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

            //var texture = new Texture2D(GraphicsDevice, 100, 100);
            //var data = new Color[100 * 100];
            _islandNoiseResultValues = new float[100 * 100];
            _noiseInit = true;
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

                var oldX = _player.Position.X;
                _player.Position += Vector3.Transform(velocity, rotationMatrix);
                GenerateIslandNoise(_player.Position.X, _player.Position.Z);
                _terrain.SetHeight(_islandNoiseResultValues);
            }
/*
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
            }*/

            // ReSharper disable once HeapView.BoxingAllocation
            //Window.Title = $"{1 / (float)gameTime.ElapsedGameTime.TotalSeconds}";
            if (_player.Position.X > 100 && gen)
            {
                GenerateIslandNoise(1, 0);
                _terrain.SetHeight(_islandNoiseResultValues);
                _terrain.Position = new Vector3(100, 0, 0);
                gen = false;
            }

            updateCamera(gameTime);
            // = _player.Position + new Vector3(0, 15, 1) + _cameraOffset;
            _sea.Position = new Vector3(_player.Position.X, 0, _player.Position.Z);
            _seaPlane.Position = new Vector3(_player.Position.X, 0, _player.Position.Z);
            _terrain.Position = new Vector3(-50 + _player.Position.X, 0, -50 + _player.Position.Z);
            _object.Position = new Vector3(_player.Position.X, _player.Position.Y, _player.Position.Z);
            _object.Scale = new Vector3(1, _player.SailFactor, 1);
            _object.Rotation = new Vector3(0, 0, 0) + _player.Rotation;
            _seaShader.Parameters["Offset"]?.SetValue(-new Vector2(_player.Position.X, -_player.Position.Z));
            
            _view = Matrix.CreateLookAt(
                _camera.Position,
                new Vector3(_player.Position.X, 0, _player.Position.Z),
                Vector3.Up
            );

            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(_fieldOfView),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            );

            _camera.Frustum = new BoundingFrustum(_view * _projection);

            _diffuseShader.Parameters["World"]?.SetValue(_world);
            _diffuseShader.Parameters["View"]?.SetValue(_view);
            _diffuseShader.Parameters["Projection"]?.SetValue(_projection);
            
            _islandShader.Parameters["World"]?.SetValue(_world);
            _islandShader.Parameters["View"]?.SetValue(_view);
            _islandShader.Parameters["Projection"]?.SetValue(_projection);
            
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
            
            _seaShader.Parameters["CameraPosition"]?.SetValue(new Vector3(_player.Position.X, 0, _player.Position.Z));
            
            _itemShader.Parameters["World"]?.SetValue(_world);
            _itemShader.Parameters["View"]?.SetValue(_view);
            _itemShader.Parameters["Projection"]?.SetValue(_projection);

            _itemShader.Parameters["Delta"]?.SetValue(deltaTime);
            _itemShader.Parameters["Total"]?.SetValue(totalTime);
            
            _itemShader.Parameters["CameraPosition"]?.SetValue(_camera.Position);
            _itemShader.Parameters["PlayerPosition"]?.SetValue(new Vector3(_player.Position.X, 0, _player.Position.Z));
            _itemShader.Parameters["FogCenter"]?.SetValue(new Vector3(_player.Position.X, 0, _player.Position.Z));

            if (Input.Instance.IsKeyboardKeyDownOnce(Keys.M))
            {
                showMap = !showMap;
            }

            _player.Position.Y = (float)Math.Cos(totalTime * 2f) * 0.05f;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(78, 202, 255));

            _player.Draw(GraphicsDevice, _playerShader, _world, _view, _projection);
            _object.Draw(GraphicsDevice, _playerShader, _world, _view, _projection);
            _sea.Draw(GraphicsDevice, _seaShader, _world, _view, _projection);
            _seaPlane.Draw(GraphicsDevice, _diffuseShader, _world, _view, _projection);

            _instancing.Draw(_camera, ref _world, ref _view, ref _projection, _camera.Position, _player.Position, _player.Position, GraphicsDevice, gameTime);
            
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
            }*/
            
            _terrain.Draw(_islandShader, _world, _view, _projection);
            
            if (showMap)
            {
                 const int mapScale = 4;
                _spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _mapShader.Techniques["SpriteDrawing"].Passes[0].Apply();
                _spriteBatch.Draw(_seaMap, new Rectangle(25, 25, 100 * mapScale, 100 * mapScale), Color.White);
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(_playerMap, new Vector2(25 + (_player.Position.X * mapScale), 25 + (_player.Position.Z * mapScale)),
                    null, Color.White, 0f, new Vector2(16, 16), 1f, SpriteEffects.None, 1f);
                _spriteBatch.End();
            }
        }
    }
}