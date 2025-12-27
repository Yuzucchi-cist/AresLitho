using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageDrawCircleTest
    {
        [TestMethod]
        public void DrawCircle_RadiusZero_SetsSinglePixel()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var res = img.DrawCircle(1, 1, 0);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                "...",
                ".#.",
                "..."
            });
        }

        [TestMethod]
        public void DrawCircle_RadiusOne_DrawsCrossLikeCircle()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var res = img.DrawCircle(1, 1, 1);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                ".#.",
                "#.#",
                ".#."
            });
        }

        [TestMethod]
        public void DrawCircle_RadiusTwo_DrawsApproximateCircle()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var res = img.DrawCircle(2, 2, 2);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                "..#..",
                ".#.#.",
                "#...#",
                ".#.#.",
                "..#.."
            });
        }

        [TestMethod]
        public void DrawCircle_CenterAtCorner_ClipsOutOfBoundsPixels()
        {
            // Arrange
            var img = new BinImage(4, 4);

            // Act
            var res = img.DrawCircle(0, 0, 2);

            // Assert: only pixels inside the image should be set
            BinImageAssert.Matches(res, new[]
            {
                "..#.",
                ".#..",
                "#...",
                "...."
            });
        }

        [TestMethod]
        public void DrawCircle_TouchesImageBorder_DoesNotThrowAndDrawsInside()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var res = img.DrawCircle(2, 0, 2);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                "#...#",
                ".#.#.",
                "..#..",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void DrawCircle_DoesNotClearExistingPixels()
        {
            // Arrange
            var img = new BinImage(5, 5);
            img = img.DrawCircle(2, 2, 1); // draw first circle

            // Act
            var res = img.DrawCircle(2, 2, 2); // draw second circle

            // Assert: pixels from both circles should remain set
            BinImageAssert.Matches(res, new[]
            {
                "..#..",
                ".###.",
                "##.##",
                ".###.",
                "..#.."
            });
        }
    }
}
