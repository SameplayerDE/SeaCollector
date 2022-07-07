using System.Collections.Generic;

namespace SeaCollector.HxObj
{
    public class ObjPart
    {
        public ObjFile ParentFile;
        public string Name;
        public MaterialData Material;
        public string MaterialName;
        public List<float[]> Vertices;
        public List<float[]> UVs;
        public List<float[]> Normals;
        public List<Face> Faces;
    }
}