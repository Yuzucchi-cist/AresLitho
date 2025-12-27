using AresLitho.Models;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageIndexerTest
    {
        [TestMethod]
        public void Indexer_Get_ReturnsCorrectPixelValue()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(1, 1, true);
            img.SetPixel(2, 0, true);

            // Act & Assert
            Assert.IsTrue(img[1, 1]);
            Assert.IsTrue(img[2, 0]);
            Assert.IsFalse(img[0, 0]);
            Assert.IsFalse(img[0, 1]);
        }

        [TestMethod]
        public void Indexer_Get_MatchesGetPixelBehavior()
        {
            // Arrange
            var img = new BinImage(4, 4);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    bool value = (x + y) % 2 == 0;
                    img.SetPixel(x, y, value);
                }
            }

            // Act & Assert - Verify indexer matches GetPixel for all positions
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Assert.AreEqual(img.GetPixel(x, y), img[x, y],
                        $"Indexer mismatch at ({x},{y})");
                }
            }
        }

        [TestMethod]
        public void Indexer_BoundaryValues_ReturnsCorrectly()
        {
            // Arrange
            var img = new BinImage(5, 5);
            // Set corners and edges
            img.SetPixel(0, 0, true);           // top-left
            img.SetPixel(4, 0, true);           // top-right
            img.SetPixel(0, 4, true);           // bottom-left
            img.SetPixel(4, 4, true);           // bottom-right
            img.SetPixel(2, 0, true);           // top-middle
            img.SetPixel(2, 4, true);           // bottom-middle

            // Act & Assert
            Assert.IsTrue(img[0, 0]);
            Assert.IsTrue(img[4, 0]);
            Assert.IsTrue(img[0, 4]);
            Assert.IsTrue(img[4, 4]);
            Assert.IsTrue(img[2, 0]);
            Assert.IsTrue(img[2, 4]);
            Assert.IsFalse(img[2, 2]); // center should be false
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_NegativeX_ThrowsException()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var _ = img[-1, 0];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_NegativeY_ThrowsException()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var _ = img[0, -1];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_XOutOfBounds_ThrowsException()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var _ = img[3, 0];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_YOutOfBounds_ThrowsException()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var _ = img[0, 3];
        }

        [TestMethod]
        public void Indexer_AfterModification_ReflectsChanges()
        {
            // Arrange
            var img = new BinImage(2, 2);
            Assert.IsFalse(img[0, 0]);

            // Act
            img.SetPixel(0, 0, true);

            // Assert
            Assert.IsTrue(img[0, 0]);

            // Act
            img.SetPixel(0, 0, false);

            // Assert
            Assert.IsFalse(img[0, 0]);
        }
    }
}