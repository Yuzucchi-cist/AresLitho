using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageDrawLineTest
    {
        [TestMethod]
        public void DrawLine_Horizontal_SetsAllPixelsInRow()
        {
            // Arrange
            var img = new BinImage(5, 3);

            // Act
            var res = img.DrawLine(0, 1, 4, 1);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                ".....",
                "#####",
                "....."
            });
        }

        [TestMethod]
        public void DrawLine_Vertical_SetsAllPixelsInColumn()
        {
            // Arrange
            var img = new BinImage(3, 5);

            // Act
            var res = img.DrawLine(1, 0, 1, 4);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                ".#.",
                ".#.",
                ".#.",
                ".#.",
                ".#."
            });
        }

        [TestMethod]
        public void DrawLine_Diagonal_TopLeftToBottomRight()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var res = img.DrawLine(0, 0, 4, 4);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                "#....",
                ".#...",
                "..#..",
                "...#.",
                "....#"
            });
        }

        [TestMethod]
        public void DrawLine_ZeroLength_SetsSinglePixel()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var res = img.DrawLine(2, 2, 2, 2);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                "...",
                "...",
                "..#"
            });
        }

        [TestMethod]
        public void DrawLine_OutOfBounds_StartClipped_SetsInsidePoints()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var res = img.DrawLine(-2, -2, 2, 2);

            // Assert: points (0,0),(1,1),(2,2) should be set
            BinImageAssert.Matches(res, new[]
            {
                "#....",
                ".#...",
                "..#..",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void DrawLine_SteepSlope_AntiDiagonalSet()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var res = img.DrawLine(0, 4, 4, 0);

            // Assert: anti-diagonal from bottom-left to top-right
            BinImageAssert.Matches(res, new[]
            {
                "....#",
                "...#.",
                "..#..",
                ".#...",
                "#...."
            });
        }
    }
}