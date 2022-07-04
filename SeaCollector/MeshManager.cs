using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector
{
    
    public abstract class MeshLoader
    {
        public Dictionary<string, GameMesh> Resources = new Dictionary<string, GameMesh>();

        public bool Has(string name)
        {
            return Resources.ContainsKey(name);
        }

        public GameMesh Find(string name)
        {
            if (Has(name))
            {
                return Resources[name];
            }
            return Resources["missing"];
        }

        public void Add(string key, GameMesh value, bool overwrite = false)
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
    
    public class MeshManager : MeshLoader
    {
        public static MeshManager Instance { get; } = new MeshManager();

        static MeshManager()
        {
            
        }
        
        private MeshManager()
        {
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
        }
    }
}