using AresLitho.Models.ImportedFiles.GenericDxf;

namespace AresLitho.Models.Layer
{
    internal class GenericLayer : Layer
    {
        public GenericLayer(GenericDxfFile dxfFile)
            : base(dxfFile.FileName, dxfFile.BinImage) { }
    }
}
