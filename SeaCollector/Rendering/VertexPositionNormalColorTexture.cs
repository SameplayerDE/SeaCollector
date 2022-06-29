using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public struct VertexPositionNormalColorTexture : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;

        public VertexPositionNormalColorTexture(Vector3 position)
        {
            Position = position;
            Color = Color.White;
            Normal = Vector3.Zero;
            TextureCoordinate = Vector2.Zero;
        }
        
        public VertexPositionNormalColorTexture(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
            Normal = Vector3.Zero;
            TextureCoordinate = Vector2.Zero;
        }
        
        public VertexPositionNormalColorTexture(Vector3 position, Color color, Vector3 normal)
        {
            Position = position;
            Color = color;
            Normal = normal;
            TextureCoordinate = Vector2.Zero;
        }
        
        public VertexPositionNormalColorTexture(Vector3 position, Color color, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Color = color;
            Normal = normal;
            TextureCoordinate = uv;
        }
        
        public VertexPositionNormalColorTexture(VertexPositionNormalColorTexture data, Vector3 normal)
        {
            Position = data.Position;
            Color = data.Color;
            Normal = normal;
            TextureCoordinate = data.TextureCoordinate;
        }
        
        public static void CalculateNormals(VertexPositionNormalColorTexture[] data)
        {

            for (int i = 0; i < data.Length / 3; i++)
            {
                var ab = data[i * 3].Position - data[i * 3 + 1].Position;
                var cb = data[i * 3 + 2].Position - data[i * 3 + 1].Position;

                ab.Normalize();
                cb.Normalize();

                var normal = Vector3.Cross(ab, cb);

                data[i * 3] = new VertexPositionNormalColorTexture(data[i * 3], normal);
                data[i * 3 + 1] = new VertexPositionNormalColorTexture(data[i * 3 + 1], normal);
                data[i * 3 + 2] = new VertexPositionNormalColorTexture(data[i * 3 + 2], normal);
            }
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6 + 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}