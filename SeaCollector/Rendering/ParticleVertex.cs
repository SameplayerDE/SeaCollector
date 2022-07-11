using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public struct ParticleVertex : IVertexType
    {
        
        public Vector3 Position;
        public Color Color;
        public Vector3 Direction;
        public Vector2 TextureCoordinate;
        public float Speed;
        public float StartTime;
        
        public ParticleVertex(Vector3 position, Vector2 textureCoordinate, 
            Vector3 direction, float speed, float startTime)
        {
            Position = position;
            Color = Color.White;
            Direction = direction;
            TextureCoordinate = textureCoordinate;
            Speed = speed;
            StartTime = startTime;
        }
        
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 5 + 4, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * 8 + 4, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(sizeof(float) * 8 + 4 + 1, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3)
        );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}