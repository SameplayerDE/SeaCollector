using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Framework;
using SeaCollector.Rendering;
using RenderContext = SeaCollector.Framework.RenderContext;

namespace SeaCollector.Entities
{
    public class GameMeshObject : GameObject3D
    {
        public string TextureFile;
        public string EffectFile;
        public string GameMeshFile;
        
        public Texture2D Texture2D;
        public Effect Effect;
        public GameMesh GameMesh;
        
        public GameMeshObject(string textureFile, string effectFile, string gameMeshFile)
        {
            TextureFile = textureFile;
            EffectFile = effectFile;
            GameMeshFile = gameMeshFile;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            base.LoadContent(graphicsDevice, contentManager);
            
            GameMesh = GameMesh.LoadFromFile(graphicsDevice, GameMeshFile);
            Effect = contentManager.Load<Effect>(EffectFile);
            Texture2D = contentManager.Load<Texture2D>(TextureFile);
        }

        public override void Draw(RenderContext renderContext)
        {
            base.Draw(renderContext);
            Effect.Parameters["Texture00"]?.SetValue(Texture2D);
            GameMesh.Draw(renderContext.GraphicsDevice, Effect, WorldMatrix, renderContext.Camera.View, renderContext.Camera.Projection);
        }
    }
}