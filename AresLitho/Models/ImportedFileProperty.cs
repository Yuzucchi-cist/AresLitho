using AresLitho.Models;

namespace AresLitho.Models
{
    internal class ImportedFileProperty(string filePath, int width, int height)
    {
        public string FilePath { get; set; } = filePath;
        public (int x, int y) Size { get; set; } = (width, height);

        public List<PropertyDict> ToPropertyDictArray()
        {
            return [new(nameof(FilePath), FilePath), new("Size (X, Y)", $"({Size.x},{Size.y})")];
        }
    }
}
