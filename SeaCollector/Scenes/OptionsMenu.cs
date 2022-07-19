using System;
using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Entities;
using SeaCollector.Framework;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector.Scenes
{
    public class OptionsMenu : GameScene
    {
        private GameButton _back;

        public OptionsMenu(Game game) : base("options", game)
        {
        }

        public override void Initialize()
        {
            
            _back = new GameButton("Fonts/Default");
            _back.Text = "Back";
            _back.OnClick += BackClicked;
            _back.Translate(0, 0);
            
            AddSceneObject(_back);
            
            base.Initialize();
        }
        
        private void BackClicked()
        {
            GameSceneManager.Instance.Stage("menu");
            GameSceneManager.Instance.Grab();
        }
    }
}