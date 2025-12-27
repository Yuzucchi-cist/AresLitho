using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageDrawFilledCircleTest
    {
        [TestMethod]
        public void DrawFilledCircle_RadiusZero_SetsOnlyCenterWhenTrue()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var res = img.DrawFilledCircle(1, 1, 0, true);

            // Assert
            BinImageAssert.Matches(res, new[]
            {
                "...",
                ".#.",
                "..."
            });
        }

        [TestMethod]
        public void DrawFilledCircle_RadiusOne_FillsPlusShape()
        {
            // Arrange
            var img = new BinImage(3, 3);

            // Act
            var res = img.DrawFilledCircle(1, 1, 1, true);

            // Assert: dx*dx + dy*dy <= 1 -> plus-shape including center
            BinImageAssert.Matches(res, new[]
            {
                ".#.",
                "###",
                ".#."
            });
        }

        [TestMethod]
        public void DrawFilledCircle_RadiusTwo_FillsDiskApproximation()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var res = img.DrawFilledCircle(2, 2, 2, true);

            // Assert: filled disk radius 2
            BinImageAssert.Matches(res, new[]
            {
                "..#..",
                ".###.",
                "#####",
                ".###.",
                "..#.."
            });
        }

        [TestMethod]
        public void DrawFilledCircle_CenterAtCorner_ClipsOutOfBounds()
        {
            // Arrange
            var img = new BinImage(4, 4);

            // Act
            var res = img.DrawFilledCircle(0, 0, 2, true);

            // Assert: only pixels inside image set (clipping)
            BinImageAssert.Matches(res, new[]
            {
                "###.",
                "##..",
                "#...",
                "...."
            });
        }

        [TestMethod]
        public void DrawFilledCircle_FillFalse_ClearsRegionLeavingOthers()
        {
            // Arrange
            var img = new BinImage(5, 5);
            // initialize all pixels to true
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    img.SetPixel(x, y, true);

            // Act: clear a radius-1 disk at center
            var res = img.DrawFilledCircle(2, 2, 1, false);

            // Assert: plus-shaped cleared region, others remain true
            BinImageAssert.Matches(res, new[]
            {
                "#####",
                "##.##",
                "#...#",
                "##.##",
                "#####"
            });
        }

        [TestMethod]
        public void DrawFilledCircle_ZeroSizeImage_NoThrowAndKeepsZeroSize()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var res = img.DrawFilledCircle(0, 0, 3, true);

            // Assert
            Assert.AreEqual(0, res.Width);
            Assert.AreEqual(0, res.Height);
        }
    }
}
