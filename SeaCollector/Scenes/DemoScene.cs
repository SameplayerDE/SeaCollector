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

        public DemoScene(Game game) : base("demo", game)
        {
        }

        public override void Initialize()
        {
            _forest = new BillboardSystem(Game.GraphicsDevice, Vector2.One, "Textures/stone");
            _forest.Mode = BillboardMode.Cylindrical;

            _camera = new FixedPerspectiveCamera(Game.GraphicsDevice);
            _camera.Translate(new Vector3(0, 2, 2));

            _hero = new Hero();
            _hero.Translate(0.0f, 0.0f, 0f);
            _hero.Scale(0.8f, 0.8f, 0.8f);

            _hero.AddChild(_camera);

            _position = new GameSpriteFont("Fonts/Default");
            _rotation = new GameSpriteFont("Fonts/Default");

            _position.AddChild(_rotation);

            AddSceneObject(_hero);
            AddSceneObject(_forest);

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