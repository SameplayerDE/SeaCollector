using System;
using HxInput;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Entities;
using SeaCollector.Framework;
using SeaCollector.Rendering.Cameras;
using SharpDX;
using BoundingSphere = Microsoft.Xna.Framework.BoundingSphere;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Point = Microsoft.Xna.Framework.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Viewport = Microsoft.Xna.Framework.Graphics.Viewport;

namespace SeaCollector.Scenes
{
    public class DemoScene : GameScene
    {
        private BillboardSystem _stones;
        private GameMeshInstanceSystem _forest;
        private Hero _hero;
        private FixedPerspectiveCamera _camera;

        private GameSpriteFont _position;
        private GameSpriteFont _rotation;

        private AnimatedGameSprite3D _animated;

        private GameSprite3D _sprite3D0;
        private GameSprite3D _sprite3D1;

        private ThrowableObject[] _billboardObject;

        private GameSprite3D _ground;
        private GameObject3D _cameraParent;

        private GameMeshObject _test;
        private RotationObject3D _rotationObject3D;

        private bool _onHead;

        private Random _random;

        public DemoScene(Game game) : base("demo", game)
        {
        }

        public override void Initialize()
        {
            _random = new Random();

            _cameraParent = new GameObject3D();

            _rotationObject3D = new RotationObject3D();
            _rotationObject3D.SpeedY = 1.5f;

            _test = new GameMeshObject("Models/Hulls/sp_hul01", "Effects/TextureCellShader",
                "Content/Models/Hulls/hull_0.obj");
            _test.Scale(Vector3.One * 7f);

            _forest = new GameMeshInstanceSystem(Game.GraphicsDevice, "Textures/tree_diffuse",
                "Content/Models/tree.obj");
            _forest.EnsureOcclusion = true;

            _stones = new BillboardSystem(Game.GraphicsDevice, Vector2.One, "Textures/stone");

            _animated = new AnimatedGameSprite3D(Game.GraphicsDevice, Vector2.One, "Textures/waves", new Point(10, 10));
            
            _sprite3D0 = new GameSprite3D(Game.GraphicsDevice, Vector2.One * 10f, "Textures/tree");
            _sprite3D0.Translate(10, 0.5f * 10f, 10);
            _sprite3D1 = new GameSprite3D(Game.GraphicsDevice, Vector2.One * 10f, "Textures/tree");
            _sprite3D1.Rotate(0, MathHelper.ToRadians(90), 0);

            _billboardObject = new ThrowableObject[1000];

            for (int i = 0; i < _billboardObject.Length; i++)
            {
                _billboardObject[i] = new ThrowableObject(Game.GraphicsDevice, Vector2.One * 0.7f, "Textures/stone");
                _billboardObject[i].Translate(RandomUtil.NextFloat(_random, -100f, 100f), 0f,
                    RandomUtil.NextFloat(_random, -100f, 100f));

                AddSceneObject(_billboardObject[i]);
            }


            _ground = new GameSprite3D(Game.GraphicsDevice, Vector2.One * 1000f, "Textures/gras");
            _ground.Rotate(MathHelper.ToRadians(90f), 0, 0);
            _ground.EnsureOcclusion = true;
            _ground.Tiling = new Vector2(1000f, 1000f);

            _camera = new FixedPerspectiveCamera(Game.GraphicsDevice);
            _camera.Translate(new Vector3(0, 3, 3));
            _camera.Rotate(MathHelper.ToRadians(-45f), 0, 0);

            _hero = new Hero();
            _hero.Translate(0.0f, 0.0f, 0f);
            _hero.Scale(Vector3.One * 1f);

            _cameraParent.FixedRotation = true;

            _position = new GameSpriteFont("Fonts/Default");
            _rotation = new GameSpriteFont("Fonts/Default");

            //_position.AddChild(_rotation);

            _rotationObject3D.AddChild(_test);

            _sprite3D0.AddChild(_sprite3D1);
            AddSceneObject(_sprite3D0);
            AddSceneObject(_animated);

            _cameraParent.AddChild(_camera);
            // _cameraParent.AddChild(_billboardObject);
            _hero.AddChild(_cameraParent);
            _animated.Translate(0, 2f, 0f);
            _hero.AddChild(_animated);
            //_hero.AddChild(_billboardObject);

            AddSceneObject(_hero);

            //AddSceneObject(_rotationObject3D);

            AddSceneObject(_cameraParent);
            AddSceneObject(_forest);

            AddSceneObject(_ground);

            //AddSceneObject(_position);
            //AddSceneObject(_rotation);

            GameSceneManager.Instance.RenderContext.Camera = _camera;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _animated.Cell.X = gameTime.TotalGameTime.Seconds;
            base.Update(gameTime);
            //var (tx, ty, tz) = _hero.WorldPosition;
            //var (rx, ry, rz, rw) = _cameraParent.WorldRotation;
            //_position.Text = $"Position:\nX: {tx}\nY: {ty}\nZ: {tz}\n";
            //_rotation.Text = $"Rotation:\nX: {rx}\nY: {ry}\nZ: {rz}\nW: {rw}";

            //_rotation.LocalPosition.Y = _position.Height;
            //_cameraParent.LocalPosition = _hero.LocalPosition;


            /*Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Viewport viewport = Game.GraphicsDevice.Viewport;

            float distance = _camera.FarPlane;
            ThrowableObject target = null;

            foreach (var throwableObject in _billboardObject)
            {
                if (GameMath.Intersects(mouseLocation, new BoundingSphere(throwableObject.WorldPosition, 0.5f),
                        Matrix.Identity, _camera.View, _camera.Projection, viewport))
                {
                    var tempdistance = GameMath.IntersectDistance(
                        new BoundingSphere(throwableObject.WorldPosition, 0.5f), mouseLocation, _camera.View,
                        _camera.Projection, viewport);

                    if (tempdistance != null)
                    {
                        if (distance > tempdistance)
                        {
                            distance = (float)tempdistance;
                            target = throwableObject;
                        }
                    }
                }
                throwableObject.Color = Color.White;
            }

            
            if (target != null) target.Color = Color.Red;

            /*
             
             var distance = Vector3.Distance(_hero.WorldPosition, _billboardObject.WorldPosition);

             if (distance < .5f && !_onHead && HxInput.Input.Instance.IsKeyboardKeyDownOnce(Keys.Space))
            {
                RemoveSceneObject(_billboardObject);
                _cameraParent.AddChild(_billboardObject);
                _billboardObject.Translate(0, 1.1f, 0);
                _onHead = true;
                _billboardObject.Held = true;
                _billboardObject.OnGround = false;
            }
            else if (_onHead && HxInput.Input.Instance.IsKeyboardKeyDownOnce(Keys.Space))
            {
                AddSceneObject(_billboardObject);
                _cameraParent.RemoveChild(_billboardObject);

                var newPos = _billboardObject.LocalPosition + _hero.WorldPosition;

                _billboardObject.Translate(newPos);
                _billboardObject.Direction = Vector3.Transform(Vector3.Forward, _hero.WorldRotation) * 2f;
                _billboardObject.Direction.Y = 1;
                _onHead = false;
                _billboardObject.Held = false;
                _billboardObject.InAir = true;
            }*/
        }
    }
}