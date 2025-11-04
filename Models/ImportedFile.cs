using netDxf;
using System.IO;

namespace AresLitho.Models
{
    class ImportedFile
    {
        private readonly string _path;
        public string FileName { get { return System.IO.Path.GetFileName(_path); } }
        public string Path { get { return _path; } }
        public bool[,]? BinImage { get; private set; }
        public byte[]? GetImageBytes
        {
            get
            {
                if (BinImage == null) return null;
                byte[] imageBytes = new byte[BinImage.GetLength(0) * BinImage.GetLength(1) * 4];
                for (int y = 0; y < BinImage.GetLength(0); y++)
                    for (int x = 0; x < BinImage.GetLength(1); x++)
                    {
                        int i = (y * BinImage.GetLength(1) + x) * 4;
                        imageBytes[i] = (byte)(BinImage[y, x] ? 0 : 255);
                        imageBytes[i + 1] = (byte)(BinImage[y, x] ? 0 : 255);
                        imageBytes[i + 2] = (byte)(BinImage[y, x] ? 0 : 255);
                        imageBytes[i + 3] = (byte)(BinImage[y, x] ? 0 : 255);
                    }
                return imageBytes;
            }
        }


        public ImportedFile(string path)
        {
            // File format validation
            ValidateFileFormat(path);

            if (System.IO.Path.GetExtension(path).Equals(".dxf", StringComparison.CurrentCultureIgnoreCase))
            {
                DxfDocument dxf = netDxf.DxfDocument.Load(path);
                BinImage = Dxf2Bitmap(dxf);
                BinImage = FillClosedAreas(BinImage);
            }
            else if (System.IO.Path.GetExtension(path).Equals(".stl", StringComparison.CurrentCultureIgnoreCase))
            {
                // STL file reading logic to be implemented
            }

            _path = path;
        }

        private static void ValidateFileFormat(string path)
        {
            if (!IsImportableFileFormat(path))
            {
                throw new ArgumentException("The file format is not supported.", path);
            }

            if (!IsFileExist(path))
            {
                throw new ArgumentException("The file does not exist.", path);
            }
        }

        private static bool IsImportableFileFormat(string path)
        {
            path = path.Split('.').Last();
            switch (path)
            {
                case "dxf":
                case "stl":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsFileExist(string path)
        {
            return File.Exists(path);
        }

        private static bool[,] Dxf2Bitmap(DxfDocument dxf)
        {
            return Dxf2Bitmap(dxf, null, null);
        }

        private static bool[,] Dxf2Bitmap(DxfDocument dxf, int? w, int? h)
        {
            int width = (w != null) ? w.Value : 555;
            int height =(h != null) ? h.Value :  555;
            bool[,] image = new bool[width, height];

            var polylines = dxf.Entities.Polylines2D;

            double minX = polylines.Min(line => (line.Vertexes.Min(vertex => vertex.Position.X)));
            double maxX = polylines.Max(line => (line.Vertexes.Max(vertex => vertex.Position.X)));
            double minY = polylines.Min(line => (line.Vertexes.Min(vertex => vertex.Position.Y)));
            double maxY = polylines.Max(line => (line.Vertexes.Max(vertex => vertex.Position.Y)));

            double scaleX = (width - 1) / (maxX - minX);
            double scaleY = (height - 1) / (maxY - minY);
            double scale = Math.Min(scaleX, scaleY);

            foreach ( var polyline in polylines )
            {
                for (int i = 0; i < polyline.Vertexes.Count - 1; i++)
                {
                    int x1 = (int)((polyline.Vertexes[i].Position.X - minX) * scale);
                    int y1 = (int)((polyline.Vertexes[i].Position.Y - minY) * scale);
                    int x2 = (int)((polyline.Vertexes[i + 1].Position.X - minX) * scale);
                    int y2 = (int)((polyline.Vertexes[i + 1].Position.Y - minY) * scale);
                    DrawLine(image, x1, y1, x2, y2);
                }
            }

            return image;
        }

        private static void DrawLine(bool[,] img, int x1, int y1, int x2, int y2)
        {
            int w = img.GetLength(1);
            int h = img.GetLength(0);

            int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
            int dy = -Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                if (x1 >= 0 && x1 < w && y1 >= 0 && y1 < h)
                {
                    img[h - 1 - y1, x1] = true;
                }
                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    x1 += sx;
                }
                if (e2 <= dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }

        private static bool[,] FillClosedAreas(bool[,] img)
        {
            int height = img.GetLength(0);
            int width = img.GetLength(1);

            bool[,] visited = new bool[height, width];

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            void FloodFill(int x, int y)
            {
                Queue<(int, int)> queue = new();
                queue.Enqueue((x, y));
                visited[y, x] = true;
                while (queue.Count > 0)
                {
                    var (cx, cy) = queue.Dequeue();
                    for (int dir = 0; dir < 4; dir++)
                    {
                        int nx = cx + dx[dir];
                        int ny = cy + dy[dir];
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height &&
                            !img[ny, nx] && !visited[ny, nx])
                        {
                            visited[ny, nx] = true;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                if (!img[0, x] && !visited[0, x])
                {
                    FloodFill(x, 0);
                }
                if (!img[height - 1, x] && !visited[height - 1, x])
                {
                    FloodFill(x, height - 1);
                }
            }
            for (int y = 0; y < height; y++)
            {
                if (!img[y, 0] && !visited[y, 0])
                {
                    FloodFill(0, y);
                }
                if (!img[y, width - 1] && !visited[y, width - 1])
                {
                    FloodFill(width - 1, y);
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!img[y, x] && !visited[y, x])
                    {
                        img[y, x] = true;
                    }
                }
            }
            return img;
        }
    }
}
