using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector.Entities
{
    public class GameMeshObject : GameObject3D
    {
        public string AssetFile;
        public Texture2D Texture2D;
        public Effect Effect;
        public GameMesh GameMesh;
        
        public GameMeshObject(string assetFile)
        {
            AssetFile = assetFile;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            GameMesh = GameMesh.LoadFromFile(graphicsDevice, AssetFile);

            base.LoadContent(graphicsDevice, contentManager);
        }

        public override void Draw(GraphicsDevice graphicsDevice, Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            GameMesh.Draw(graphicsDevice, effect, WorldMatrix, view, projection);
            base.Draw(graphicsDevice, effect, world, view, projection);
        }
    }
}