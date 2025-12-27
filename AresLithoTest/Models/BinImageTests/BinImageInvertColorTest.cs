using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageInvertColorTest
    {
        [TestMethod]
        public void InvertColor_AllFalse_BecomesAllTrue()
        {
            // Arrange
            var img = new BinImage(3, 2);

            // Act
            var inverted = img.InvertColor();

            // Assert
            BinImageAssert.Matches(inverted, new[]
            {
                "###",
                "###"
            });
        }

        [TestMethod]
        public void InvertColor_AllTrue_BecomesAllFalse()
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
            var inverted = img.InvertColor();

            // Assert
            BinImageAssert.Matches(inverted, new[]
            {
                "...",
                "..."
            });
        }

        [TestMethod]
        public void InvertColor_MixedPattern_InvertsCorrectly()
        {
            // Arrange
            var img = new BinImage(4, 3);
            // Pattern:
            // #.#.
            // .#.#
            // #.#.
            img.SetPixel(0, 0, true);
            img.SetPixel(2, 0, true);
            img.SetPixel(1, 1, true);
            img.SetPixel(3, 1, true);
            img.SetPixel(0, 2, true);
            img.SetPixel(2, 2, true);

            // Act
            var inverted = img.InvertColor();

            // Assert
            BinImageAssert.Matches(inverted, new[]
            {
                ".#.#",
                "#.#.",
                ".#.#"
            });
        }

        [TestMethod]
        public void InvertColor_SinglePixelTrue_BecomesFalse()
        {
            // Arrange
            var img = new BinImage(1, 1);
            img.SetPixel(0, 0, true);

            // Act
            var inverted = img.InvertColor();

            // Assert
            BinImageAssert.Matches(inverted, new[] { "." });
        }

        [TestMethod]
        public void InvertColor_SinglePixelFalse_BecomesTrue()
        {
            // Arrange
            var img = new BinImage(1, 1);

            // Act
            var inverted = img.InvertColor();

            // Assert
            BinImageAssert.Matches(inverted, new[] { "#" });
        }

        [TestMethod]
        public void InvertColor_ZeroSize_ReturnsZeroSize()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var inverted = img.InvertColor();

            // Assert
            Assert.AreEqual(0, inverted.Width);
            Assert.AreEqual(0, inverted.Height);
        }

        [TestMethod]
        public void InvertColor_DoubleinvertColor_ReturnsOriginal()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);
            img.SetPixel(2, 2, true);

            // Act
            var inverted1 = img.InvertColor();
            var inverted2 = inverted1.InvertColor();

            // Assert - Double inversion should return to original
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Assert.AreEqual(img[x, y], inverted2[x, y],
                        $"Mismatch at ({x},{y})");
                }
            }
        }

        [TestMethod]
        public void InvertColor_ReturnsNewInstance()
        {
            // Arrange
            var img = new BinImage(2, 2);
            img.SetPixel(0, 0, true);

            // Act
            var inverted = img.InvertColor();

            // Assert
            Assert.IsNotNull(inverted);
            Assert.AreNotSame(img, inverted);
            Assert.AreEqual(img.Width, inverted.Width);
            Assert.AreEqual(img.Height, inverted.Height);
        }

        [TestMethod]
        public void InvertColor_LargeImage_PerformsCorrectly()
        {
            // Arrange
            var img = new BinImage(100, 100);
            // Set checkerboard pattern
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        img.SetPixel(x, y, true);
                    }
                }
            }

            // Act
            var inverted = img.InvertColor();

            // Assert - Verify inversion at sample points
            for (int y = 0; y < 100; y += 10)
            {
                for (int x = 0; x < 100; x += 10)
                {
                    Assert.AreEqual(!img[x, y], inverted[x, y],
                        $"Inversion failed at ({x},{y})");
                }
            }
        }
    }
}