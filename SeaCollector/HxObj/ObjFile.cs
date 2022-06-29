using System.Collections.Generic;

namespace SeaCollector.HxObj
{
    public class ObjFile
    {
        public string Name;
        public List<float[]> Vertices;
        public List<float[]> UVs;
        public List<float[]> Normals;
        public List<Face> Faces;
    }
}