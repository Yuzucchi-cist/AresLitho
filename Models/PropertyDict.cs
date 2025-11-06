namespace AresLitho.Models
{
    public class PropertyDict
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public PropertyDict(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
