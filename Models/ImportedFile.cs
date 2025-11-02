using System.IO;

namespace AresLitho.Models
{
    class ImportedFile
    {
        private readonly string _path;
        public string FileName { get { return System.IO.Path.GetFileName(_path); } }
        public string Path { get { return _path; } }

        public ImportedFile(string path)
        {
            // File format validation
            ValidateFileFormat(path);

            _path = path;
        }

        private static void ValidateFileFormat(string path)
        {
            if (!IsImportableFileFormat(path))
            {
                throw new ArgumentException("The file format is not supported.", path);
            }

            if (!IsFileExist(path))
            {
                throw new ArgumentException("The file does not exist.", path);
            }
        }

        private static bool IsImportableFileFormat(string path)
        {
            path = path.Split('.').Last();
            switch (path)
            {
                case "dxf":
                case "stl":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsFileExist(string path)
        {
            return File.Exists(path);
        }
    }
}
