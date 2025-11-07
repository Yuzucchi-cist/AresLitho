using netDxf;

namespace AresLitho.Models
{
    public class BinImage
    {
        private bool[,] _binImage;
        public int Width { get => _binImage.GetLength(1); }
        public int Height { get => _binImage.GetLength(0); }
        public byte[,] Byte { get
            {
                byte[,] bytes = new byte[Height, Width];
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        bytes[y, x] = (byte)(_binImage[y, x] ? 0xFF : 0x00);
                return bytes;
            }
        }
        public BinImage(int width, int height)
        {
            _binImage = new bool[height, width];
        }
        public BinImage(bool[,] binImage)
        {
            _binImage = binImage;
        }

        public BinImage(DxfDocument dxf)
        {
            _binImage = Dxf2Bitmap(dxf, null, null);
        }

        public BinImage(DxfDocument dxf, int?width, int? height)
        {
            _binImage = Dxf2Bitmap(dxf, width, height);
        }

        public BinImage Resize(int newWidth, int newHeight)
        {
            bool[,] newImage = new bool[newHeight, newWidth];
            double scaleY = (double)newHeight / _binImage.GetLength(0);
            double scaleX = (double)newWidth / _binImage.GetLength(1);
            for (int y = 0; y < newHeight; y++)
                for (int x = 0; x < newWidth; x++)
                {
                    newImage[y, x] = _binImage[(int)(y / scaleY), (int)(x / scaleX)];
                }
            return new BinImage(newImage);
        }

        public byte[] EncodeToBgr32()
        {
            byte[] imageBytes = new byte[_binImage.GetLength(0) * _binImage.GetLength(1) * 4];
            for (int y = 0; y < _binImage.GetLength(0); y++)
                for (int x = 0; x < _binImage.GetLength(1); x++)
                {
                    int i = (y * _binImage.GetLength(1) + x) * 4;
                    imageBytes[i] = (byte)(_binImage[y, x] ? 0 : 0xff);
                    imageBytes[i + 1] = (byte)(_binImage[y, x] ? 0 : 0xff);
                    imageBytes[i + 2] = (byte)(_binImage[y, x] ? 0 : 0xff);
                    imageBytes[i + 3] = (byte)(_binImage[y, x] ? 0 : 0xff);
                }
            return imageBytes;
        }

        public byte[] EncodeToBinary()
        {
            int height = _binImage.GetLength(0);
            int width = _binImage.GetLength(1);
            byte[] imageData = new byte[height * width * 2];
            int index = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    ushort pixelValue = (ushort)(_binImage[y, x] ? 0xFFFF : 0x0000);
                    imageData[index++] = (byte)((pixelValue >> 8) & 0xFF); // High byte
                    imageData[index++] = (byte)(pixelValue & 0xFF);        // Low byte
                }
            return imageData;
        }

        public BinImage SetToCenter(BinImage source)
        {
            int x1 = (Width + source.Width) / 2;
            int y1 = (Height + source.Height) / 2;
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    _binImage[y1 + y, x1 + x] = source._binImage[y, x];
                }
            }
            return new BinImage(_binImage);
        }

        private static bool[,] Dxf2Bitmap(DxfDocument dxf, int? w, int? h)
        {
            int width = (w != null) ? w.Value : 555;
            int height =(h != null) ? h.Value :  555;
            bool[,] image = new bool [width, height];

            var polylines = dxf.Entities.Polylines2D;

            double minX = polylines.Min(line => (line.Vertexes.Min(vertex => vertex.Position.X)));
            double maxX = polylines.Max(line => (line.Vertexes.Max(vertex => vertex.Position.X)));
            double minY = polylines.Min(line => (line.Vertexes.Min(vertex => vertex.Position.Y)));
            double maxY = polylines.Max(line => (line.Vertexes.Max(vertex => vertex.Position.Y)));

            double scaleX = (width - 1) / (maxX - minX);
            double scaleY = (height - 1) / (maxY - minY);
            double scale = Math.Min(scaleX, scaleY);

            foreach (var polyline in polylines)
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

            image = FillClosedAreas(image);

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
