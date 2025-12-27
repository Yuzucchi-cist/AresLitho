using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageCompositeTest
    {
        [TestMethod]
        public void Composite_DefaultOffset_PlacesSourceAtOrigin()
        {
            // Arrange
            var baseImg = new BinImage(4, 4);
            var src = new BinImage(2, 2);
            src.SetPixel(0, 0, true);
            src.SetPixel(1, 1, true);

            // Act
            var result = baseImg.Composite(src); // default offset (0,0)

            // Assert: source placed at top-left
            BinImageAssert.Matches(result, new[]
            {
                "#...",
                ".#..",
                "....",
                "...."
            });
        }

        [TestMethod]
        public void Composite_WithPositiveOffset_PlacesSourceAtOffset()
        {
            // Arrange
            var baseImg = new BinImage(5, 5);
            var src = new BinImage(2, 2);
            src.SetPixel(0, 0, true);
            src.SetPixel(1, 1, true);

            // Act
            var result = baseImg.Composite(src, 2, 1);

            // Assert: source (0,0)->(2,1), (1,1)->(3,2)
            BinImageAssert.Matches(result, new[]
            {
                ".....",
                "..#..",
                "...#.",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void Composite_SourceLargerThanBase_ClipsToBase()
        {
            // Arrange
            var baseImg = new BinImage(3, 3);
            var src = new BinImage(5, 5);
            // set a src pixel that will map inside after offset
            src.SetPixel(3, 3, true);

            // Act: offset -1,-1 maps src(3,3) -> target (2,2)
            var result = baseImg.Composite(src, -1, -1);

            // Assert: only (2,2) of base becomes set
            BinImageAssert.Matches(result, new[]
            {
                "...",
                "...",
                "..#"
            });
        }

        [TestMethod]
        public void Composite_Overlap_OverwritesBasePixels()
        {
            // Arrange
            var baseImg = new BinImage(3, 3);
            // base center true
            baseImg.SetPixel(1, 1, true);

            var src = new BinImage(3, 3);
            // src has top-left true only, center remains false and should overwrite base center to false
            src.SetPixel(0, 0, true);

            // Act
            var result = baseImg.Composite(src, 0, 0);

            // Assert: (0,0) from src true; (1,1) overwritten to false
            BinImageAssert.Matches(result, new[]
            {
                "#..",
                "...",
                "..."
            });
        }

        [TestMethod]
        public void Composite_NegativeOffset_PartialMapping()
        {
            // Arrange
            var baseImg = new BinImage(4, 3);
            var src = new BinImage(2, 2);
            src.SetPixel(0, 0, true);
            src.SetPixel(1, 1, true);

            // Act: offset (-1,1) gives only src(1,1) -> target (0,2)
            var result = baseImg.Composite(src, -1, 1);

            // Assert: only (0,2) set
            BinImageAssert.Matches(result, new[]
            {
                "....",
                "....",
                "#..."
            });
        }

        [TestMethod]
        public void Composite_ZeroSizeSource_NoChange()
        {
            // Arrange
            var baseImg = new BinImage(2, 2);
            baseImg.SetPixel(0, 0, true);
            baseImg.SetPixel(1, 1, true);

            var src = new BinImage(0, 0);

            // Act
            var result = baseImg.Composite(src, 0, 0);

            // Assert: unchanged
            BinImageAssert.Matches(result, new[]
            {
                "#.",
                ".#"
            });
        }
    }
}