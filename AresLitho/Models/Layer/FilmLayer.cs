using AresLitho.Models.ImportedFiles.PCBDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.Layer
{
    internal class FilmLayer : Layer
    {
        private readonly EdgeCutLayer EdgeCutLayer;
        private readonly PasteLayer PasteLayer;

        public FilmLayer(EdgeCutLayer edgeCutLayer, PasteLayer pasteLayer)
            : base(GenerateLayerName(pasteLayer), GenerateBinImage(edgeCutLayer, pasteLayer))
        {
            EdgeCutLayer = edgeCutLayer;
            PasteLayer = pasteLayer;
        }

        private static string GenerateLayerName(PasteLayer pasteLayer)
            => $"Film Layer {pasteLayer.LayerSide}";

        private static BinImage GenerateBinImage(EdgeCutLayer edgeCutLayer, PasteLayer pasteLayer)
            => edgeCutLayer.BinImage.Composite(pasteLayer.BinImage.InvertColor());
    }
}
