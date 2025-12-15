using AresLitho.Models.ImportedFiles;

namespace AresLitho.Models.Layer
{
    internal class SingleLayer : Layer
    {
        public SingleLayer(ImportedFile dxfFile)
            : base(dxfFile.FileName, dxfFile.BinImage) { }
    }
}
