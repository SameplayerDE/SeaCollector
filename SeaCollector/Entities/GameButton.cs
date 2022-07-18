using SeaCollector.Framework;

namespace SeaCollector.Entities
{
    public class GameButton : GameSpriteFont
    {

        private RectangleCollider2D _rectangleCollider2D;
        
        public GameButton(string spriteFontFile) : base(spriteFontFile)
        {
        }

        public override void Initialize()
        {
            _rectangleCollider2D = new RectangleCollider2D();
            _rectangleCollider2D.Width = (int)Width;
            _rectangleCollider2D.Height = (int)Height;
            AddChild(_rectangleCollider2D);
            base.Initialize();
        }
    }
}