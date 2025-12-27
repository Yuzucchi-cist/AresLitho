using AresLitho.Models;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageContractTest
    {
        [TestMethod]
        public void Constructor_WithWidthHeight_SetsDimensionsAndClearedPixels()
        {
            // Arrange
            int width = 3;
            int height = 2;

            // Act
            var img = new BinImage(width, height);

            // Assert
            Assert.AreEqual(width, img.Width);
            Assert.AreEqual(height, img.Height);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Assert.IsFalse(img.GetPixel(x, y), $"Expected pixel ({x},{y}) to be false by default");
                }
            }
        }

        [TestMethod]
        public void SetPixel_GetPixel_ReflectsChangesAtCoordinates()
        {
            // Arrange
            var img = new BinImage(2, 2);

            // Act
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Assert
            Assert.IsTrue(img.GetPixel(0, 0));
            Assert.IsTrue(img.GetPixel(1, 1));

            // other pixels remain false
            Assert.IsFalse(img.GetPixel(1, 0));
            Assert.IsFalse(img.GetPixel(0, 1));
        }

        [TestMethod]
        public void Constructor_FromByteArray_PopulatesPixelsRowMajor()
        {
            // Arrange
            // width=3, height=2, flattened row-major: row0:{1,0,1}, row1:{0,1,0}
            int width = 3;
            int height = 2;
            byte[] raw = new byte[]
            {
                1, 0, 1,
                0, 1, 0
            };

            // Act
            var img = new BinImage(raw, width, height);

            // Assert: check a few coordinates mapping (x,y)
            Assert.IsTrue(img.GetPixel(0, 0)); // raw[0]
            Assert.IsFalse(img.GetPixel(1, 0)); // raw[1]
            Assert.IsTrue(img.GetPixel(2, 0)); // raw[2]
            Assert.IsFalse(img.GetPixel(0, 1)); // raw[3]
            Assert.IsTrue(img.GetPixel(1, 1)); // raw[4]
            Assert.IsFalse(img.GetPixel(2, 1)); // raw[5]
        }

        [TestMethod]
        public void Constructor_FromBoolArray_PreservesCoordinates()
        {
            // Arrange
            // bool[,] constructed as [height, width]
            bool[,] src = new bool[2, 3]
            {
                { true, false, true },  // y=0
                { false, true, false }  // y=1
            };

            // Act
            var img = new BinImage(src);

            // Assert: check mapping GetPixel(x,y) == src[y,x]
            Assert.AreEqual(src[0, 0], img.GetPixel(0, 0));
            Assert.AreEqual(src[0, 1], img.GetPixel(1, 0));
            Assert.AreEqual(src[0, 2], img.GetPixel(2, 0));
            Assert.AreEqual(src[1, 0], img.GetPixel(0, 1));
            Assert.AreEqual(src[1, 1], img.GetPixel(1, 1));
            Assert.AreEqual(src[1, 2], img.GetPixel(2, 1));
        }
    }
}