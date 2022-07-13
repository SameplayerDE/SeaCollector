using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Worlds
{
    public class WorldManager
    {
        
        public Dictionary<string, World> Resources = new Dictionary<string, World>();

        public static WorldManager Instance { get; } = new WorldManager();
        
        public ContentManager ContentManager;
        public GraphicsDevice GraphicsDevice;

        public World Previous;
        public World Current;
        public World Next;
        
        public string PreviousKey;
        public string CurrentKey;
        public string NextKey;
        
        
        static WorldManager()
        {
            
        }
        
        private WorldManager()
        {
            
        }

        public void Fill(Game game)
        {
            Add("ship", new Stage0(game));
            Add("forest1", new Stage1(game));
            Add("forest2", new Stage2(game));
            Add("forest3", new Stage3(game));
        }

        public bool Has(string name)
        {
            return Resources.ContainsKey(name);
        }

        public World Find(string name)
        {
            if (Has(name))
            {
                return Resources[name];
            }
            throw new NullReferenceException();
        }

        public void Add(string key, World value, bool overwrite = false)
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

        public void Grab()
        {
            if (Next == null) return;
            Previous?.UnloadContent();
            if (Current != null)
            {
                Previous = Current;
                PreviousKey = CurrentKey;
            }

            CurrentKey = NextKey;
            Current = Next;
            Next = null;
            NextKey = string.Empty;
        }
        
        public void Stage(string key)
        {
            if (!Has(key)) return;
            if (NextKey == key) return;
            if (CurrentKey == key) return;
            var value = Find(key);
            if (PreviousKey != key)
            {
                Next?.UnloadContent();
                NextKey = key;
                Next = value;
                Next.LoadContent(ContentManager);
            }
            else
            {
                NextKey = PreviousKey;
                Next = Previous;
                Previous = null;
                PreviousKey = string.Empty;
            }
        }
        
        public void UnStage(string key)
        {
            if (!Has(key)) return;
            var value = Find(key);
            Previous = value;
            Previous.UnloadContent();
        }
        
        public void Load(string key)
        {
            if (!Has(key)) return;
            var value = Find(key);
            value.LoadContent(ContentManager);
        }
        
        public void UnLoad(string key)
        {
            if (!Has(key)) return;
            var value = Find(key);
            value.UnloadContent();
        }
    }
}