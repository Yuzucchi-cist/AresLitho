using AresLitho.Models;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageBytePropertyTest
    {
        [TestMethod]
        public void ByteProperty_AllFalse_ReturnsZeroArray()
        {
            // Arrange
            var img = new BinImage(3, 2);

            // Act
            var bytes = img.Byte;

            // Assert
            Assert.AreEqual(2, bytes.GetLength(0)); // height
            Assert.AreEqual(3, bytes.GetLength(1)); // width
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Assert.AreEqual(0x00, bytes[y, x]);
                }
            }
        }

        [TestMethod]
        public void ByteProperty_AllTrue_ReturnsFFArray()
        {
            // Arrange
            var img = new BinImage(3, 2);
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    img.SetPixel(x, y, true);
                }
            }

            // Act
            var bytes = img.Byte;

            // Assert
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Assert.AreEqual(0xFF, bytes[y, x]);
                }
            }
        }

        [TestMethod]
        public void ByteProperty_MixedPattern_CorrectlyConverts()
        {
            // Arrange
            var img = new BinImage(3, 2);
            // Pattern: #.#
            //          .#.
            img.SetPixel(0, 0, true);
            img.SetPixel(2, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var bytes = img.Byte;

            // Assert
            Assert.AreEqual(0xFF, bytes[0, 0]); // (0,0) true
            Assert.AreEqual(0x00, bytes[0, 1]); // (1,0) false
            Assert.AreEqual(0xFF, bytes[0, 2]); // (2,0) true
            Assert.AreEqual(0x00, bytes[1, 0]); // (0,1) false
            Assert.AreEqual(0xFF, bytes[1, 1]); // (1,1) true
            Assert.AreEqual(0x00, bytes[1, 2]); // (2,1) false
        }

        [TestMethod]
        public void ByteProperty_SinglePixelTrue_ReturnsFF()
        {
            // Arrange
            var img = new BinImage(1, 1);
            img.SetPixel(0, 0, true);

            // Act
            var bytes = img.Byte;

            // Assert
            Assert.AreEqual(1, bytes.GetLength(0));
            Assert.AreEqual(1, bytes.GetLength(1));
            Assert.AreEqual(0xFF, bytes[0, 0]);
        }

        [TestMethod]
        public void ByteProperty_SinglePixelFalse_ReturnsZero()
        {
            // Arrange
            var img = new BinImage(1, 1);

            // Act
            var bytes = img.Byte;

            // Assert
            Assert.AreEqual(0x00, bytes[0, 0]);
        }

        [TestMethod]
        public void ByteProperty_ZeroSize_ReturnsEmptyArray()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var bytes = img.Byte;

            // Assert
            Assert.IsNotNull(bytes);
            Assert.AreEqual(0, bytes.GetLength(0));
            Assert.AreEqual(0, bytes.GetLength(1));
        }

        [TestMethod]
        public void ByteProperty_CalledMultipleTimes_ReturnsConsistentResults()
        {
            // Arrange
            var img = new BinImage(2, 2);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var bytes1 = img.Byte;
            var bytes2 = img.Byte;

            // Assert - Both calls should return same values
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    Assert.AreEqual(bytes1[y, x], bytes2[y, x]);
                }
            }
        }

        [TestMethod]
        public void ByteProperty_AfterModification_ReflectsChanges()
        {
            // Arrange
            var img = new BinImage(2, 2);
            var bytesBefore = img.Byte;
            Assert.AreEqual(0x00, bytesBefore[0, 0]);

            // Act
            img.SetPixel(0, 0, true);
            var bytesAfter = img.Byte;

            // Assert
            Assert.AreEqual(0xFF, bytesAfter[0, 0]);
        }
    }
}