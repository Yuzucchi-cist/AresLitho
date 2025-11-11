using netDxf;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Services
{
    internal static class DxfService
    {
        public static DxfDocument Load(string path)
        {
            try
            {
                return DxfDocument.Load(path);
            }
            catch (DxfVersionNotSupportedException)
            {
                return R12DxfLoader.Load(path);
            }
        }
    }
}
