using System;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Entities;
using SeaCollector.Framework;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector.Scenes
{
    public class DemoScene : GameScene
    {
        private BillboardSystem _forest;
        private Hero _hero;
        private FixedPerspectiveCamera _camera;

        private GameSpriteFont _position;
        private GameSpriteFont _rotation;
        
        private GameSprite3D _sprite3D0;
        private GameSprite3D _sprite3D1;
        
        private GameSprite3D _ground;
        private GameObject3D _cameraParent;

        private GameMeshObject _test;
        private RotationObject3D _rotationObject3D;

        public DemoScene(Game game) : base("demo", game)
        {
        }

        public override void Initialize()
        {
            _cameraParent = new GameObject3D();
            
            _rotationObject3D = new RotationObject3D();
            _rotationObject3D.SpeedY = 1.5f;
            
            _test = new GameMeshObject("Models/Hulls/sp_hul01", "Effects/TextureCellShader", "Content/Models/Hulls/hull_0.obj");
            _test.Scale(Vector3.One * 7f);
            
            _forest = new BillboardSystem(Game.GraphicsDevice, Vector2.One * 3f, "Textures/tree");
            _forest.Mode = BillboardMode.Spherical;

            _sprite3D0 = new GameSprite3D(Game.GraphicsDevice, Vector2.One * 10f, "Textures/tree");
            _sprite3D0.Translate(10, 0.5f * 10f, 10);
            _sprite3D1 = new GameSprite3D(Game.GraphicsDevice, Vector2.One * 10f, "Textures/tree");
            _sprite3D1.Rotate(0, MathHelper.ToRadians(90), 0);
            
            _ground = new GameSprite3D(Game.GraphicsDevice, Vector2.One * 1000f, "Textures/gras");
            _ground.Rotate(MathHelper.ToRadians(90f), 0, 0);
            _ground.EnsureOcclusion = false;
            _ground.Tiling = new Vector2(1000f, 1000f);
            
            _camera = new FixedPerspectiveCamera(Game.GraphicsDevice);
            _camera.Translate(new Vector3(0, 5, 5));
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
            
            _cameraParent.AddChild(_camera);
            _hero.AddChild(_cameraParent);
            
            AddSceneObject(_hero);
            
            AddSceneObject(_rotationObject3D);
            AddSceneObject(_forest);
            AddSceneObject(_cameraParent);
            AddSceneObject(_ground);

            //AddSceneObject(_position);
            //AddSceneObject(_rotation);

            GameSceneManager.Instance.RenderContext.Camera = _camera;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var (tx, ty, tz) = _hero.WorldPosition;
            var (rx, ry, rz, rw) = _cameraParent.WorldRotation;
            //_position.Text = $"Position:\nX: {tx}\nY: {ty}\nZ: {tz}\n";
            //_rotation.Text = $"Rotation:\nX: {rx}\nY: {ry}\nZ: {rz}\nW: {rw}";

            //_rotation.LocalPosition.Y = _position.Height;
            //_cameraParent.LocalPosition = _hero.LocalPosition;
        }
    }
}