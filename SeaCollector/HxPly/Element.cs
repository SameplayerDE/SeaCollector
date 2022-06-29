using System.Collections.Generic;

namespace SeaCollector.HxPly
{
    public class Element
    {
        public readonly ElementType Type;
        public readonly int Count;
        public bool ReadAllData = false;
        public List<Property> Properties;

        public Element(ElementType type, int count)
        {
            Type = type;
            Count = count;
            Properties = new List<Property>();
        }

        public void AddProperty(Property property)
        {
            Properties.Add(property);
        }
        
    }
}