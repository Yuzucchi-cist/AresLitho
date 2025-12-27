using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageInvertTest
    {
        [TestMethod]
        public void Invert_NoFlip_ReturnsIdentical()
        {
            // Arrange
            var img = new BinImage(3, 2);
            // pattern:
            // #..
            // .#.
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var result = img.Invert(false, false);

            // Assert
            BinImageAssert.Matches(result, new[]
            {
                    "#..",
                    ".#."
                });
        }

        [TestMethod]
        public void Invert_FlipX_HorizontallyFlipped()
        {
            // Arrange
            var img = new BinImage(3, 2);
            // original:
            // #..
            // ..#
            img.SetPixel(0, 0, true);
            img.SetPixel(2, 1, true);

            // Act
            var result = img.Invert(true, false); // horizontal flip

            // Assert: horizontal mirror
            BinImageAssert.Matches(result, new[]
            {
                    "..#",
                    "#.."
                });
        }

        [TestMethod]
        public void Invert_FlipY_VerticallyFlipped()
        {
            // Arrange
            var img = new BinImage(3, 3);
            // original:
            // #..
            // .#.
            // ..#
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);
            img.SetPixel(2, 2, true);

            // Act
            var result = img.Invert(false, true); // vertical flip

            // Assert: vertical mirror (rows reversed)
            BinImageAssert.Matches(result, new[]
            {
                    "..#",
                    ".#.",
                    "#.."
                });
        }

        [TestMethod]
        public void Invert_FlipBoth_FlipsHorizontallyAndVertically()
        {
            // Arrange
            var img = new BinImage(4, 3);
            // original:
            // #..#
            // .##.
            // .... 
            img.SetPixel(0, 0, true);
            img.SetPixel(3, 0, true);
            img.SetPixel(1, 1, true);
            img.SetPixel(2, 1, true);

            // Act
            var result = img.Invert(true, true); // both axes

            // Assert: both axes flipped
            BinImageAssert.Matches(result, new[]
            {
                    "....",
                    ".##.",
                    "#..#"
                });
        }

        [TestMethod]
        public void Invert_SinglePixel_NoChangeFor1x1()
        {
            // Arrange
            var img = new BinImage(1, 1);
            img.SetPixel(0, 0, true);

            // Act
            var r1 = img.Invert(false, false);
            var r2 = img.Invert(true, false);
            var r3 = img.Invert(false, true);
            var r4 = img.Invert(true, true);

            // Assert: all results equal to single filled pixel
            BinImageAssert.Matches(r1, new[] { "#" });
            BinImageAssert.Matches(r2, new[] { "#" });
            BinImageAssert.Matches(r3, new[] { "#" });
            BinImageAssert.Matches(r4, new[] { "#" });
        }

        [TestMethod]
        public void Invert_ZeroSize_DoesNotThrowAndKeepsZeroSize()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var result = img.Invert(true, true);

            // Assert
            Assert.AreEqual(0, result.Width);
            Assert.AreEqual(0, result.Height);
        }
    }
}
