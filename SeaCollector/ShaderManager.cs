using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector
{
    
    public abstract class ShaderLoader
    {
        public Dictionary<string, Effect> Resources = new Dictionary<string, Effect>();

        public bool Has(string name)
        {
            return Resources.ContainsKey(name);
        }

        public Effect Find(string name)
        {
            return Has(name) ? Resources[name] : Resources["missing"];
        }

        public void Add(string key, Effect value, bool overwrite = false)
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
    
    public class ShaderManager : ShaderLoader
    {
        public static ShaderManager Instance { get; } = new ShaderManager();

        static ShaderManager()
        {
            
        }
        
        private ShaderManager()
        {
            
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
        }
    }
}