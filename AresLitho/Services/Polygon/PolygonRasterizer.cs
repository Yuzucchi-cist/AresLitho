using AresLitho.Models;
using SurfaceAnalyzer;
using System.Numerics;

namespace AresLitho.Services.Polygon
{
    public static class PolygonRasterizer
    {
        public const double pixelSizeMm = 0.018;   // 18 microns per pixel

        public static BinImage LoadAndRasterize(PolygonModel polygon, float z)
        {
            // First, find the intersection points of the polygon with the plane at height z
            List<(Vector2 start, Vector2 end)> intersectionLines = [];

            foreach (var face in polygon.Faces)
            {
                if (face.Vertices.Count() != 3)
                    throw new Exception("Only triangular faces are supported.");


                var v0 = face.Vertices[0].P;
                var v1 = face.Vertices[1].P;
                var v2 = face.Vertices[2].P;

                var vertexes = new[] { (v0, v1), (v1, v2), (v2, v0) };

                List<Vector2> points = [];

                foreach (var (start, end) in vertexes)
                {
                    if (start.Z == z && end.Z == z)
                    {
                        // The entire edge lies on the plane
                        points.Add(new Vector2(start.X, start.Y));
                        points.Add(new Vector2(end.X, end.Y));
                        continue;
                    }
                    else if (start.Z == z)
                    {
                        // The start vertex lies on the plane
                        points.Add(new Vector2(start.X, start.Y));
                    }
                    else if (end.Z == z)
                    {
                        // The end vertex lies on the plane
                        points.Add(new Vector2(end.X, end.Y));
                    }
                    else if (start.Z <= z == end.Z >= z)
                    {
                        // The edge crosses the plane, calculate the intersection point
                        var t = (z - start.Z) / (end.Z - start.Z);
                        var intersectionPoint = start + t * (end - start);
                        points.Add(new Vector2(intersectionPoint.X, intersectionPoint.Y));
                    }
                    else
                    {
                        // The edge does not intersect the plane
                        continue;
                    }
                }

                points = points.Distinct().ToList();

                if (points.Count == 2)
                    intersectionLines.Add((points[0], points[1]));
                else if (points.Count == 3)
                {
                    // If all three vertices lie on the plane, create edges between them
                    intersectionLines.Add((points[0], points[1]));
                    intersectionLines.Add((points[1], points[2]));
                    intersectionLines.Add((points[2], points[0]));
                }

            }

            // Next, we need to convert these intersection points into a 2D raster image
            float minX = (float)intersectionLines.Min(p => Math.Min(p.start.X, p.end.X));
            float minY = (float)intersectionLines.Min(p => Math.Min(p.start.Y, p.end.Y));
            float maxX = (float)intersectionLines.Max(p => Math.Max(p.start.X, p.end.X));
            float maxY = (float)intersectionLines.Max(p => Math.Max(p.start.Y, p.end.Y));

            if (minX < 0 && minY < 0)
            {
                Vector2 offset = new Vector2(-minX, -minY);
                intersectionLines = intersectionLines.Select(
                    l => (l.start + offset, l.end + offset)).ToList();
                maxX += -minX;
                maxY += -minY;
                minX = 0;
                minY = 0;
            }
            else if (minX < 0)
            {
                Vector2 offset = new Vector2(-minX, 0);
                intersectionLines = intersectionLines.Select(
                    l => (l.start + offset, l.end + offset)).ToList();
                maxX += -minX;
                minX = 0;
            }
            else if (minY < 0)
            {
                Vector2 offset = new Vector2(0, -minY);
                intersectionLines = intersectionLines.Select(
                    l => (l.start + offset, l.end + offset)).ToList();
                maxY += -minY;
                minY = 0;
            }

            int width = (int)((maxX - minX) / pixelSizeMm) + 1;
            int height = (int)((maxY - minY) / pixelSizeMm) + 1;

            // Now, create the binary image and draw the lines
            BinImage binImage = new BinImage(width, height);
            foreach (var point in intersectionLines)
            {
                int x1 = (int)((point.start.X / pixelSizeMm) - minX);
                int y1 = (int)((point.start.Y / pixelSizeMm) - minY);
                int x2 = (int)((point.end.X / pixelSizeMm) - minX);
                int y2 = (int)((point.end.Y / pixelSizeMm) - minY);
                binImage = binImage.DrawLine(x1, y1, x2, y2);
            }

            return binImage;
        }
    }
}
