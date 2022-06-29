using System.Collections.Generic;

namespace SeaCollector.HxPly
{
    public struct Header
    {
        public Format Format;
        public Dictionary<ElementType, Element> ElementDictionary;
        public List<Element> ElementList;
    }
}