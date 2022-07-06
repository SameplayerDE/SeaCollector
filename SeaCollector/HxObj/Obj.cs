using System.Collections.Generic;

namespace SeaCollector.HxObj
{
    public class Obj
    {
        public ObjFile ParentFile;
        public string Name;
        public MaterialData Material;
        public List<float[]> Vertices;
        public List<float[]> UVs;
        public List<float[]> Normals;
        public List<Face> Faces;
    }
}