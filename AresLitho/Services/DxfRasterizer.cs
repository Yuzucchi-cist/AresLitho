using AresLitho.Models;
using netDxf;
using netDxf.Entities;

namespace AresLitho.Services
{
    public static class DxfRasterizer
    {
        public const double pixelSizeMm = 0.018;   // 18 microns per pixel

        public static BinImage LoadAndRasterize(DxfDocument dxf)
        {
            var bounds = GetBounds(dxf.Entities.All);
            int width = (int)(bounds.Width / pixelSizeMm) + 1;
            int height = (int)(bounds.Height / pixelSizeMm) + 1;
            var image = new BinImage(width, height);

            foreach (var entity in dxf.Entities.All)
            {
                switch (entity)
                {
                    case Line line:
                        DrawLine(image, line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
                        break;
                    case Polyline2D polyline2D:
                        DrawPolyline2D(image, polyline2D);
                        break;
                    case Circle circle:
                        DrawCircle(image, circle);
                        break;
                    case Arc arc:
                        DrawArc(image, arc);
                        break;
                }
            }
            return image;
        }

        private static Bounds GetBounds(IEnumerable<EntityObject> entities)
        {
            var bounds = new Bounds();
            foreach (var entity in entities)
            {
                switch (entity)
                {
                    case Line line:
                        bounds.Update(line.StartPoint.X, line.StartPoint.Y);
                        bounds.Update(line.EndPoint.X, line.EndPoint.Y);
                        break;
                    case Polyline2D polyline2D:
                        foreach (var vertex in polyline2D.Vertexes)
                        {
                            bounds.Update(vertex.Position.X, vertex.Position.Y);
                        }
                        break;
                    case Circle circle:
                        bounds.Update(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius);
                        bounds.Update(circle.Center.X + circle.Radius, circle.Center.Y + circle.Radius);
                        break;
                    case Arc arc:
                        bounds.Update(arc.Center.X - arc.Radius, arc.Center.Y - arc.Radius);
                        bounds.Update(arc.Center.X + arc.Radius, arc.Center.Y + arc.Radius);
                        break;
                }
            }
            return bounds;
        }

        private static void DrawLine(BinImage image, double x0, double y0, double x1, double y1)
        {
            int ix0 = (int)(x0 / pixelSizeMm);
            int iy0 = (int)(y0 / pixelSizeMm);
            int ix1 = (int)(x1 / pixelSizeMm);
            int iy1 = (int)(y1 / pixelSizeMm);
            int dx = Math.Abs(ix1 - ix0);
            int dy = Math.Abs(iy1 - iy0);
            int sx = ix0 < ix1 ? 1 : -1;
            int sy = iy0 < iy1 ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                image.SetPixel(ix0, iy0, true);
                if (ix0 == ix1 && iy0 == iy1) break;
                int err2 = 2 * err;
                if (err2 > -dy)
                {
                    err -= dy;
                    ix0 += sx;
                }
                if (err2 < dx)
                {
                    err += dx;
                    iy0 += sy;
                }
            }
        }

        private static void DrawPolyline2D(BinImage image, Polyline2D polyline)
        {
            for (int i = 0; i < polyline.Vertexes.Count - 1; i++)
            {
                var v0 = polyline.Vertexes[i].Position;
                var v1 = polyline.Vertexes[i + 1].Position;
                DrawLine(image, v0.X, v0.Y, v1.X, v1.Y);
            }
            if (polyline.IsClosed && polyline.Vertexes.Count > 2)
            {
                var vStart = polyline.Vertexes[0].Position;
                var vEnd = polyline.Vertexes[polyline.Vertexes.Count - 1].Position;
                DrawLine(image, vEnd.X, vEnd.Y, vStart.X, vStart.Y);
            }
        }

        private static void DrawCircle(BinImage image, Circle circle)
        {
            int centerX = (int)(circle.Center.X / pixelSizeMm);
            int centerY = (int)(circle.Center.Y / pixelSizeMm);
            int radius = (int)(circle.Radius / pixelSizeMm);

            image.DrawCircle(centerX, centerY, radius);
        }

        private static void DrawArc(BinImage image, Arc arc)
        {
            int centerX = (int)(arc.Center.X / pixelSizeMm);
            int centerY = (int)(arc.Center.Y / pixelSizeMm);
            int radius = (int)(arc.Radius / pixelSizeMm);
            double startAngleRad = arc.StartAngle * Math.PI / 180.0;
            double endAngleRad = arc.EndAngle * Math.PI / 180.0;
            for (double angle = startAngleRad; angle <= endAngleRad; angle += 0.01)
            {
                int x = centerX + (int)(radius * Math.Cos(angle));
                int y = centerY + (int)(radius * Math.Sin(angle));
                image.SetPixel(x, y, true);
            }
        }
    }

    internal class Bounds
    {
        public double MaxX { get; set; } = double.MinValue;
        public double MaxY { get; set; } = double.MinValue;
        public void Update(double x, double y)
        {
            if (x > MaxX) MaxX = x;
            if (y > MaxY) MaxY = y;
        }
        public double Width => MaxX;
        public double Height => MaxY;
    }
}
