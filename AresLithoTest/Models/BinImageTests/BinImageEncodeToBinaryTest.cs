using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageEncodeToBinaryTest
    {
        [TestMethod]
        public void EncodeToBinary_LengthMatchesDimensions()
        {
            // Arrange
            int width = 5, height = 3;
            var img = new BinImage(width, height);
            img.SetPixel(2, 1, true);

            // Act
            var bytes = img.EncodeToBinary();

            // Assert
            Assert.HasCount(width * height * 2, bytes);
        }

        [TestMethod]
        public void EncodeToBinary_Pattern_MatchesExpectedRowMajorBytes()
        {
            // Arrange
            // pattern:
            // #.
            // .#
            var img = new BinImage(2, 2);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var bytes = img.EncodeToBinary();

            // Expected: each pixel -> 2 bytes (true -> 0xFF,0xFF ; false -> 0x00,0x00)
            var expected = new byte[2 * 2 * 2];
            // (0,0) true
            expected[0] = 0xFF; expected[1] = 0xFF;
            // (1,0) false
            expected[2] = 0x00; expected[3] = 0x00;
            // (0,1) false
            expected[4] = 0x00; expected[5] = 0x00;
            // (1,1) true
            expected[6] = 0xFF; expected[7] = 0xFF;

            // Assert
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBinary_SinglePixel_TrueEncodesFFFF()
        {
            // Arrange
            var img = new BinImage(1, 1);
            img.SetPixel(0, 0, true);

            // Act
            var bytes = img.EncodeToBinary();

            // Assert
            var expected = new byte[] { 0xFF, 0xFF };
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBinary_AllFalse_ProducesAllZeroBytes()
        {
            // Arrange
            int width = 3, height = 2;
            var img = new BinImage(width, height); // all false by default

            // Act
            var bytes = img.EncodeToBinary();

            // Assert
            var expected = new byte[width * height * 2];
            for (int i = 0; i < expected.Length; i++) expected[i] = 0x00;
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBinary_AllTrue_ProducesAllFFFFBytes()
        {
            // Arrange
            int width = 3, height = 2;
            var img = new BinImage(width, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    img.SetPixel(x, y, true);

            // Act
            var bytes = img.EncodeToBinary();

            // Assert
            var expected = new byte[width * height * 2];
            for (int i = 0; i < expected.Length; i += 2)
            {
                expected[i] = 0xFF;
                expected[i + 1] = 0xFF;
            }
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBinary_ZeroSize_ReturnsEmptyArray()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var bytes = img.EncodeToBinary();

            // Assert
            Assert.IsNotNull(bytes);
            Assert.IsEmpty(bytes);
        }
    }
}
