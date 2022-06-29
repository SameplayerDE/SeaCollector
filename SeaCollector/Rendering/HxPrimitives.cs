using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public class HxPrimitives
    {
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer
        public bool IsIndex = false;
        public HxMaterial Material;
        public Dictionary<string, float[]> Dictionary = new Dictionary<string, float[]>();

        public void Init(GraphicsDevice graphicsDevice)
        {
            var positions = new List<Vector3>();
            var uvs = new List<Vector2>();
            var normals = new List<Vector3>();
            
            foreach (var (key, value) in Dictionary)
            {
                switch (key)
                {
                    case "POSITION":
                    {
                        for (var x = 0; x < value.Length; x += 3)
                        {
                            positions.Add(new Vector3(value[x], value[x + 1], value[x + 2]));
                            //Console.WriteLine(positions[x / 3]);
                        }

                        break;
                    }
                    case "NORMAL":
                    {
                        for (var x = 0; x < value.Length; x += 3)
                        {
                            normals.Add(new Vector3(value[x], value[x + 1], value[x + 2]));
                        }

                        break;
                    }
                    case "TEXCOORD_0":
                    {
                        for (var x = 0; x < value.Length; x += 2)
                        {
                            uvs.Add(new Vector2(value[x], value[x + 1]));
                        }

                        break;
                    }
                }
            }
            var count = positions.Count;
            var vertexData = new VertexPositionNormalTangentBinormalTexture[count];
            
            for (var x = 0; x < count; x++)
            {
                vertexData[x] = new VertexPositionNormalTangentBinormalTexture(positions[x], normals.Count >= count ? normals[x] : Vector3.Up, uvs.Count >= count ? uvs[x] : Vector2.One);
            }

            if (normals.Count < count)
            {
                VertexPositionNormalTangentBinormalTexture.CalculateFaceNormals(vertexData);
            }
            VertexPositionNormalTangentBinormalTexture.CalculateTangent(vertexData);
            VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTangentBinormalTexture.VertexDeclaration, count, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertexData);
        }
    }
}