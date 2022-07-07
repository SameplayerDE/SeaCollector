using System.Collections.Generic;

namespace SeaCollector.HxObj
{
    public class ObjFile
    {
        public string Name;
        public List<MtlFile> Materials;
        public List<float[]> Vertices;
        public List<float[]> UVs;
        public List<float[]> Normals;
        public List<Face> Faces;
        public List<ObjPart> ObjectParts;
        
    }
}