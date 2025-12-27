using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageInvertColorTest
    {
        [TestMethod]
        public void InvertColor_MixedPattern_FlipsPixels()
        {
            // Arrange
            // pattern:
            // #..
            // .#.
            var img = new BinImage(3, 2);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var inverted = img.InvertColor();

            // Assert: '#' <-> '.'
            BinImageAssert.Matches(inverted, new[]
            {
                ".##",
                "#.#"
            });
        }

        [TestMethod]
        public void InvertColor_Twice_RestoresOriginal()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(0, 0, true);
            img.SetPixel(2, 2, true);

            // Act
            var once = img.InvertColor();
            var twice = once.InvertColor();

            // Assert: double inversion returns original pattern
            BinImageAssert.Matches(twice, new[]
            {
                "#..",
                "...",
                "..#"
            });
        }

        [TestMethod]
        public void InvertColor_AllTrue_ProducesAllFalse()
        {
            // Arrange
            var img = new BinImage(4, 2);
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    img.SetPixel(x, y, true);

            // Act
            var inverted = img.InvertColor();

            // Assert: all cleared
            BinImageAssert.Matches(inverted, new[]
            {
                "....",
                "...."
            });
        }

        [TestMethod]
        public void InvertColor_ZeroSize_NoThrowAndKeepsZeroSize()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var inverted = img.InvertColor();

            // Assert
            Assert.AreEqual(0, inverted.Width);
            Assert.AreEqual(0, inverted.Height);
        }
    }
}
