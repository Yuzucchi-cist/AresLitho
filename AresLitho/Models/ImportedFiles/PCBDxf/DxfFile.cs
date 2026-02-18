using AresLitho.Models.ImportedFiles.GenericDxf;
using AresLitho.Services.Dxf;
using netDxf;
using System.IO;
using System.Text.RegularExpressions;

namespace AresLitho.Models.ImportedFiles.PCBDxf
{
    abstract partial class DxfFile : ImportedFile
    {
        public DxfDocument DxfDocument { get; }
        public byte[]? Bgr32Image
        {
            get => BinImage?.EncodeToBgr32();
        }

        public DxfFile(string path) : base(path, LoadAndRasterize(path, out DxfDocument dxfDocument))
        {
            // File format validation
            ValidateFileFormat(path);
            DxfDocument = dxfDocument;
        }

        public static new DxfFile Load(string path)
        {
            DxfFile? dxfFile = LoadKicadFileFormat(path);

            return dxfFile ?? new GenericDxfFile(path);
        }

        private static BinImage LoadAndRasterize(string path, out DxfDocument dxfDocument)
        {
            dxfDocument = DxfService.Load(path);
            return DxfRasterizer.LoadAndRasterize(dxfDocument).FillClosedAreas();
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
