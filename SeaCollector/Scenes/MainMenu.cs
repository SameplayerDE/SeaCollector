using System;
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
        private GameButton _start;

        public MainMenu(Game game) : base("menu", game)
        {
        }

        public override void Initialize()
        {
            
            _start = new GameButton("Fonts/Default");
            _start.Text = "Start";
            _start.OnClick += StartClicked;
            _start.Translate(128, 96);

            AddSceneObject(_start);
            //GameSceneManager.Instance.RenderContext.Camera = _camera;

            base.Initialize();
        }

        private void StartClicked()
        {
            GameSceneManager.Instance.Stage("demo");
            GameSceneManager.Instance.Grab();
        }
    }
}