using AresLitho.Models.ImportedFiles.PCBDxf;
using AresLitho.Models.ImportedFiles.Stl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.ImportedFiles
{
    abstract class ImportedFile
    {
        protected readonly string _path;
        public string FileName { get => System.IO.Path.GetFileName(_path); }
        public string Path { get => _path; }
        public BinImage BinImage { get; private set; }

        public ImportedFile(string path, BinImage binImage)
        {
            _path = path;
            BinImage = binImage;
        }

        public static ImportedFile Load(string path)
        {
            switch (System.IO.Path.GetExtension(path).ToLower())
            {
                case ".dxf":
                    return DxfFile.Load(path);
                case ".stl":
                    return StlFile.Load(path);
                default:
                    throw new NotSupportedException($"File extension '{System.IO.Path.GetExtension(path)}' is not supported.");
            }
        }
    }
}
