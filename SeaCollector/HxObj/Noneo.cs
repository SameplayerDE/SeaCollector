using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Monogame_VS2019_Shared_Game0003
{

    public class FileReader_OBJ
    {

        private List<Vector3> _normals = new List<Vector3>();
        private List<Vector2> _uvs = new List<Vector2>();
        private List<Vector3> _verticies = new List<Vector3>();
        private List<Face> _faces = new List<Face>();

        public FileReader_OBJ(string path)
        {
            string[] lines;
#if ANDROID
            Stream fileStream = TitleContainer.OpenStream(path);
            StreamReader reader = new StreamReader(fileStream);
            string streamLine = "";
            List<string> lineList = new List<string>();

            while ((streamLine = reader.ReadLine()) != null)
            {
                lineList.Add(streamLine);
            }

            lines = lineList.ToArray();
#else
            //lines = System.IO.File.ReadAllLines(path);
#endif
            foreach (string line in File.ReadAllLines(path))
            {
                float a, b, c;
                int d, e, f, g, h, i, j, k, l;
                string start = line.Split(" ")[0];
                string[] values = line.Substring(start.Length + 1).Replace(".", ",").Split(" ");

                if (start == "v")
                {
                    a = float.Parse(values[0]);
                    b = float.Parse(values[1]);
                    c = float.Parse(values[2]);

                    _verticies.Add(new Vector3(a, b, c));
                }
                else if (start == "vt")
                {
                    a = float.Parse(values[0]);
                    b = float.Parse(values[1]);

                    _uvs.Add(new Vector2(a, b));
                }
                else if (start == "vn")
                {
                    a = float.Parse(values[0]);
                    b = float.Parse(values[1]);
                    c = float.Parse(values[2]);

                    _normals.Add(new Vector3(a, b, c));
                }
                else if (start == "f")
                {

                    string[] ids = values[0].Split("/");

                    d = int.Parse(ids[0]); //X
                    g = int.Parse(ids[1]); //UVX
                    j = int.Parse(ids[2]); //NX

                    ids = values[1].Split("/");

                    e = int.Parse(ids[0]); //Y
                    h = int.Parse(ids[1]); //UVY
                    k = int.Parse(ids[2]); //NY

                    ids = values[2].Split("/");

                    f = int.Parse(ids[0]); //Z
                    i = int.Parse(ids[1]); //UVZ
                    l = int.Parse(ids[2]); //NZ

                    //x y z = d e f
                    //uvx uvy uvz = g h i
                    //nx ny nz = j k l

                    Face face = new Face(
                        d, e, f,
                        g, h, i,
                        j, k, l);
                    _faces.Add(face);

                }

            }
        }

        public Mesh ToMesh()
        {
            int faceCount = _faces.Count;
            int vertexCount = _verticies.Count;
            int uvCount = _uvs.Count;

            VertexPositionNormalTexture[] vertexPositions = new VertexPositionNormalTexture[faceCount * 3];

            for (int i = 0; i < faceCount; i++)
            {

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

            return new Mesh(vertexPositions, faceCount, vertexCount);
        }
    }
}
