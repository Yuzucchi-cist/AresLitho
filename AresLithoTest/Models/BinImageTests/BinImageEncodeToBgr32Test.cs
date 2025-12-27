using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageEncodeToBgr32Test
    {
        [TestMethod]
        public void EncodeToBgr32_TwoPixels_MappingIsCorrect()
        {
            // Arrange
            var img = new BinImage(2, 1);
            img.SetPixel(0, 0, true);  // filled -> 0x00
            img.SetPixel(1, 0, false); // empty  -> 0xFF

            // Act
            var bytes = img.EncodeToBgr32();

            // Assert: pattern-based check and explicit byte-array check
            BinImageAssert.MatchesBgr32(img, new[] { "#." });

            var expected = new byte[]
            {
                0x00,0x00,0x00,0x00, // pixel (0,0)
                0xFF,0xFF,0xFF,0xFF  // pixel (1,0)
            };
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBgr32_RowMajorOrder_IsPreserved()
        {
            // Arrange
            var img = new BinImage(2, 2);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var bytes = img.EncodeToBgr32();

            // Assert using pattern helper (row-major)
            BinImageAssert.MatchesBgr32(img, new[]
            {
                "#.",
                ".#"
            });

            // Explicit expected bytes (row-major: (0,0),(1,0),(0,1),(1,1))
            var expected = new byte[]
            {
                0x00,0x00,0x00,0x00, // (0,0) true
                0xFF,0xFF,0xFF,0xFF, // (1,0) false
                0xFF,0xFF,0xFF,0xFF, // (0,1) false
                0x00,0x00,0x00,0x00  // (1,1) true
            };
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBgr32_SinglePixel_ProducesFourBytes()
        {
            // Arrange
            var img = new BinImage(1, 1);
            img.SetPixel(0, 0, true);

            // Act
            var bytes = img.EncodeToBgr32();

            // Assert: pattern and length
            BinImageAssert.MatchesBgr32(img, new[] { "#" });
            Assert.HasCount(4, bytes);

            // Explicit expected bytes
            var expected = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBgr32_AllFalse_ProducesAllFFBytes()
        {
            // Arrange
            var img = new BinImage(3, 2); // 3x2 all false

            // Act
            var bytes = img.EncodeToBgr32();

            // Assert via pattern helper
            BinImageAssert.MatchesBgr32(img, new[]
            {
                "...",
                "..."
            });

            // Explicit expected bytes (all 0xFF)
            var expected = new byte[3 * 2 * 4];
            for (int i = 0; i < expected.Length; i++) expected[i] = 0xFF;
            BinImageAssert.BytesEqual(expected, bytes);
        }

        [TestMethod]
        public void EncodeToBgr32_ZeroSize_ReturnsEmptyArray()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var bytes = img.EncodeToBgr32();

            // Assert
            Assert.IsNotNull(bytes);
            Assert.IsEmpty(bytes);

            // Explicit expected empty array
            var expected = Array.Empty<byte>();
            BinImageAssert.BytesEqual(expected, bytes);
        }
    }
}