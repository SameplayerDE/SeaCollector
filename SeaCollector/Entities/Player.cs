using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector.Entities
{
    public class Player : GameObject
    {

        public Texture2D Texture2D;
        public Effect Effect;
        public GameMesh GameMesh;
        
        public float SailFactor = 0f;
        public float SailDirection = 0f;


        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            Effect = contentManager.Load<Effect>("Effects/TextureCellShader");
            Texture2D = contentManager.Load<Texture2D>("Models/Hulls/sp_hul01");
            Mesh = GameMesh.LoadFromFile(graphicsDevice, "Content/Models/Hulls/hull_0.obj");
        }

        public void ChangeSailFactor(float value)
        {
            SailFactor += value;
            SailFactor = Math.Clamp(SailFactor, 0f, 1f);
        }
        
        public void ChangeSailDirection(float value)
        {
            SailDirection += value;
            SailDirection = Math.Clamp(SailDirection, -1f, 1f);
        }
    }
}