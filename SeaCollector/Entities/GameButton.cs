using System;
using HxInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SeaCollector.Framework;

namespace SeaCollector.Entities
{
    public class GameButton : GameSpriteFont
    {
        private RectangleCollider2D _rectangleCollider2D;
        public event Action OnClick;
        public event Action OnEnter;
        public event Action OnLeave;

        public GameButton(string spriteFontFile) : base(spriteFontFile)
        {
        }

        public override void Initialize()
        {
            _rectangleCollider2D = new RectangleCollider2D();
            AddChild(_rectangleCollider2D);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SpriteFont != null)
            {
                _rectangleCollider2D.Width = (int)Width;
                _rectangleCollider2D.Height = (int)Height;


                if (GameSceneManager.Instance.RenderContext.RenderTargetRectangle.Contains(Mouse.GetState().Position))
                {
                    if (_rectangleCollider2D.Contains(Mouse.GetState().Position -
                                                      GameSceneManager.Instance.RenderContext.RenderTargetRectangle
                                                          .Location))
                    {
                        if (HxInput.Input.Instance.IsMouseKeyDown(MouseButton.Left))
                        {
                            OnClick();
                        }
                    }
                }
            }
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
        }
    }
}