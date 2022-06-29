using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector
{
    
    public abstract class TextureLoader
    {
        public Dictionary<string, Texture2D> Resources = new Dictionary<string, Texture2D>();

        public bool Has(string name)
        {
            return Resources.ContainsKey(name);
        }

        public Texture2D Find(string name)
        {
            if (Has(name))
            {
                return Resources[name];
            }
            return Resources["missing"];
        }

        public void Add(string key, Texture2D value, bool overwrite = false)
        {
            if (overwrite)
            {
                Resources[key] = value;
            }
            else
            {
                Resources.Add(key, value);
            }
        }

        public abstract void LoadContent(ContentManager contentManager);
    }
    
    public class TextureManager : TextureLoader
    {
        public static TextureManager Instance { get; } = new TextureManager();

        static TextureManager()
        {
            
        }
        
        private TextureManager()
        {
            
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
        }
    }

    public static class TextureManagerSpriteBatchExpander
    {
        public static void Draw(this SpriteBatch spriteBatch, string key, Vector2 position, Color color)
        {
            var texture = TextureManager.Instance.Find(key);
            spriteBatch.Draw(texture, position, color);
        }
    }
}