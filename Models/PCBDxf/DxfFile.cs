using AresLitho.Services;
using netDxf;
using System.IO;

namespace AresLitho.Models.PCBDxf
{
    abstract partial class DxfFile
    {
        private readonly string _path;
        public string FileName { get { return System.IO.Path.GetFileName(_path); } }
        public string Path { get { return _path; } }
        public DxfDocument DxfDocument { get; }
        public BinImage BinImage { get; private set; }
        public byte[]? Bgr32Image
        {
            get => BinImage?.EncodeToBgr32();
        }

        public ImportedFileProperty Property
        {
            get
            {
                int width = BinImage?.Width ?? 0;
                int height = BinImage?.Height ?? 0;
                return new ImportedFileProperty(_path, width, height);
            }
        }

        public DxfFile(string path)
        {
            // File format validation
            ValidateFileFormat(path);

            DxfDocument = DxfService.Load(path);
            BinImage = DxfRasterizer.LoadAndRasterize(DxfDocument).FillClosedAreas();

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
