using AresLitho.Models.ImportedFiles.PCBDxf;

namespace AresLitho.Models.Layer
{
    internal class MetalLayer : Layer
    {
        private readonly CuLayer CuLayer;

        public MetalLayer(CuLayer cuLayer)
            : base(GenerateLayerName(cuLayer), cuLayer.BinImage)
        {
            CuLayer = cuLayer;
        }

        private static string GenerateLayerName(CuLayer cuLayer)
        {
            switch (cuLayer.LayerSide)
            {
                case CuLayerSide.Front:
                    return "Metal Layer F";

                case CuLayerSide.Back:
                    return "Metal Layer B";

                case CuLayerSide.Inner:
                    return $"Metal Layer {cuLayer.innerLayerNum}";

                default:
                    throw new ArgumentException("Invalid CuLayerType");
            }
        }
    }
}
