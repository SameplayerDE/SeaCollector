using System.Collections.Generic;

namespace SeaCollector.HxObj
{
    public class Face
    {
        public FaceType Type;
        public MaterialData Material;
        public string MaterialName;
        public int DataGroupCount => Data.Count;
        public readonly List<int[]> Data;

        public Face()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Data = new List<int[]>();
        }
        
        public Face(FaceType type)
        {
            Type = type;
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Data = new List<int[]>();
        }

        public void AddData(int[] data)
        {
            Data.Add(data);
        }

        public void SetType(FaceType type)
        {
            Type = type;
        }
        
    }
}