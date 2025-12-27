using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageFillClosedAreasTest
    {
        [TestMethod]
        public void FillClosedAreas_SimpleSquareWithHole_FillsHole()
        {
            // Arrange
            var img = new BinImage(5, 5);
            // Draw outer square
            for (int i = 0; i < 5; i++)
            {
                img.SetPixel(i, 0, true); // top
                img.SetPixel(i, 4, true); // bottom
                img.SetPixel(0, i, true); // left
                img.SetPixel(4, i, true); // right
            }
            // Center (2,2) should be empty initially

            // Act
            var result = img.FillClosedAreas();

            // Assert - The enclosed area should be filled
            Assert.IsTrue(result[2, 2], "Center should be filled");
        }

        [TestMethod]
        public void FillClosedAreas_NoClosedAreas_RemainsUnchanged()
        {
            // Arrange
            var img = new BinImage(5, 5);
            // Just a line, no closed area
            for (int i = 0; i < 5; i++)
            {
                img.SetPixel(i, 2, true);
            }

            // Act
            var result = img.FillClosedAreas();

            // Assert - Should remain mostly the same
            Assert.IsTrue(result[2, 2]);
            Assert.IsFalse(result[2, 0]);
            Assert.IsFalse(result[2, 4]);
        }

        [TestMethod]
        public void FillClosedAreas_EmptyImage_RemainsEmpty()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var result = img.FillClosedAreas();

            // Assert
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Assert.IsFalse(result[x, y]);
                }
            }
        }

        [TestMethod]
        public void FillClosedAreas_MultipleHoles_FillsAll()
        {
            // Arrange
            var img = new BinImage(10, 10);
            // Draw two separate closed shapes
            // First square at (1,1) to (4,4)
            for (int i = 1; i <= 4; i++)
            {
                img.SetPixel(i, 1, true);
                img.SetPixel(i, 4, true);
                img.SetPixel(1, i, true);
                img.SetPixel(4, i, true);
            }
            // Second square at (6,6) to (9,9)
            for (int i = 6; i <= 9; i++)
            {
                img.SetPixel(i, 6, true);
                img.SetPixel(i, 9, true);
                img.SetPixel(6, i, true);
                img.SetPixel(9, i, true);
            }

            // Act
            var result = img.FillClosedAreas();

            // Assert - Both enclosed areas should be filled
            Assert.IsTrue(result[2, 2], "First hole should be filled");
            Assert.IsTrue(result[7, 7], "Second hole should be filled");
        }

        [TestMethod]
        public void FillClosedAreas_ReturnsNewInstance()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var result = img.FillClosedAreas();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(img.Width, result.Width);
            Assert.AreEqual(img.Height, result.Height);
        }

        [TestMethod]
        public void FillClosedAreas_SinglePixel_DoesNotCrash()
        {
            // Arrange
            var img = new BinImage(1, 1);
            img.SetPixel(0, 0, true);

            // Act
            var result = img.FillClosedAreas();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FillClosedAreas_ZeroSize_ReturnsZeroSize()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var result = img.FillClosedAreas();

            // Assert
            Assert.AreEqual(0, result.Width);
            Assert.AreEqual(0, result.Height);
        }

        [TestMethod]
        public void FillClosedAreas_ComplexShape_HandlesCorrectly()
        {
            // Arrange
            var img = new BinImage(7, 7);
            // Draw a more complex closed shape
            img.SetPixel(1, 1, true);
            img.SetPixel(2, 1, true);
            img.SetPixel(3, 1, true);
            img.SetPixel(4, 1, true);
            img.SetPixel(5, 1, true);
            img.SetPixel(1, 2, true);
            img.SetPixel(5, 2, true);
            img.SetPixel(1, 3, true);
            img.SetPixel(5, 3, true);
            img.SetPixel(1, 4, true);
            img.SetPixel(5, 4, true);
            img.SetPixel(1, 5, true);
            img.SetPixel(2, 5, true);
            img.SetPixel(3, 5, true);
            img.SetPixel(4, 5, true);
            img.SetPixel(5, 5, true);

            // Act
            var result = img.FillClosedAreas();

            // Assert - Interior should be filled
            Assert.IsTrue(result[3, 3], "Center should be filled");
        }
    }
}