using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageScaleTest
    {
        [TestMethod]
        public void Scale_Enlarge_PreservesTopLeftBlock()
        {
            // Arrangqe
            var src = new BinImage(2, 2);
            // pattern:
            // #.
            // .#
            src.SetPixel(0, 0, true);
            src.SetPixel(1, 1, true);

            // Act
            var scaled = src.Scale(4, 4);

            // Assert: nearest-neighbor-style sampling; src (0,0) -> block (0..1,0..1), src(1,1)-> block (2..3,2..3)
            BinImageAssert.Matches(scaled, new[]
            {
                    "##..",
                    "##..",
                    "..##",
                    "..##"
                });
        }

        [TestMethod]
        public void Scale_Shrink_SamplesTopLeftAndMiddleRows()
        {
            // Arrange
            var src = new BinImage(4, 4);
            // diagonal pattern
            src.SetPixel(0, 0, true);
            src.SetPixel(1, 1, true);
            src.SetPixel(2, 2, true);
            src.SetPixel(3, 3, true);

            // Act
            var scaled = src.Scale(2, 2);

            // Assert: mapping picks rows 0 and 2 and cols 0 and 2 -> resulting diagonal
            BinImageAssert.Matches(scaled, new[]
            {
                    "#.",
                    ".#"
                });
        }

        [TestMethod]
        public void Scale_NonUniform_WiderAndShorter_PreservesSampledRow()
        {
            // Arrange
            var src = new BinImage(3, 2);
            // top row: #.#
            src.SetPixel(0, 0, true);
            src.SetPixel(2, 0, true);
            // bottom row left empty

            // Act: expand width x2, shrink height to 1
            var scaled = src.Scale(6, 1);

            // Assert: single row where cols [0,1]->src.x=0 true, [2,3]->src.x=1 false, [4,5]->src.x=2 true
            BinImageAssert.Matches(scaled, new[]
            {
                    "##..##"
                });
        }

        [TestMethod]
        public void Scale_SameSize_NoChange()
        {
            // Arrange
            var src = new BinImage(3, 2);
            // pattern:
            // #.#
            // .#.
            src.SetPixel(0, 0, true);
            src.SetPixel(2, 0, true);
            src.SetPixel(1, 1, true);

            // Act
            var scaled = src.Scale(3, 2);

            // Assert: identical
            BinImageAssert.Matches(scaled, new[]
            {
                    "#.#",
                    ".#."
                });
        }

        [TestMethod]
        public void Scale_ToZeroSize_ReturnsZeroSizeImage()
        {
            // Arrange
            var src = new BinImage(2, 2);
            src.SetPixel(0, 0, true);

            // Act
            var scaled = src.Scale(0, 0);

            // Assert
            Assert.AreEqual(0, scaled.Width);
            Assert.AreEqual(0, scaled.Height);
        }

        [TestMethod]
        public void Scale_FromZeroSize_ThrowsIndexOutOfRangeException()
        {
            // Arrange
            var src = new BinImage(0, 0);

            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => src.Scale(2, 2));
        }
    }
}
