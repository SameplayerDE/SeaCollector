using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.HxObj;
using SeaCollector.HxPly;

namespace SeaCollector.Rendering
{
    public class GameMesh
    {
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer
        
        public VertexPositionNormalColorTexture[] Data;
        public GraphicsDevice GraphicsDevice;

        public List<GameMeshPart> MeshParts;

        public GameMesh()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            MeshParts = new List<GameMeshPart>();
        }

        public static GameMesh LoadFromFile(GraphicsDevice graphicsDevice, string path)
        {
            var result = new GameMesh();

            if (Directory.Exists(path))
            {
                throw new Exception("passed directory path");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("file could not be found");
            }

            var extension = Path.GetExtension(path);
            if (!extension.Equals(".ply") && !extension.Equals(".obj"))
            {
                throw new FileLoadException("file could not be loaded, wrong file type");
            }

            if (extension == ".ply")
            {
                var file = PlyLoader.Load(path);

                var list = new List<VertexPositionNormalColorTexture>();
                var meshPart = new GameMeshPart();

                foreach (var indexSet in file.IndexData)
                {
                    if (indexSet.Length == 3)
                    {
                        int a, b, c, d;
                        a = indexSet[0];
                        b = indexSet[1];
                        c = indexSet[2];
                        var p = new[] { a, b, c };
                        //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");
                        foreach (var indexPair in p)
                        {
                            list.Add(file.VertexData[indexPair]);
                        }
                    }

                    if (indexSet.Length == 4)
                    {
                        int a, b, c, d;
                        a = indexSet[0];
                        b = indexSet[1];
                        c = indexSet[2];
                        d = indexSet[3];
                        var p = new[] { a, b, c };
                        var p2 = new[] { c, d, a };
                        //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");
                        foreach (var indexPair in p)
                        {
                            list.Add(file.VertexData[indexPair]);
                        }

                        foreach (var indexPair in p2)
                        {
                            list.Add(file.VertexData[indexPair]);
                        }
                    }

                    //list.Add(p2);

                    //Console.WriteLine("[{0}]", string.Join(", ", p));
                    //Console.WriteLine("[{0}]", string.Join(", ", p2));
                }

                /*var splitList = new List<short>();
    
                foreach (var indexPair in list)
                {
                    //splitList.AddRange(indexPair);
                }*/

                /*result.VertexBuffer = new VertexBuffer(
                    graphicsDevice,
                    VertexPositionNormalColorTexture.VertexDeclaration,
                    list.Count,
                    BufferUsage.WriteOnly
                );
                
                result.IndexBuffer = new IndexBuffer(
                    graphicsDevice,
                    typeof(short),
                    list.Count,
                    BufferUsage.WriteOnly
                );*/

                var listArray = list.ToArray();
                meshPart.VertexBuffer = new VertexBuffer(graphicsDevice,
                    VertexPositionNormalColorTexture.VertexDeclaration,
                    result.Data.Length, BufferUsage.WriteOnly);

                meshPart.IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits,
                    result.Data.Length,
                    BufferUsage.WriteOnly);

                var meshIndices = new int [result.Data.Length];
                for (var i = 0; i < meshIndices.Length; i++)
                {
                    meshIndices[i] = i;
                }

                meshPart.VertexBuffer.SetData(result.Data);
                meshPart.IndexBuffer.SetData(meshIndices);
                result.MeshParts.Add(meshPart);
                //VertexPositionNormalColorTexture.CalculateNormals(listArray);
                result.Data = listArray;
            }

            else if (extension == ".obj")
            {
                var file = ObjLoader.Load(path);

                var list = new List<VertexPositionNormalColorTexture>();

                foreach (var objPart in file.ObjectParts)
                {
                    var meshPart = new GameMeshPart();
                    foreach (var face in objPart.Faces)
                    {
                        var type = face.Type;

                        if (type == FaceType.Vertex)
                        {
                            if (face.DataGroupCount == 3)
                            {
                                int a, b, c;
                                a = face.Data[0][0];
                                b = face.Data[1][0];
                                c = face.Data[2][0];
                                var p = new[] { a, b, c };
                                //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");
                                foreach (var indexPair in p)
                                {
                                    var values = objPart.Vertices[indexPair - 1];
                                    var x = values[0];
                                    var y = values[1];
                                    var z = values[2];
                                    list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z)));
                                }
                            }
                        }

                        if (type == FaceType.VertexNormal)
                        {
                            if (face.DataGroupCount == 3)
                            {
                                int a, b, c;
                                a = face.Data[0][0];
                                b = face.Data[1][0];
                                c = face.Data[2][0];
                                var position = new[] { a, b, c };

                                int an, bn, cn;
                                an = face.Data[0][1];
                                bn = face.Data[1][1];
                                cn = face.Data[2][1];
                                var normal = new[] { an, bn, cn };

                                //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");

                                for (var i = 0; i < face.DataGroupCount; i++)
                                {
                                    var vertexIndex = position[i];
                                    var normalIndex = normal[i];

                                    var vertexValues = objPart.Vertices[vertexIndex - 1];
                                    var normalValues = objPart.Normals[normalIndex - 1];

                                    var x = vertexValues[0];
                                    var y = vertexValues[1];
                                    var z = vertexValues[2];

                                    var nx = normalValues[0];
                                    var ny = normalValues[1];
                                    var nz = normalValues[2];

                                    list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z), Color.White,
                                        new Vector3(nx, ny, nz)));
                                }
                            }
                        }

                        if (type == FaceType.VertexTexture)
                        {
                            if (face.DataGroupCount == 3)
                            {
                                int a, b, c;
                                a = face.Data[0][0];
                                b = face.Data[1][0];
                                c = face.Data[2][0];
                                var position = new[] { a, b, c };

                                int auv, buv, cuv;
                                auv = face.Data[0][1];
                                buv = face.Data[1][1];
                                cuv = face.Data[2][1];
                                var uv = new[] { auv, buv, cuv };

                                //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");

                                for (var i = 0; i < face.DataGroupCount; i++)
                                {
                                    var vertexIndex = position[i];
                                    var uvIndex = uv[i];

                                    var vertexValues = objPart.Vertices[vertexIndex - 1];
                                    var uvValues = objPart.UVs[uvIndex - 1];

                                    var x = vertexValues[0];
                                    var y = vertexValues[1];
                                    var z = vertexValues[2];

                                    var u = uvValues[0];
                                    var v = uvValues[1];

                                    list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z), Color.White,
                                        new Vector3(0, 0, 0), new Vector2(u, 1 - v)));
                                }
                            }
                        }

                        if (type == FaceType.VertexTextureNormal)
                        {
                            if (face.DataGroupCount == 3)
                            {
                                int a, b, c;
                                a = face.Data[0][0];
                                b = face.Data[1][0];
                                c = face.Data[2][0];
                                var position = new[] { a, b, c };

                                int an, bn, cn;
                                an = face.Data[0][2];
                                bn = face.Data[1][2];
                                cn = face.Data[2][2];
                                var normal = new[] { an, bn, cn };

                                int auv, buv, cuv;
                                auv = face.Data[0][1];
                                buv = face.Data[1][1];
                                cuv = face.Data[2][1];
                                var uv = new[] { auv, buv, cuv };

                                //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");

                                for (var i = 0; i < face.DataGroupCount; i++)
                                {
                                    var vertexIndex = position[i];
                                    var normalIndex = normal[i];
                                    var uvIndex = uv[i];

                                    var vertexValues = objPart.Vertices[vertexIndex - 1];
                                    var normalValues = objPart.Normals[normalIndex - 1];
                                    var uvValues = objPart.UVs[uvIndex - 1];

                                    var x = vertexValues[0];
                                    var y = vertexValues[1];
                                    var z = vertexValues[2];

                                    var nx = normalValues[0];
                                    var ny = normalValues[1];
                                    var nz = normalValues[2];

                                    var u = uvValues[0];
                                    var v = uvValues[1];

                                    list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z), Color.White,
                                        new Vector3(nx, ny, nz), new Vector2(u, 1 - v)));
                                }
                            }
                        }
                    }

                    meshPart.VertexBuffer = new VertexBuffer(graphicsDevice,
                        VertexPositionNormalColorTexture.VertexDeclaration,
                        list.Count, BufferUsage.WriteOnly);

                    meshPart.IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, list.Count,
                        BufferUsage.WriteOnly);

                    var indices0 = new int [list.Count];
                    for (int i = 0; i < indices0.Length; i++)
                    {
                        indices0[i] = i;
                    }

                    meshPart.VertexBuffer.SetData(list.ToArray());
                    meshPart.IndexBuffer.SetData(indices0);
                    result.MeshParts.Add(meshPart);
                }

                foreach (var face in file.Faces)
                {
                    var type = face.Type;

                    if (type == FaceType.Vertex)
                    {
                        if (face.DataGroupCount == 3)
                        {
                            int a, b, c;
                            a = face.Data[0][0];
                            b = face.Data[1][0];
                            c = face.Data[2][0];
                            var p = new[] { a, b, c };
                            //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");
                            foreach (var indexPair in p)
                            {
                                var values = file.Vertices[indexPair - 1];
                                var x = values[0];
                                var y = values[1];
                                var z = values[2];
                                list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z)));
                            }
                        }
                    }

                    if (type == FaceType.VertexNormal)
                    {
                        if (face.DataGroupCount == 3)
                        {
                            int a, b, c;
                            a = face.Data[0][0];
                            b = face.Data[1][0];
                            c = face.Data[2][0];
                            var position = new[] { a, b, c };

                            int an, bn, cn;
                            an = face.Data[0][1];
                            bn = face.Data[1][1];
                            cn = face.Data[2][1];
                            var normal = new[] { an, bn, cn };

                            //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");

                            for (var i = 0; i < face.DataGroupCount; i++)
                            {
                                var vertexIndex = position[i];
                                var normalIndex = normal[i];

                                var vertexValues = file.Vertices[vertexIndex - 1];
                                var normalValues = file.Normals[normalIndex - 1];

                                var x = vertexValues[0];
                                var y = vertexValues[1];
                                var z = vertexValues[2];

                                var nx = normalValues[0];
                                var ny = normalValues[1];
                                var nz = normalValues[2];

                                list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z), Color.White,
                                    new Vector3(nx, ny, nz)));
                            }
                        }
                    }

                    if (type == FaceType.VertexTexture)
                    {
                        if (face.DataGroupCount == 3)
                        {
                            int a, b, c;
                            a = face.Data[0][0];
                            b = face.Data[1][0];
                            c = face.Data[2][0];
                            var position = new[] { a, b, c };

                            int auv, buv, cuv;
                            auv = face.Data[0][1];
                            buv = face.Data[1][1];
                            cuv = face.Data[2][1];
                            var uv = new[] { auv, buv, cuv };

                            //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");

                            for (var i = 0; i < face.DataGroupCount; i++)
                            {
                                var vertexIndex = position[i];
                                var uvIndex = uv[i];

                                var vertexValues = file.Vertices[vertexIndex - 1];
                                var uvValues = file.UVs[uvIndex - 1];

                                var x = vertexValues[0];
                                var y = vertexValues[1];
                                var z = vertexValues[2];

                                var u = uvValues[0];
                                var v = uvValues[1];

                                list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z), Color.White,
                                    new Vector3(0, 0, 0), new Vector2(u, 1 - v)));
                            }
                        }
                    }

                    if (type == FaceType.VertexTextureNormal)
                    {
                        if (face.DataGroupCount == 3)
                        {
                            int a, b, c;
                            a = face.Data[0][0];
                            b = face.Data[1][0];
                            c = face.Data[2][0];
                            var position = new[] { a, b, c };

                            int an, bn, cn;
                            an = face.Data[0][2];
                            bn = face.Data[1][2];
                            cn = face.Data[2][2];
                            var normal = new[] { an, bn, cn };

                            int auv, buv, cuv;
                            auv = face.Data[0][1];
                            buv = face.Data[1][1];
                            cuv = face.Data[2][1];
                            var uv = new[] { auv, buv, cuv };

                            //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");

                            for (var i = 0; i < face.DataGroupCount; i++)
                            {
                                var vertexIndex = position[i];
                                var normalIndex = normal[i];
                                var uvIndex = uv[i];

                                var vertexValues = file.Vertices[vertexIndex - 1];
                                var normalValues = file.Normals[normalIndex - 1];
                                var uvValues = file.UVs[uvIndex - 1];

                                var x = vertexValues[0];
                                var y = vertexValues[1];
                                var z = vertexValues[2];

                                var nx = normalValues[0];
                                var ny = normalValues[1];
                                var nz = normalValues[2];

                                var u = uvValues[0];
                                var v = uvValues[1];

                                list.Add(new VertexPositionNormalColorTexture(new Vector3(x, y, z), Color.White,
                                    new Vector3(nx, ny, nz), new Vector2(u, 1 - v)));
                            }
                        }
                    }
                }

                var listArray = list.ToArray();
                //VertexPositionNormalColorTexture.CalculateNormals(listArray);
                result.Data = listArray;
            }

            result.VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalColorTexture.VertexDeclaration,
                result.Data.Length, BufferUsage.WriteOnly);

            result.IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, result.Data.Length,
                BufferUsage.WriteOnly);

            var indices = new int [result.Data.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            result.VertexBuffer.SetData(result.Data);
            result.IndexBuffer.SetData(indices);
            result.GraphicsDevice = graphicsDevice;

            return result;
        }

        ~GameMesh()
        {
            Console.WriteLine("Destructor was called");
        }

        public void Draw(GraphicsDevice graphicsDevice, Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            var l_view = view;
            var l_projection = projection;

            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.Indices = IndexBuffer;
            
            effect.Parameters["WorldViewProjection"]?.SetValue(world * l_view * l_projection);
            effect.Parameters["World"]?.SetValue(world);
            effect.Parameters["View"]?.SetValue(l_view);
            effect.Parameters["Projection"]?.SetValue(l_projection);
            
            foreach (var currentTechniquePass in effect.CurrentTechnique.Passes)
            {
                
                currentTechniquePass.Apply();
                //graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Mesh.Data, 0, Mesh.Data.Length / 3);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Data.Length / 3);
            }
        }
    }
}