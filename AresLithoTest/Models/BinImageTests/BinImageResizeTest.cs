using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageResizeTest
    {
        [TestMethod]
        public void Resize_Expand_PreservesTopLeft()
        {
            // Arrange
            var img = new BinImage(2, 2);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var resized = img.Resize(4, 4);

            // Assert: top-left 2x2 preserved, rest cleared
            BinImageAssert.Matches(resized, new[] {
                "#...",
                ".#..",
                "....",
                "...."
            });
        }

        [TestMethod]
        public void Resize_Shrink_TruncatesToTopLeftRegion()
        {
            // Arrange
            var img = new BinImage(4, 4);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);
            img.SetPixel(2, 2, true);
            img.SetPixel(3, 3, true);

            // Act
            var resized = img.Resize(2, 2);

            // Assert: only top-left 2x2 remains
            BinImageAssert.Matches(resized, new[] {
                "#.",
                ".#"
            });
        }

        [TestMethod]
        public void Resize_WiderAndShorter_PreservesOverlapRegion()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(0, 0, true);
            img.SetPixel(2, 2, true);

            // Act: width expands to 5, height shrinks to 2
            var resized = img.Resize(5, 2);

            // Assert: preserved area is min dims (height 2, width 3) placed at top-left
            BinImageAssert.Matches(resized, new[] {
                "#....",
                "....."
            });
        }

        [TestMethod]
        public void Resize_To1x1_PreservesTopLeftPixel()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(0, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var resized = img.Resize(1, 1);

            // Assert
            BinImageAssert.Matches(resized, new[] { "#" });
        }

        [TestMethod]
        public void Resize_FromZeroZero_ExpandsToClearedGrid()
        {
            // Arrange
            var img = new BinImage(0, 0);

            // Act
            var resized = img.Resize(2, 2);

            // Assert: new area is cleared
            BinImageAssert.Matches(resized, new[] {
                "..",
                ".."
            });
        }

        [TestMethod]
        public void Resize_SameSize_NoChange()
        {
            // Arrange
            var img = new BinImage(3, 2);
            // pattern: #.# / .#.
            img.SetPixel(0, 0, true);
            img.SetPixel(2, 0, true);
            img.SetPixel(1, 1, true);

            // Act
            var resized = img.Resize(3, 2);

            // Assert: unchanged
            BinImageAssert.Matches(resized, [
                "#.#",
                ".#."
            ]);
        }
    }
}