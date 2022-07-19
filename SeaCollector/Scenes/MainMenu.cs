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
        private GameButton _options;
        private GameButton _exit;

        public MainMenu(Game game) : base("menu", game)
        {
        }

        public override void Initialize()
        {
            
            _start = new GameButton("Fonts/Default");
            _start.Text = "Start";
            _start.OnClick += StartClicked;
            _start.Translate(0, 0);
            
            _options = new GameButton("Fonts/Default");
            _options.Text = "Options";
            _options.OnClick += OptionsClicked;
            _options.Translate(0, 64);
            
            _exit = new GameButton("Fonts/Default");
            _exit.Text = "Exit";
            _exit.OnClick += ExitClicked;
            _exit.Translate(0, 128);
            
            AddSceneObject(_start);
            AddSceneObject(_options);
            AddSceneObject(_exit);
            //GameSceneManager.Instance.RenderContext.Camera = _camera;

            base.Initialize();
        }

        private void OptionsClicked()
        {
            GameSceneManager.Instance.Stage("options");
            GameSceneManager.Instance.Grab();
        }

        private void ExitClicked()
        {
            Game.Exit();
        }

        private void StartClicked()
        {
            GameSceneManager.Instance.Stage("demo");
            GameSceneManager.Instance.Grab();
        }
    }
}