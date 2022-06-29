using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector.HxPly
{
    public class PlyFile
    {
        public Header Header;
        
        public List<VertexPositionNormalColorTexture> VertexData = new List<VertexPositionNormalColorTexture>();
        public List<int[]> IndexData = new List<int[]>();

        public PlyFile()
        {
            Header.ElementDictionary = new Dictionary<ElementType, Element>();
            Header.ElementList = new List<Element>();
        }
        
    }
}