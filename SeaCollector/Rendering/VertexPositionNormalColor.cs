using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public VertexPositionNormalColor(Vector3 Position)
        {
            this.Position = Position;
            this.Color = Color.White;
            this.Normal = Vector3.Zero;
        }
        
        public VertexPositionNormalColor(Vector3 Position, Color Color)
        {
            this.Position = Position;
            this.Color = Color;
            this.Normal = Vector3.Zero;
        }
        public VertexPositionNormalColor(Vector3 Position, Color Color, Vector3 Normal)
        {
            this.Position = Position;
            this.Color = Color;
            this.Normal = Normal;
        }

        public VertexPositionNormalColor(VertexPositionNormalColor vertex, Vector3 Normal)
        {
            this.Position = vertex.Position;
            this.Color = vertex.Color;
            this.Normal = Normal;
        }

        public VertexPositionNormalColor(VertexPositionNormalColor vertex)
        {
            this.Position = vertex.Position;
            this.Color = vertex.Color;
            this.Normal = vertex.Normal;
        }

        private static void CalculateNormals(VertexPositionNormalColor A, VertexPositionNormalColor B, VertexPositionNormalColor C)
        {
            Vector3 ab = A.Position - B.Position;
            Vector3 cb = C.Position - B.Position;

            ab.Normalize();
            cb.Normalize();

            Vector3 normal = Vector3.Cross(ab, cb);

            A = new VertexPositionNormalColor(A, normal);
            B = new VertexPositionNormalColor(B, normal);
            C = new VertexPositionNormalColor(C, normal);

        }

        public static void CalculateNormals(VertexPositionNormalColor[] data)
        {

            for (int i = 0; i < data.Length / 3; i++)
            {

                data[i * 3] = new VertexPositionNormalColor(data[i * 3]);
                data[i * 3 + 1] = new VertexPositionNormalColor(data[i * 3 + 1]);
                data[i * 3 + 2] = new VertexPositionNormalColor(data[i * 3 + 2]);

                Vector3 ab = data[i * 3].Position - data[i * 3 + 1].Position;
                Vector3 cb = data[i * 3 + 2].Position - data[i * 3 + 1].Position;

                ab.Normalize();
                cb.Normalize();

                Vector3 normal = Vector3.Cross(ab, cb);

                data[i * 3] = new VertexPositionNormalColor(data[i * 3], normal);
                data[i * 3 + 1] = new VertexPositionNormalColor(data[i * 3 + 1], normal);
                data[i * 3 + 2] = new VertexPositionNormalColor(data[i * 3 + 2], normal);

            }

        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 3 + 4 + sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

    }
}