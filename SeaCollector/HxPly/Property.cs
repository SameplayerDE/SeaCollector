namespace SeaCollector.HxPly
{
    public class Property
    {
        public PropertyType Type;
        public string Name;
        public PropertyType ListCountType;
        public PropertyType ListValueType;
        public bool IsList = false;

        public Property(params string[] data)
        {
            Type = data[0] switch
            {
                "float" => PropertyType.Float,
                "uchar" => PropertyType.UnsignedChar,
                "int" => PropertyType.Int,
                "list" => PropertyType.List,
                _ => Type
            };
            IsList = Type == PropertyType.List;

            if (IsList)
            {
                ListCountType = data[1] switch
                {
                    "float" => PropertyType.Float,
                    "uchar" => PropertyType.UnsignedChar,
                    "int" => PropertyType.Int,
                    "list" => PropertyType.List,
                    _ => Type
                };
                ListValueType = data[2] switch
                {
                    "float" => PropertyType.Float,
                    "uchar" => PropertyType.UnsignedChar,
                    "int" => PropertyType.Int,
                    "list" => PropertyType.List,
                    _ => Type
                };
                Name = data[3];
            }
            else
            {
                Name = data[1];
            }
        }
    }
}