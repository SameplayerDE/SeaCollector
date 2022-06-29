using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public struct VertexPositionNormalTangentBinormalTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Binormal;
        public Vector3 Tangent;
        public Vector2 TextureCoordinate;
        
        public VertexPositionNormalTangentBinormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Tangent = normal;
            Binormal = Vector3.Zero;
            //Tangent = Vector3.Cross(normal, Vector3.Transform(normal, Matrix.CreateRotationX(MathHelper.ToRadians(90))));
            //Tangent = Vector3.Cross(normal, Vector3.Transform(normal, Matrix.CreateRotationY(MathHelper.ToRadians(90))));
            //Binormal = Vector3.Cross(normal, Tangent);
        }
        
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
            new VertexElement(sizeof(float) * 9, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
            new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        public static void CalculateTangent(VertexPositionNormalTangentBinormalTexture[] data)
        {
            for (var i = 0 ; i < data.Length; i += 3) {
                
                var v0 = data[i + 0];
                var v1 = data[i + 1];
                var v2 = data[i + 2];
                
                var ab = v1.Position - v0.Position;
                var cb = v2.Position - v0.Position;

                var deltaU1 = v1.TextureCoordinate.X - v0.TextureCoordinate.X;
                var deltaV1 = v1.TextureCoordinate.Y - v0.TextureCoordinate.Y;
                var deltaU2 = v2.TextureCoordinate.X - v0.TextureCoordinate.X;
                var deltaV2 = v2.TextureCoordinate.Y - v0.TextureCoordinate.Y;

                var f = 1.0f / (deltaU1 * deltaV2 - deltaU2 * deltaV1);

                Vector3 tangent;
                Vector3 binormal;

                tangent.X = f * (deltaV2 * ab.X - deltaV1 * cb.X);
                tangent.Y = f * (deltaV2 * ab.Y - deltaV1 * cb.Y);
                tangent.Z = f * (deltaV2 * ab.Z - deltaV1 * cb.Z);

                binormal.X = f * (-deltaU2 * ab.X + deltaU1 * cb.X);
                binormal.Y = f * (-deltaU2 * ab.Y + deltaU1 * cb.Y);
                binormal.Z = f * (-deltaU2 * ab.Z + deltaU1 * cb.Z);

                data[i + 0].Tangent = Vector3.Normalize(tangent);
                data[i + 1].Tangent = Vector3.Normalize(tangent);
                data[i + 2].Tangent = Vector3.Normalize(tangent);
                
                data[i + 0].Binormal = Vector3.Normalize(binormal);
                data[i + 1].Binormal = Vector3.Normalize(binormal);
                data[i + 2].Binormal = Vector3.Normalize(binormal);
            }
        }
        
        public static void CalculateFaceNormals(VertexPositionNormalTangentBinormalTexture[] data)
        {

            for (var i = 0 ; i < data.Length; i += 3) {
                
                var v0 = data[i + 0];
                var v1 = data[i + 1];
                var v2 = data[i + 2];
                
                var ab = v1.Position - v0.Position;
                var cb = v2.Position - v0.Position;

                ab.Normalize();
                cb.Normalize();

                var normal = Vector3.Cross(ab, cb);

                data[i + 0].Normal = normal;
                data[i + 1].Normal = normal;
                data[i + 2].Normal = normal;
            }

        }
        
    }
}