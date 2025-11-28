using AresLitho.Models.PCBDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.Layer
{
    internal class SingleLayer : Layer
    {
        public SingleLayer(DxfFile dxfFile)
            : base(dxfFile.FileName, dxfFile.BinImage) { }
    }
}
