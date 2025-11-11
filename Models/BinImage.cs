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

        public void SetPixel(int x, int y, bool value) =>
            _binImage[y, x] = value;

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

        public BinImage FillClosedAreas()
        {
            int height = _binImage.GetLength(0);
            int width = _binImage.GetLength(1);

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
                            !_binImage[ny, nx] && !visited[ny, nx])
                        {
                            visited[ny, nx] = true;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                if (!_binImage[0, x] && !visited[0, x])
                {
                    FloodFill(x, 0);
                }
                if (!_binImage[height - 1, x] && !visited[height - 1, x])
                {
                    FloodFill(x, height - 1);
                }
            }
            for (int y = 0; y < height; y++)
            {
                if (!_binImage[y, 0] && !visited[y, 0])
                {
                    FloodFill(0, y);
                }
                if (!_binImage[y, width - 1] && !visited[y, width - 1])
                {
                    FloodFill(width - 1, y);
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!_binImage[y, x] && !visited[y, x])
                    {
                        _binImage[y, x] = true;
                    }
                }
            }
            return new BinImage(_binImage);
        }

    }
}
