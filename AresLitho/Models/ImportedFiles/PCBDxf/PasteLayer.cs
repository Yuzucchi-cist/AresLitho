namespace AresLitho.Models.ImportedFiles.PCBDxf
{
    internal class PasteLayer : DxfFile
    {
        public PasteLayerSide LayerSide { get; }

        public PasteLayer(string path, PasteLayerSide pasteLayerSide) : base(path)
        {
            LayerSide = pasteLayerSide;
        }
    }

    enum PasteLayerSide
    {
        Front,
        Back
    }
}
