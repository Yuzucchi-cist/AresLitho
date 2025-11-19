using AresLitho.Services;
using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.PCBDxf
{
    internal class CuLayer : DxfFile
    {
        public CuLayerSide LayerSide { get; }
        public List<Via> Vias { get; } = [];
        public int? innerLayerNum;

        public CuLayer(string path, CuLayerSide layerSide) : this(path, layerSide, null) { }

        public CuLayer(string path, CuLayerSide layerSide, int? innerLayerNum) : base(path)
        {
            LayerSide = layerSide;
            foreach (EntityObject? entity in DxfDocument.Entities.All)
            {
                if (entity is Circle circle)
                    Vias.Add(new Via(circle.Center.X, circle.Center.Y, circle.Radius));
            }
            this.innerLayerNum = innerLayerNum;
        }
    }

    enum CuLayerSide
    {
        Front,
        Back,
        Inner
    }

    internal record Via(double X, double Y, double Radius)
    {
        public double X { get; } = X;
        public double Y { get; } = Y;
        public double Radius { get; } = Radius;

        public int PixelX { get => (int)(X / DxfRasterizer.pixelSizeMm); }
        public int PixelY { get => (int)(Y / DxfRasterizer.pixelSizeMm); }
        public int PixelRadius { get => (int)(Radius / DxfRasterizer.pixelSizeMm); }
    }
}
