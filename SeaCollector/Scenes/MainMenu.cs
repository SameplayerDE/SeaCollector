using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Entities;
using SeaCollector.Framework;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector.Scenes
{
    public class MainMenu : GameScene
    {

        private BillboardSystem _forest;
        private Hero _hero;
        private FixedPerspectiveCamera _camera;
        private GameSprite _gameSprite;
        private GameSpriteFont _gameSpriteFont;
        
        public MainMenu(Game game) : base("menu", game)
        {
        }

        public override void Initialize()
        {
            _forest = new BillboardSystem(Game.GraphicsDevice, Vector2.One * 10, "Textures/tree");
            _forest.Mode = BillboardMode.Cylindrical;
            
            _camera = new FixedPerspectiveCamera(Game.GraphicsDevice);
            _camera.Translate(new Vector3(0, 2, 2));

            _hero = new Hero();
            _hero.Translate(0.0f, 0.0f, -100f);
            _hero.Scale(0.8f, 0.8f, 0.8f);
            
            _hero.AddChild(_camera);

            _gameSprite = new GameSprite("Textures/tree");
            _gameSprite.DrawInFrontOf3D = false;
            _gameSprite.Depth = 1;

            _gameSpriteFont = new GameSpriteFont("Fonts/Default");
            _gameSpriteFont.Text = "Hallo Patrick";
            _gameSpriteFont.Depth = 0;
            _gameSpriteFont.DrawInFrontOf3D = false;
            
            AddSceneObject(_hero);
            AddSceneObject(_forest);
            AddSceneObject(_gameSprite);
            AddSceneObject(_gameSpriteFont);

            GameSceneManager.Instance.RenderContext.Camera = _camera;
            
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _gameSprite.LocalPosition.X += Time.Instance.DeltaSecondsF;
        }

        public override void Draw3D(RenderContext renderContext)
        {
            base.Draw3D(renderContext);
        }

        /*public override void Draw3D(GraphicsDevice graphicsDevice, Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            graphicsDevice.Clear(new Color(78, 202, 255));
            base.Draw3D(graphicsDevice, effect, world, _camera.View, _camera.Projection);
        }*/
    }
}