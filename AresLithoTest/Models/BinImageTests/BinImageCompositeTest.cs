using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageCompositeTest
    {
        [TestMethod]
        public void Composite_NoOffset_OverlaysAtOrigin()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(3, 3);
            source.SetPixel(1, 1, true);

            // Act
            var result = base_img.Composite(source);

            // Assert
            BinImageAssert.Matches(result, new[]
            {
                ".....",
                ".#...",
                ".....",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void Composite_WithOffset_OverlaysAtPosition()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(2, 2);
            source.SetPixel(0, 0, true);
            source.SetPixel(1, 1, true);

            // Act
            var result = base_img.Composite(source, 2, 2);

            // Assert
            BinImageAssert.Matches(result, new[]
            {
                ".....",
                ".....",
                "..#..",
                "...#.",
                "....."
            });
        }

        [TestMethod]
        public void Composite_SourceLargerThanBase_ClipsToBaseBounds()
        {
            // Arrange
            var base_img = new BinImage(3, 3);
            var source = new BinImage(5, 5);
            source.SetPixel(0, 0, true);
            source.SetPixel(1, 1, true);
            source.SetPixel(2, 2, true);

            // Act
            var result = base_img.Composite(source);

            // Assert - Only visible portion should be composited
            BinImageAssert.Matches(result, new[]
            {
                "#..",
                ".#.",
                "..#"
            });
        }

        [TestMethod]
        public void Composite_PartialOverlap_OnlyOverlapsVisiblePortion()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(3, 3);
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    source.SetPixel(x, y, true);
                }
            }

            // Act - Offset so source extends beyond base bounds
            var result = base_img.Composite(source, 3, 3);

            // Assert
            BinImageAssert.Matches(result, new[]
            {
                ".....",
                ".....",
                ".....",
                "...##",
                "...##"
            });
        }

        [TestMethod]
        public void Composite_NegativeOffset_ClipsCorrectly()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(3, 3);
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    source.SetPixel(x, y, true);
                }
            }

            // Act
            var result = base_img.Composite(source, -1, -1);

            // Assert - Only the portion of source within bounds should be visible
            BinImageAssert.Matches(result, new[]
            {
                "##...",
                "##...",
                ".....",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void Composite_CompletelyOutOfBounds_NoChange()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(2, 2);
            source.SetPixel(0, 0, true);
            source.SetPixel(1, 1, true);

            // Act
            var result = base_img.Composite(source, 10, 10);

            // Assert - Base should remain unchanged
            BinImageAssert.Matches(result, new[]
            {
                ".....",
                ".....",
                ".....",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void Composite_OverwritesExistingPixels()
        {
            // Arrange
            var base_img = new BinImage(3, 3);
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    base_img.SetPixel(x, y, true);
                }
            }
            var source = new BinImage(2, 2);
            // Source has false pixels

            // Act
            var result = base_img.Composite(source, 0, 0);

            // Assert - Source's false pixels should overwrite base's true pixels
            BinImageAssert.Matches(result, new[]
            {
                "..",
                "..#",
                "###"
            });
        }

        [TestMethod]
        public void Composite_ReturnsNewInstance()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(2, 2);

            // Act
            var result = base_img.Composite(source);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotSame(base_img, result);
            Assert.AreEqual(base_img.Width, result.Width);
            Assert.AreEqual(base_img.Height, result.Height);
        }

        [TestMethod]
        public void SetToCenter_SmallSource_CentersCorrectly()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(3, 3);
            source.SetPixel(1, 1, true);

            // Act
            var result = base_img.SetToCenter(source);

            // Assert - Source should be centered: offset = ((5-3)/2, (5-3)/2) = (1,1)
            BinImageAssert.Matches(result, new[]
            {
                ".....",
                ".....",
                "..#..",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void SetToCenter_SinglePixelSource_CentersAtMiddle()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(1, 1);
            source.SetPixel(0, 0, true);

            // Act
            var result = base_img.SetToCenter(source);

            // Assert - Should be at (2,2)
            BinImageAssert.Matches(result, new[]
            {
                ".....",
                ".....",
                "..#..",
                ".....",
                "....."
            });
        }

        [TestMethod]
        public void SetToCenter_SourceSameAsBase_OverlaysCompletely()
        {
            // Arrange
            var base_img = new BinImage(3, 3);
            var source = new BinImage(3, 3);
            source.SetPixel(0, 0, true);
            source.SetPixel(2, 2, true);

            // Act
            var result = base_img.SetToCenter(source);

            // Assert
            BinImageAssert.Matches(result, new[]
            {
                "#..",
                "...",
                "..#"
            });
        }

        [TestMethod]
        public void SetToCenter_SourceLargerThanBase_ClipsToBase()
        {
            // Arrange
            var base_img = new BinImage(3, 3);
            var source = new BinImage(5, 5);
            source.SetPixel(2, 2, true);

            // Act
            var result = base_img.SetToCenter(source);

            // Assert - Offset would be negative, so clips appropriately
            Assert.IsTrue(result[1, 1]); // Source's (2,2) mapped to base's center
        }

        [TestMethod]
        public void SetToCenter_OddAndEvenDimensions_HandlesRounding()
        {
            // Arrange
            var base_img = new BinImage(6, 6);
            var source = new BinImage(3, 3);
            source.SetPixel(1, 1, true);

            // Act
            var result = base_img.SetToCenter(source);

            // Assert - Offset = ((6-3)/2, (6-3)/2) = (1,1) with integer division
            Assert.IsTrue(result[2, 2]); // Source (1,1) at base (1+1, 1+1)
        }

        [TestMethod]
        public void SetToCenter_ReturnsNewInstance()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(3, 3);

            // Act
            var result = base_img.SetToCenter(source);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotSame(base_img, result);
        }

        [TestMethod]
        public void SetToCenter_ZeroSizeSource_DoesNotCrash()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(0, 0);

            // Act
            var result = base_img.SetToCenter(source);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Width);
            Assert.AreEqual(5, result.Height);
        }
    }
}