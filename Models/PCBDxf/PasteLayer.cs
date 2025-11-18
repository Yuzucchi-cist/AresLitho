using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.PCBDxf
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
