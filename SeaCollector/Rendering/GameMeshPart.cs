using Microsoft.Xna.Framework.Graphics;
using SeaCollector.HxObj;

namespace SeaCollector.Rendering
{
    public class GameMeshPart
    {
        public int PrimitiveCount;
        public int StartIndex;
        public int VertexOffset;
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer
        public MaterialData Material;
    }
}