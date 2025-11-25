using AresLitho.Services;
using netDxf;
using System.IO;
using System.Text.RegularExpressions;

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

        public DxfFile(string path)
        {
            // File format validation
            ValidateFileFormat(path);

            DxfDocument = DxfService.Load(path);
            BinImage = DxfRasterizer.LoadAndRasterize(DxfDocument).FillClosedAreas();

            _path = path;
        }

        public static DxfFile Load(string path)
        {
            DxfFile? dxfFile = LoadKicadFileFormat(path);

            return dxfFile is not null
                ? dxfFile
                : throw new ArgumentException("The file format is not supported.", path);
        }

        private static DxfFile? LoadKicadFileFormat(string path)
        {
            string filename = System.IO.Path.GetFileName(path);
            var match = _KicadFileFormatRegex().Match(filename);

            if (!match.Success)
            {
                return null;
            }

            if (match.Groups["edge"].Success)
            {
                return new EdgeCutLayer(path);
            }
            else
            {
                if (match.Groups["type"].Value == "Cu")
                {
                    if (match.Groups["side"].Value == "F")
                    {
                        return new CuLayer(path, CuLayerSide.Front);
                    }
                    else if (match.Groups["side"].Value == "B")
                    {
                        return new CuLayer(path, CuLayerSide.Back);
                    }
                    else if (match.Groups["side"].Value.StartsWith("In"))
                    {
                        return new CuLayer(path, CuLayerSide.Inner, Int32.Parse(match.Groups["innerNum"].Value));
                    }
                }
                else if (match.Groups["type"].Value == "Paste")
                {
                    if (match.Groups["side"].Value == "F")
                    {
                        return new PasteLayer(path, PasteLayerSide.Front);
                    }
                    else if (match.Groups["side"].Value == "B")
                    {
                        return new PasteLayer(path, PasteLayerSide.Back);
                    }
                }
            }
            return null;
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


        [GeneratedRegex(@"^.*?(?<layer>(?<side>F|B|In(?<innerNum>\d))_(?<type>Cu|Paste)|(?<edge>Edge_Cuts))\.dxf$")]
        private static partial Regex _KicadFileFormatRegex();
    }
}
