using netDxf;
using netDxf.IO;

namespace AresLitho.Services.Dxf
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
