using OpenCvSharp;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AresLitho.Models
{
    public class BinImage
    {
        private bool[,] _binImage;
        public bool this[int x, int y] { get => GetPixel(x, y); }
        public int Width { get => _binImage.GetLength(1); }
        public int Height { get => _binImage.GetLength(0); }
        public byte[,] Byte
        {
            get
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

        public BinImage(byte[] bytes, int width, int height)
        {
            _binImage = new bool[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;
                    _binImage[y, x] = bytes[i] != 0;
                }
        }

        public bool GetPixel(int x, int y) => _binImage[y, x];

        public void SetPixel(int x, int y, bool value) =>
            _binImage[y, x] = value;

        public BinImage Invert(bool x, bool y)
        {
            bool[,] newImage = new bool[Height, Width];
            for (int iy = 0; iy < Height; iy++)
                for (int ix = 0; ix < Width; ix++)
                    newImage[iy, ix] = _binImage[y ? Height - iy - 1 : iy, x ? Width - ix - 1 : ix];
            return new BinImage(newImage);
        }

        public BinImage Resize(int newWidth, int newHeight)
        {
            bool[,] newImage = new bool[newHeight, newWidth];

            int copyWidth = Math.Min(Width, newWidth);
            int copyHeight = Math.Min(Height, newHeight);

            for (int y = 0; y < copyHeight; y++)
                Array.Copy(_binImage, y * Width, newImage, y * newWidth, copyWidth);

            return new BinImage(newImage);
        }

        public BinImage Scale(int newWidth, int newHeight)
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

        public BinImage DrawLine(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                if (x1 >= 0 && x1 < Width && y1 >= 0 && y1 < Height)
                {
                    _binImage[y1, x1] = true;
                }
                if (x1 == x2 && y1 == y2) break;
                int err2 = 2 * err;
                if (err2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (err2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
            return new BinImage(_binImage);
        }

        public BinImage DrawCircle(int centerX, int centerY, int radius)
        {
            // Convert to OpenCV Mat
            Mat mat = ConvertToMat(_binImage);

            // Draw circle
            Cv2.Circle(mat, new Point(centerX, centerY), radius, Scalar.White, 1);

            // Convert back to BinImage
            byte[] outputBytes = new byte[mat.Height * mat.Width * mat.ElemSize()];
            Marshal.Copy(mat.Data, outputBytes, 0, outputBytes.Length);
            return new BinImage(outputBytes, Width, Height);
        }

        public BinImage DrawFilledCircle(int centerX, int centerY, int radius, bool fillValue)
        {
            int r2 = radius * radius;

            int yStart = Math.Max(0, centerY - radius);
            int yEnd = Math.Min(Height - 1, centerY + radius);
            int xStart = Math.Max(0, centerX - radius);
            int xEnd = Math.Min(Width - 1, centerX + radius);

            for (int y = yStart; y <= yEnd; y++)
            {
                int dy = y - centerY;
                int dy2 = dy * dy;

                for (int x = xStart; x <= xEnd; x++)
                {
                    int dx = x - centerX;
                    if (dx * dx + dy2 <= r2)
                    {
                        _binImage[y, x] = fillValue;
                    }
                }
            }

            return new BinImage(_binImage);
        }

        public BinImage InvertColor()
        {
            bool[,] newBinImage = new bool[Height, Width];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    newBinImage[y, x] = !_binImage[y, x];
            return new BinImage(newBinImage);
        }

        public BinImage Composite(BinImage source) => Composite(source, 0, 0);

        public BinImage Composite(BinImage source, int offsetX, int offsetY)
        {
            bool[,] newBinImage = new bool[Height, Width];
            Array.Copy(_binImage, newBinImage, _binImage.Length);
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    int targetX = offsetX + x;
                    int targetY = offsetY + y;
                    if (targetX >= 0 && targetX < Width && targetY >= 0 && targetY < Height)
                    {
                        newBinImage[targetY, targetX] = source._binImage[y, x];
                    }
                }
            }
            return new BinImage(newBinImage);
        }

        public BinImage SetToCenter(BinImage source)
        {
            int x1 = (Width - source.Width) / 2;
            int y1 = (Height - source.Height) / 2;
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
            // Convert to OpenCV Mat
            Mat mat = ConvertToMat(_binImage);

            // Find contours
            Cv2.FindContours(mat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            // Fill only the child contours (closed areas)
            contours = contours.Where((_, i) => hierarchy[i].Parent >= 0).ToArray();
            Cv2.FillPoly(mat, contours, Scalar.White);

            // Convert back to BinImage
            byte[] outputBytes = new byte[mat.Height * mat.Width * mat.ElemSize()];
            Marshal.Copy(mat.Data, outputBytes, 0, outputBytes.Length);
            return new BinImage(outputBytes, Width, Height);
        }

        private Mat ConvertToMat(bool[,] img)
        {
            Mat mat = new(Height, Width, MatType.CV_8UC1);
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    mat.Set(y, x, img[y, x] ? 255 : 0);
            return mat;
        }
    }
}
