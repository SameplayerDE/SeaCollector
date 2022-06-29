using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.HxPly;

namespace SeaCollector.Rendering
{
    public class GameMesh
    {
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer
        public VertexPositionNormalColorTexture[] Data;

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
            if (!extension.Equals(".ply"))
            {
                throw new FileLoadException("file could not be loaded, wrong file type");
            }

            if (extension == ".ply")
            {
                var file = PlyLoader.Load(path);

                var list = new List<VertexPositionNormalColorTexture>();

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
                //VertexPositionNormalColorTexture.CalculateNormals(listArray);
                result.Data = listArray;
            }

            return result;
        }

        ~GameMesh()
        {
            Console.WriteLine("Destructor was called");
        }
    }
}