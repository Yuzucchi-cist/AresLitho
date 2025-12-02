using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLitho.Models.ImportedFiles;

namespace AresLitho.Models.Layer
{
    internal class SingleLayer : Layer
    {
        public SingleLayer(ImportedFile dxfFile)
            : base(dxfFile.FileName, dxfFile.BinImage) { }
    }
}
