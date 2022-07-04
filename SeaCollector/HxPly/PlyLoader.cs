using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SeaCollector.Rendering;

namespace SeaCollector.HxPly
{
    public static class PlyLoader
    {
        public static PlyFile Load(string path)
        {
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

            return LoadFromPlyFile(path);
        }

        private static PlyFile LoadFromPlyFile(string path)
        {
            var plyFile = new PlyFile();

            Element currentElement = null;
            var vertexCount = 0;
            var faceCount = 0;

            var headerRead = false;
            var dataRead = false;

            var hasColor = false;
            var hasNormals = false;
            var hasAlpha = false;
            var hasST = false;

            var shapes = 3;
            var faces3 = 0;
            var faces4 = 0;

            var vertexData = new List<VertexPositionNormalColorTexture>();
            var indexData = new List<int[]>();

            var lines = File.ReadAllLines(path);

            var dataCounter = 0;
            var elementIndex = 0;

            foreach (var line in lines)
            {
                var split = line.Split(" ");

                if (!headerRead)
                {
                    var a = split[0];

                    switch (a)
                    {
                        case "end_header":
                            headerRead = true;
                            currentElement = null;
                            continue;
                        case "format":
                        {
                            var type = split[1];
                            var version = float.Parse(split[2].Replace(".", ","));

                            plyFile.Header.Format.Type = type switch
                            {
                                "ascii" => FormatType.Ascii,
                                "binary_little_endian" => FormatType.BinaryLittleEndian,
                                "binary_big_endian" => FormatType.BinaryBigEndian,
                                _ => plyFile.Header.Format.Type
                            };

                            plyFile.Header.Format.Version = version;
                            break;
                        }
                        case "element":
                        {
                            var type = split[1];
                            var count = int.Parse(split[2]);

                            switch (type)
                            {
                                case "vertex":
                                    vertexCount = count;
                                    currentElement = new Element(ElementType.Vertex, vertexCount);
                                    break;
                                case "face":
                                    faceCount = count;
                                    currentElement = new Element(ElementType.Face, faceCount);
                                    break;
                            }

                            if (currentElement != null)
                            {
                                plyFile.Header.ElementDictionary[currentElement.Type] = currentElement;
                                plyFile.Header.ElementList.Add(currentElement);
                            }

                            break;
                        }
                        case "property":
                        {
                            var property = new Property(split[1..]);
                            currentElement?.AddProperty(property);
                            break;
                        }
                    }
                }

                if (headerRead && !dataRead)
                {
                    //Console.WriteLine($"{line}");
                    var value = plyFile.Header.ElementList[elementIndex];
                    if (!value.ReadAllData)
                    {
                        if (value.Type == ElementType.Vertex)
                        {
                            var valueList = new Dictionary<string, string>();
                            dataCounter++;
                            for (var index = 0; index < value.Properties.Count; index++)
                            {
                                var property = value.Properties[index];
                                var propertyValue = split[index];
                                valueList[property.Name] = propertyValue;
                                //Console.WriteLine($"{property.Name} : {propertyValue}");
                            }

                            vertexData.Add
                            (
                                new VertexPositionNormalColorTexture
                                (
                                    new Vector3
                                    (
                                        float.Parse(valueList["x"].Replace(".", ",")),
                                        float.Parse(valueList["y"].Replace(".", ",")),
                                        float.Parse(valueList["z"].Replace(".", ","))
                                    ),
                                    new Color
                                    (
                                        byte.Parse(valueList.ContainsKey("red") ? valueList["red"] : "0"),
                                        byte.Parse(valueList.ContainsKey("green") ? valueList["green"] : "0"),
                                        byte.Parse(valueList.ContainsKey("blue") ? valueList["blue"] : "0")
                                    ),
                                    new Vector3
                                    (
                                        float.Parse(valueList.ContainsKey("nx") ? valueList["nx"].Replace(".", ",") : "0"),
                                        float.Parse(valueList.ContainsKey("ny") ? valueList["ny"].Replace(".", ",") : "0"),
                                        float.Parse(valueList.ContainsKey("nz") ? valueList["nz"].Replace(".", ",") : "0")
                                    ),
                                    new Vector2
                                    (
                                        float.Parse(valueList.ContainsKey("s") ? valueList["s"].Replace(".", ",") : "0"),
                                        float.Parse(valueList.ContainsKey("t") ? valueList["t"].Replace(".", ",") : "0")
                                    )
                                )
                            );

                            if (dataCounter == value.Count)
                            {
                                value.ReadAllData = true;
                                dataCounter = 0;
                                elementIndex++;
#if DEBUG
                                Console.WriteLine("vertex data has been read!");
#endif
                            }
                        }
                        else if (value.Type == ElementType.Face)
                        {
                            dataCounter++;
                            for (var index = 0; index < value.Properties.Count; index++)
                            {
                                var property = value.Properties[index];
                                var propertyValue = split[index];

                                //Console.WriteLine($"{property.Name} : {propertyValue}");

                                if (property.IsList)
                                {
                                    var listSize = int.Parse(propertyValue);
                                    var listData = new int[listSize];
                                    for (var listIndex = 1; listIndex <= listSize; listIndex++)
                                    {
                                        var listValue = split[listIndex];
                                        listData[listIndex - 1] = int.Parse(listValue);
                                    }

                                    indexData.Add(listData);
                                }
                            }

                            if (dataCounter == value.Count)
                            {
                                value.ReadAllData = true;
                                dataCounter = 0;
                                elementIndex++;
#if DEBUG
                                Console.WriteLine("face data has been read!");
#endif
                            }
                        }
                    }
                }
            }

            plyFile.IndexData = indexData;
            plyFile.VertexData = vertexData;

            return plyFile;
        }

        /*public GameMesh Load(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            foreach (var line in lines)
            {
                if (line.StartsWith("element"))
                {
                    if (line.Contains("vertex"))
                    {
                        verticies = int.Parse(line.AsSpan(14));
                    }
                    if (line.Contains("face"))
                    {
                        faces = int.Parse(line.AsSpan(12));
                    }
                }
                if (line.StartsWith("property"))
                {
                    string property = line.Substring(9);
                    properties.Add(property);
                    if (property.Contains("nx"))
                    {
                        hasNormals = true;
                    }
                    if (property.Contains("red"))
                    {
                        hasColor = true;
                    }
                    if (property.Contains("alpha"))
                    {
                        hasAlpha = true;
                    }
                    if (property.Contains("float s"))
                    {
                        hasST = true;
                    }
                }
                if (line == "end_header")
                {
                    data = true;
                    continue;
                }
                if (data)
                {
                    if (vector3s.Count != verticies)
                    {
                        string[] values = line.Replace(".", ",").Split(" ");
                        float x, y, z;
                        Vector3 normal = Vector3.One;
                        Vector2 texture = Vector2.One;

                        Color color = Color.White;
                        x = float.Parse(values[0]);
                        y = float.Parse(values[1]);
                        z = float.Parse(values[2]);

                        if (hasNormals)
                        {
                            float nx, ny, nz;
                            nx = float.Parse(values[3]);
                            ny = float.Parse(values[4]);
                            nz = float.Parse(values[5]);
                            normal = new Vector3(nx, ny, nz);
                        }

                        if (hasST)
                        {
                            float s, t;
                            if (hasNormals)
                            {

                                s = float.Parse(values[6]);
                                t = float.Parse(values[7]);
                            }
                            else
                            {
                                s = float.Parse(values[3]);
                                t = float.Parse(values[4]);

                            }
                            texture = new Vector2(s, -t);
                        }

                        if (hasColor)
                        {
                            int r, g, b, a = 255;
                            if (hasNormals)
                            {
                                if (hasST)
                                {
                                    if (hasAlpha)
                                    {

                                        r = int.Parse(values[8]);
                                        g = int.Parse(values[9]);
                                        b = int.Parse(values[10]);
                                        a = int.Parse(values[11]);

                                    }
                                    else
                                    {
                                        r = int.Parse(values[8]);
                                        g = int.Parse(values[9]);
                                        b = int.Parse(values[10]);
                                    }
                                }
                                else
                                {

                                    if (hasAlpha)
                                    {

                                        r = int.Parse(values[6]);
                                        g = int.Parse(values[7]);
                                        b = int.Parse(values[8]);
                                        a = int.Parse(values[9]);

                                    }
                                    else
                                    {
                                        r = int.Parse(values[6]);
                                        g = int.Parse(values[7]);
                                        b = int.Parse(values[8]);
                                    }
                                }
                            }
                            else
                            {
                                if (hasAlpha)
                                {
                                    r = int.Parse(values[3]);
                                    g = int.Parse(values[4]);
                                    b = int.Parse(values[5]);
                                    a = int.Parse(values[6]);
                                }
                                else
                                {
                                    r = int.Parse(values[3]);
                                    g = int.Parse(values[4]);
                                    b = int.Parse(values[5]);
                                }

                            }
                            color = new Color(r, g, b, a);
                        }
                        Vector3 v = new Vector3(x, y, z);
                        VertexPositionNormalColor vertex = new VertexPositionNormalColor(v, color);
                        vertex = new VertexPositionNormalColor(vertex, normal);
                        vector3s.Add(vertex);
                        //Console.WriteLine(vertex);
                    }
                    else
                    {
                        string[] coordinates = line.Split(" ");

                        if (int.Parse(coordinates[0]) == 4)
                        {
                            faces4++;
                            int a, b, c, d;
                            a = int.Parse(coordinates[1]);
                            b = int.Parse(coordinates[2]);
                            c = int.Parse(coordinates[3]);
                            d = int.Parse(coordinates[4]);
                            var p = new int[] {a,b,c};
                            var p2 = new int[] {c, d, a};
                            //Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + ",d:" + d + "}");
                            point3s.Add(p);
                            point3s.Add(p2);
                        }
                        else if (int.Parse(coordinates[0]) == 3)
                        {
                            faces3++;
                            int a, b, c;
                            a = int.Parse(coordinates[1]);
                            b = int.Parse(coordinates[2]);
                            c = int.Parse(coordinates[3]);
                            var p = new int[] {a, b, c};
                            // Console.WriteLine("{a:" + a + ",b:" + b + ",c:" + c + "}");
                            point3s.Add(p);
                        }
                    }

                }
            }

            Console.WriteLine("Verticis(file): " + verticies);
            Console.WriteLine("Faces(file): " + faces);
            Console.WriteLine("Faces3: " + faces3);
            Console.WriteLine("Faces4: " + faces4);
            Console.WriteLine("Faces4*2+Faces3: " + (faces4 * 2 + faces3));
            Console.WriteLine("Punkte: " + point3s.Count);
            Console.WriteLine(shapes);

            Console.WriteLine("Press any key to exit.");
            //System.Console.ReadKey();

            if (shapes == 1)
            {
                vertexPositions = new VertexPositionNormalColor[faces * 2 * 3];
                for (int i = 0; i < faces * 2; i++)
                {
                    vertexPositions[i * 3] = vector3s[point3s[i][0]];
                    vertexPositions[i * 3 + 1] = vector3s[point3s[i][1]];
                    vertexPositions[i * 3 + 2] = vector3s[point3s[i][2]];
                }
            }
            else if (shapes == 2)
            {
                vertexPositions = new VertexPositionNormalColor[faces * 3];
                for (int i = 0; i < faces; i++)
                {
                    vertexPositions[i * 3] = vector3s[point3s[i][0]];
                    vertexPositions[i * 3 + 1] = vector3s[point3s[i][1]];
                    vertexPositions[i * 3 + 2] = vector3s[point3s[i][2]];
                }
            }
            else if (shapes == 3)
            {
                vertexPositions = new VertexPositionNormalColor[(faces4 * 2 * 3) + (faces3 * 3)];
                for (int i = 0; i < (faces4 * 2) + faces3; i++)
                {
                    vertexPositions[i * 3] = vector3s[point3s[i][0]];
                    vertexPositions[i * 3 + 1] = vector3s[point3s[i][1]];
                    vertexPositions[i * 3 + 2] = vector3s[point3s[i][2]];
                }
            }
            if (!hasNormals)
            {
                VertexPositionNormalColor.CalculateNormals(vertexPositions);
            }
            Console.WriteLine("Press any key to exit.");
            //System.Console.ReadKey();
        }
    }
    
    public Mesh ToMesh(GraphicsDevice graphicsDevice)
    {
        int faceCount = _faces.Count;
        int vertexCount = _verticies.Count;
        int uvCount = _uvs.Count;
        Material material = null;
        CMaterial cmaterial = null;

        VertexPositionNormalTexture[] vertexPositions = new VertexPositionNormalTexture[faceCount * 3];

        for (int i = 0; i < faceCount; i++)
        {

            //Console.WriteLine(_faces[i].ToString);

            if (_faces[i].Material != null && material == null)
            {
                material = _faces[i].Material;
            }

            if (_faces[i].CMaterial != null && cmaterial == null)
            {
                cmaterial = _faces[i].CMaterial;
            }

            Vector3[] verticies = { 
                _verticies[_faces[i].X - 1], 
                _verticies[_faces[i].Y - 1], 
                _verticies[_faces[i].Z - 1] };

            Vector3[] normals = { 
                _normals[_faces[i].NX - 1], 
                _normals[_faces[i].NY - 1], 
                _normals[_faces[i].NZ - 1] };

            Vector2[] uvs = { 
                _uvs[_faces[i].UVX - 1], 
                _uvs[_faces[i].UVY - 1], 
                _uvs[_faces[i].UVZ - 1] };


            vertexPositions[i * 3 + 0] = new VertexPositionNormalTexture(verticies[0], normals[0], uvs[0]);
            vertexPositions[i * 3 + 1] = new VertexPositionNormalTexture(verticies[1], normals[1], uvs[1]);
            vertexPositions[i * 3 + 2] = new VertexPositionNormalTexture(verticies[2], normals[2], uvs[2]);

        }

        return new GameMesh(vertexPositions, faceCount, vertexCount, graphicsDevice);
    }*/
    }
}