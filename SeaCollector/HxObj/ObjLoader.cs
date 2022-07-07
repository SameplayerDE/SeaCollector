using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SharpDX.Direct3D11;

namespace SeaCollector.HxObj
{
    public static class ObjLoader
    {
        public static ObjFile Load(string path)
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
            if (!extension.Equals(".obj"))
            {
                throw new FileLoadException("file could not be loaded, wrong file type");
            }

            return LoadFromObjFile(path);
        }

        private static ObjFile LoadFromObjFile(string path)
        {

            var result = new ObjFile();
            
            var name = string.Empty;
            var materials = new List<MtlFile>();
            var normals = new List<float[]>();
            var uvs = new List<float[]>();
            var vertices = new List<float[]>();
            var faces = new List<Face>();

            var objParts = new List<ObjPart>();
            ObjPart current = null;

            var objOffset = 0;
            var vertexOffset = 0;
            var normalOffset = 0;
            var uvOffset = 0;

            var materialName = string.Empty;

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var split = line.Replace(".", ",").Split(" ");

                var type = split[0];
                var values = split[1..];

                if (type.Equals("o"))
                {
                    //name
                    if (current != null)
                    {
                        current.Vertices = new List<float[]>(vertices);
                        current.UVs = new List<float[]>(uvs);
                        current.Normals = new List<float[]>(normals);
                        current.Faces = new List<Face>(faces);
                        
                        objParts.Add(current);
                        vertexOffset += current.Vertices.Count;
                        normalOffset += current.Normals.Count;
                        uvOffset += current.UVs.Count;
                    }
                    
                    faces.Clear();
                    uvs.Clear();
                    normals.Clear();
                    vertices.Clear();
                    
                    current = new ObjPart();
                    current.Name = values[0];
                }

                if (type.Equals("mtllib"))
                {
                    //material
                    materials.Add(MtlLoader.Load(path.Replace(Path.GetFileName(path), values[0].Replace(",", "."))));
                }
                
                if (type.Equals("usemtl"))
                {
                    materialName = values[0].Replace(",", ".");
                    //material
                    //current.Material = materials.Last().Materials[values[0].Replace(",", ".")];
                    //current.MaterialName = values[0].Replace(",", ".");
                    //materials.Add(MtlLoader.Load(path.Replace(Path.GetFileName(path), values[0].Replace(",", "."))));
                }
                
                if (type.Equals("v"))
                {
                    //vertex
                    // ReSharper disable once HeapView.DelegateAllocation
                    vertices.Add(Array.ConvertAll(values, float.Parse));
                }

                if (type.Equals("vt"))
                {
                    //vertex texture
                    // ReSharper disable once HeapView.DelegateAllocation
                    uvs.Add(Array.ConvertAll(values, float.Parse));
                }

                if (type.Equals("vn"))
                {
                    //vertex normal
                    // ReSharper disable once HeapView.DelegateAllocation
                    normals.Add(Array.ConvertAll(values, float.Parse));
                }

                if (type.Equals("f"))
                {
#if DEBUG
                    Console.WriteLine(line);
#endif
                    // ReSharper disable once HeapView.ObjectAllocation.Evident
                    var face = new Face();
                    //face
                    if (Regex.IsMatch(line, @"f{1}(\s\d+\/\d+\/\d+){3,}", RegexOptions.None)) // x\x\x
                    {
                        //vertex uv normals
                        face.SetType(FaceType.VertexTextureNormal);
                        foreach (var value in values) //DataGroups
                        {
                            // ReSharper disable once HeapView.DelegateAllocation
                            var indices = Array.ConvertAll(value.Split("/"), int.Parse);
                            indices[0] -= vertexOffset;
                            indices[1] -= uvOffset;
                            indices[2] -= normalOffset;
#if DEBUG
                            Console.WriteLine(string.Join(",", indices));
#endif
                            face.AddData(indices);
                        }
                    }
                    else if (Regex.IsMatch(line, @"f{1}(\s\d+\/\/\d+){3,}", RegexOptions.None)) // x\\x
                    {
                        //vertex and normals
                        face.SetType(FaceType.VertexNormal);
                        foreach (var value in values) //DataGroups
                        {
                            // ReSharper disable once HeapView.DelegateAllocation
                            var indices = Array.ConvertAll(value.Split("//"), int.Parse);
#if DEBUG
                            Console.WriteLine(string.Join(",", indices));
#endif
                            face.AddData(indices);
                        }
                    }
                    else if (Regex.IsMatch(line, @"f{1}(\s\d+\/\d+){3,}", RegexOptions.None)) // x\x
                    {
                        //vertex and uv
                        face.SetType(FaceType.VertexTexture);
                        foreach (var value in values) //DataGroups
                        {
                            // ReSharper disable once HeapView.DelegateAllocation
                            var indices = Array.ConvertAll(value.Split("/"), int.Parse);
#if DEBUG
                            Console.WriteLine(string.Join(",", indices));
#endif
                            face.AddData(indices);
                        }
                    }
                    else if (Regex.IsMatch(line, @"f{1}(\s\d+){3,}", RegexOptions.None)) // x
                    {
                        //vertex only
                        face.SetType(FaceType.Vertex);
                        // ReSharper disable once HeapView.DelegateAllocation
                        var indices = Array.ConvertAll(values, int.Parse);
#if DEBUG
                        Console.WriteLine(string.Join(",", indices));
#endif
                        foreach (var index in indices)
                        {
                            face.AddData(new [] {index});
                        }
                    }
                    face.MaterialName = materialName;
                    faces.Add(face);
                }
            }

            if (current != null)
            {
                current.Vertices = new List<float[]>(vertices);
                current.UVs = new List<float[]>(uvs);
                current.Normals = new List<float[]>(normals);
                current.Faces = new List<Face>(faces);
                        
                objParts.Add(current);
            }
            
            result.Name = name;
            result.Materials = new List<MtlFile>(materials);
            result.Vertices = vertices;
            result.UVs = uvs;
            result.Normals = normals;
            result.Faces = faces;
            result.ObjectParts = new List<ObjPart>(objParts);
            
            return result;
        }
    }
}