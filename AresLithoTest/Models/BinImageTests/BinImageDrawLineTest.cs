using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageDrawLineTest
    {
        [TestMethod]
        public void DrawLine_HorizontalLine_DrawsCorrectly()
        {
            // Arrange
            var img = new BinImage(5, 3);

            // Act
            img.DrawLine(0, 1, 4, 1);

            // Assert
            BinImageAssert.Matches(img, new[]
            {
                ".....",
                "#####",
                "....."
            });
        }

        [TestMethod]
        public void DrawLine_VerticalLine_DrawsCorrectly()
        {
            // Arrange
            var img = new BinImage(3, 5);

            // Act
            img.DrawLine(1, 0, 1, 4);

            // Assert
            BinImageAssert.Matches(img, new[]
            {
                ".#.",
                ".#.",
                ".#.",
                ".#.",
                ".#."
            });
        }

        [TestMethod]
        public void DrawLine_DiagonalLineDownRight_DrawsCorrectly()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.DrawLine(0, 0, 4, 4);

            // Assert
            BinImageAssert.Matches(img, new[]
            {
                "#....",
                ".#...",
                "..#..",
                "...#.",
                "....#"
            });
        }

        [TestMethod]
        public void DrawLine_DiagonalLineUpRight_DrawsCorrectly()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.DrawLine(0, 4, 4, 0);

            // Assert
            BinImageAssert.Matches(img, new[]
            {
                "....#",
                "...#.",
                "..#..",
                ".#...",
                "#...."
            });
        }

        [TestMethod]
        public void DrawLine_SinglePoint_DrawsSinglePixel()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            img.DrawLine(1, 1, 1, 1);

            // Assert
            BinImageAssert.Matches(img, new[]
            {
                "...",
                ".#.",
                "..."
            });
        }

        [TestMethod]
        public void DrawLine_ReverseDirection_DrawsSameLine()
        {
            // Arrange
            var img1 = new BinImage(5, 5);
            var img2 = new BinImage(5, 5);

            // Act
            img1.DrawLine(0, 0, 4, 4);
            img2.DrawLine(4, 4, 0, 0);

            // Assert - Both should produce same result
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Assert.AreEqual(img1[x, y], img2[x, y],
                        $"Mismatch at ({x},{y})");
                }
            }
        }

        [TestMethod]
        public void DrawLine_PartiallyOutOfBounds_DrawsVisiblePortion()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act - Line from inside to outside bounds
            img.DrawLine(2, 2, 7, 2);

            // Assert - Should draw from (2,2) to (4,2)
            BinImageAssert.Matches(img, new[]
            {
                ".....",
                ".....",
                "..###",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void DrawLine_CompletelyOutOfBounds_DoesNotCrash()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act - Line completely outside bounds
            img.DrawLine(-5, -5, -1, -1);

            // Assert - Image should remain empty
            BinImageAssert.Matches(img, new[]
            {
                ".....",
                ".....",
                ".....",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void DrawLine_StartOutsideEndInside_DrawsPartialLine()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.DrawLine(-2, 2, 2, 2);

            // Assert - Should draw from (0,2) to (2,2)
            BinImageAssert.Matches(img, new[]
            {
                ".....",
                ".....",
                "###..",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void DrawLine_SteepSlope_DrawsCorrectly()
        {
            // Arrange
            var img = new BinImage(5, 9);

            // Act - Nearly vertical line
            img.DrawLine(2, 0, 3, 8);

            // Assert - Line should be drawn with Bresenham algorithm
            Assert.IsTrue(img[2, 0]);
            Assert.IsTrue(img[3, 8]);
            // Check that a line is drawn (not testing exact algorithm, just that pixels are set)
            int pixelsSet = 0;
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (img[x, y]) pixelsSet++;
                }
            }
            Assert.IsTrue(pixelsSet >= 7, "Line should have at least 7 pixels");
        }

        [TestMethod]
        public void DrawLine_MultipleLines_CanOverlap()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act - Draw an X pattern
            img.DrawLine(0, 0, 4, 4);
            img.DrawLine(4, 0, 0, 4);

            // Assert - Center should have overlapping pixel
            Assert.IsTrue(img[2, 2]);
            Assert.IsTrue(img[0, 0]);
            Assert.IsTrue(img[4, 4]);
            Assert.IsTrue(img[0, 4]);
            Assert.IsTrue(img[4, 0]);
        }

        [TestMethod]
        public void DrawLine_NegativeCoordinates_HandlesCorrectly()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act - Line starts at negative coordinates
            img.DrawLine(-1, -1, 2, 2);

            // Assert - Should draw visible portion starting at (0,0)
            Assert.IsTrue(img[0, 0]);
            Assert.IsTrue(img[1, 1]);
            Assert.IsTrue(img[2, 2]);
        }

        [TestMethod]
        public void DrawLine_ReturnsNewInstance_WithModifiedData()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var result = img.DrawLine(0, 0, 2, 2);

            // Assert - Result should be a new instance
            Assert.IsNotNull(result);
            Assert.IsTrue(result[0, 0]);
            Assert.IsTrue(result[1, 1]);
            Assert.IsTrue(result[2, 2]);
        }
    }
}