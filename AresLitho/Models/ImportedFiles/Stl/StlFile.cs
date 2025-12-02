using AresLitho.Services.Polygon;
using SurfaceAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.ImportedFiles.Stl
{
    internal class StlFile : ImportedFile
    {
        private readonly PolygonModel _polygonModel;
        public StlFile(string filePath) : base(filePath, LoadAndRasterize(filePath, out PolygonModel polygon))
        {
            _polygonModel = polygon;
        }

        private static BinImage LoadAndRasterize(string path, out PolygonModel polygon)
        {
            polygon = LoadData.LoadSTL(path);

            // Use the middle Z value for rasterizatio
            var pointZes = polygon.Vertices.Select(v => v.P.Z);
            float z = (pointZes.Max() + pointZes.Min()) / 2;

            return PolygonRasterizer.LoadAndRasterize(polygon, z).FillClosedAreas();
        }

        public static new StlFile Load(string path)
        {
            return new StlFile(path);
        }
    }
}
