using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageSetToCenterTest
    {
        [TestMethod]
        public void SetToCenter_OddBase_OddSource_PlacesSourceAtCenter()
        {
            // Arrange
            var baseImg = new BinImage(7, 7);
            var src = new BinImage(3, 3);
            src.SetPixel(1, 1, true); // source center

            // Act
            var result = baseImg.SetToCenter(src);

            // Assert: source center should map to base center (3,3)
            BinImageAssert.Matches(result, new[]
            {
                ".......",
                ".......",
                ".......",
                "...#...",
                ".......",
                ".......",
                "......."
            });
        }

        [TestMethod]
        public void SetToCenter_EvenBase_OddSource_PlacementIsFloorCentered()
        {
            // Arrange
            var baseImg = new BinImage(6, 6);
            var src = new BinImage(3, 3);
            src.SetPixel(1, 1, true); // source center

            // Act
            var result = baseImg.SetToCenter(src);

            // Assert: (Width-source.Width)/2 => 1, so center maps to (1+1,1+1) == (2,2)
            BinImageAssert.Matches(result, new[]
            {
                "......",
                "......",
                "..#...",
                "......",
                "......",
                "......"
            });
        }

        [TestMethod]
        public void SetToCenter_SameSizeSource_ReplacesEntireImage()
        {
            // Arrange
            var baseImg = new BinImage(3, 3);
            // mark some base pixels to ensure overwrite occurs
            baseImg.SetPixel(0, 0, true);
            baseImg.SetPixel(2, 2, true);

            var src = new BinImage(3, 3);
            src.SetPixel(0, 0, true);
            src.SetPixel(1, 1, true);
            src.SetPixel(2, 2, true);

            // Act
            var result = baseImg.SetToCenter(src);

            // Assert: entire image equals source pattern
            BinImageAssert.Matches(result, new[]
            {
                "#..",
                ".#.",
                "..#"
            });
        }

        [TestMethod]
        public void SetToCenter_LargerSource_ThrowsIndexOutOfRangeException()
        {
            // Arrange
            var baseImg = new BinImage(3, 3);
            var src = new BinImage(5, 5);
            src.SetPixel(2, 2, true);

            // Act & Assert: implementation writes with negative start index -> IndexOutOfRangeException expected
            Assert.Throws<IndexOutOfRangeException>(() => baseImg.SetToCenter(src));
        }

        [TestMethod]
        public void SetToCenter_ZeroSizeSource_NoChange()
        {
            // Arrange
            var baseImg = new BinImage(3, 3);
            baseImg.SetPixel(1, 1, true); // preserve this
            var src = new BinImage(0, 0);

            // Act
            var result = baseImg.SetToCenter(src);

            // Assert: unchanged
            BinImageAssert.Matches(result, new[]
            {
                "...",
                ".#.",
                "..."
            });
        }
    }
}
