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
        private GameSprite3D _sprite3D;
        private GameSprite3D _ground;

        private GameMeshObject _test;

        public DemoScene(Game game) : base("demo", game)
        {
        }

        public override void Initialize()
        {
            _test = new GameMeshObject("Models/Hulls/sp_hul01", "Effects/TextureCellShader", "Content/Models/Hulls/hull_0.obj");
            _test.Scale(4, 4, 4);
            
            _forest = new BillboardSystem(Game.GraphicsDevice, Vector2.One, "Textures/stone");
            _forest.Mode = BillboardMode.Spherical;

            _sprite3D = new GameSprite3D(Game.GraphicsDevice, Vector2.One, "Textures/tree");
            _ground = new GameSprite3D(Game.GraphicsDevice, Vector2.One * 1000f, "Textures/gras");
            _ground.Rotate(MathHelper.ToRadians(90f), 0, 0);
            _ground.EnsureOcclusion = false;
            _ground.Tiling = new Vector2(1000f, 1000f);
            
            _camera = new FixedPerspectiveCamera(Game.GraphicsDevice);
            _camera.Translate(new Vector3(0, 10, 10));
            _camera.Rotate(MathHelper.ToRadians(-45f), 0, 0);

            _hero = new Hero();
            _hero.Translate(0.0f, 0.0f, 0f);
            _hero.Scale(0.8f, 0.8f, 0.8f);

            _hero.AddChild(_camera);
            //AddSceneObject(_camera);

            _position = new GameSpriteFont("Fonts/Default");
            _rotation = new GameSpriteFont("Fonts/Default");

            _position.AddChild(_rotation);
            
            AddSceneObject(_hero);
            AddSceneObject(_sprite3D);
            AddSceneObject(_forest);
            AddSceneObject(_test);
            AddSceneObject(_ground);
            

            AddSceneObject(_position);
            AddSceneObject(_rotation);

            GameSceneManager.Instance.RenderContext.Camera = _camera;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var (tx, ty, tz) = _hero.WorldPosition;
            var (rx, ry, rz, rw) = _hero.WorldRotation;
            
            _position.Text = $"Position:\nX: {tx}\nY: {ty}\nZ: {tz}\n";
            _rotation.Text = $"Rotation:\nX: {rx}\nY: {ry}\nZ: {rz}\nW: {rw}";

            _rotation.LocalPosition.Y = _position.Height;
        }
    }
}