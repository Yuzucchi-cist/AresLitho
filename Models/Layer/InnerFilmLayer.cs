using AresLitho.Models.PCBDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.Layer
{
    internal class InnerFilmLayer : Layer
    {
        private readonly EdgeCutLayer EdgeCutLayer;
        private readonly CuLayer UpperHoleLayer;
        private readonly CuLayer LowerHoleLayer;

        public InnerFilmLayer(EdgeCutLayer edgeCutLayer, CuLayer upperHoleLayer, CuLayer lowerHoleLayer)
            : base(GenerateLayerName(upperHoleLayer, lowerHoleLayer), GenerateBinImage(edgeCutLayer, (upperHoleLayer, lowerHoleLayer)))
        {
            EdgeCutLayer = edgeCutLayer;
            UpperHoleLayer = upperHoleLayer;
            LowerHoleLayer = lowerHoleLayer;
        }

        private static string GenerateLayerName(CuLayer upper, CuLayer lower)
        {
            string lowerLayerName = lower.LayerSide == CuLayerSide.Inner
                ? lower.innerLayerNum.ToString() ?? ""
                : lower.LayerSide.ToString();
            string upperLayerName = upper.LayerSide == CuLayerSide.Inner
                ? upper.innerLayerNum.ToString() ?? ""
                : upper.LayerSide.ToString();

            return $"Inner Film Layer {lowerLayerName} to {upperLayerName}";
        }

        private static BinImage GenerateBinImage(EdgeCutLayer edgeCutLayer, (CuLayer u, CuLayer l) cuLayers)
        {
            BinImage binImage = edgeCutLayer.BinImage;

            var holes = cuLayers.u.Vias.FindAll(cuLayers.l.Vias.Contains);

            foreach (var hole in holes)
                binImage = binImage.DrawFilledCircle(hole.PixelX, hole.PixelY, hole.PixelRadius, false);

            return binImage;
        }
    }
}
