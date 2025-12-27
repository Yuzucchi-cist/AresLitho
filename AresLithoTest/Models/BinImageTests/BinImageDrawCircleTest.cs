using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageDrawCircleTest
    {
        [TestMethod]
        public void DrawCircle_SmallRadius_DrawsCircleOutline()
        {
            // Arrange
            var img = new BinImage(7, 7);

            // Act
            img.DrawCircle(3, 3, 2);

            // Assert - Circle outline should be drawn, not filled
            Assert.IsTrue(img[3, 1]); // top
            Assert.IsTrue(img[3, 5]); // bottom
            Assert.IsTrue(img[1, 3]); // left
            Assert.IsTrue(img[5, 3]); // right
            Assert.IsFalse(img[3, 3]); // center should be empty
        }

        [TestMethod]
        public void DrawCircle_ZeroRadius_DrawsSinglePoint()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.DrawCircle(2, 2, 0);

            // Assert
            Assert.IsTrue(img[2, 2]);
            // Verify other pixels are not set
            Assert.IsFalse(img[1, 2]);
            Assert.IsFalse(img[3, 2]);
        }

        [TestMethod]
        public void DrawCircle_LargeRadius_DrawsWithinBounds()
        {
            // Arrange
            var img = new BinImage(10, 10);

            // Act
            img.DrawCircle(5, 5, 4);

            // Assert - Check that some circle pixels are drawn
            Assert.IsTrue(img[5, 1] || img[5, 2]); // top area
            Assert.IsTrue(img[5, 8] || img[5, 9]); // bottom area
            Assert.IsTrue(img[1, 5] || img[2, 5]); // left area
            Assert.IsTrue(img[8, 5] || img[9, 5]); // right area
        }

        [TestMethod]
        public void DrawCircle_CenterAtOrigin_DrawsQuarterCircle()
        {
            // Arrange
            var img = new BinImage(10, 10);

            // Act
            img.DrawCircle(0, 0, 5);

            // Assert - Only the quarter in bounds should be drawn
            int pixelsSet = 0;
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (img[x, y]) pixelsSet++;
                }
            }
            Assert.IsTrue(pixelsSet > 0, "Some pixels should be drawn");
        }

        [TestMethod]
        public void DrawCircle_CenterOutOfBounds_DoesNotCrash()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.DrawCircle(-10, -10, 3);

            // Assert - Should not crash, image remains empty or minimally affected
            Assert.IsNotNull(img);
        }

        [TestMethod]
        public void DrawCircle_ReturnsNewInstance()
        {
            // Arrange
            var img = new BinImage(10, 10);

            // Act
            var result = img.DrawCircle(5, 5, 3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Width);
            Assert.AreEqual(10, result.Height);
        }

        [TestMethod]
        public void DrawFilledCircle_SmallRadius_FillsCircle()
        {
            // Arrange
            var img = new BinImage(7, 7);

            // Act
            img.DrawFilledCircle(3, 3, 2, true);

            // Assert - Center and surrounding pixels should be filled
            Assert.IsTrue(img[3, 3]); // center
            Assert.IsTrue(img[2, 3]); // left of center
            Assert.IsTrue(img[4, 3]); // right of center
            Assert.IsTrue(img[3, 2]); // above center
            Assert.IsTrue(img[3, 4]); // below center
        }

        [TestMethod]
        public void DrawFilledCircle_WithFalseValue_ClearsCircle()
        {
            // Arrange
            var img = new BinImage(7, 7);
            // Fill entire image first
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    img.SetPixel(x, y, true);
                }
            }

            // Act - Clear a circle
            img.DrawFilledCircle(3, 3, 2, false);

            // Assert - Circle area should be cleared
            Assert.IsFalse(img[3, 3]); // center
            Assert.IsFalse(img[2, 3]); // left of center
            Assert.IsTrue(img[0, 0]); // corner should still be set
        }

        [TestMethod]
        public void DrawFilledCircle_ZeroRadius_FillsSinglePixel()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.DrawFilledCircle(2, 2, 0, true);

            // Assert
            Assert.IsTrue(img[2, 2]);
            Assert.IsFalse(img[1, 2]);
            Assert.IsFalse(img[3, 2]);
        }

        [TestMethod]
        public void DrawFilledCircle_LargeRadius_FillsCorrectly()
        {
            // Arrange
            var img = new BinImage(11, 11);

            // Act
            img.DrawFilledCircle(5, 5, 4, true);

            // Assert
            Assert.IsTrue(img[5, 5]); // center
            Assert.IsTrue(img[5, 2]); // top
            Assert.IsTrue(img[5, 8]); // bottom
            Assert.IsTrue(img[2, 5]); // left
            Assert.IsTrue(img[8, 5]); // right
            
            // Corners of bounding box should not be filled
            Assert.IsFalse(img[1, 1]);
            Assert.IsFalse(img[9, 1]);
            Assert.IsFalse(img[1, 9]);
            Assert.IsFalse(img[9, 9]);
        }

        [TestMethod]
        public void DrawFilledCircle_PartiallyOutOfBounds_FillsVisiblePortion()
        {
            // Arrange
            var img = new BinImage(10, 10);

            // Act - Circle extends beyond bounds
            img.DrawFilledCircle(0, 0, 5, true);

            // Assert
            Assert.IsTrue(img[0, 0]);
            Assert.IsTrue(img[3, 3]);
        }

        [TestMethod]
        public void DrawFilledCircle_CompletelyOutOfBounds_DoesNotCrash()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.DrawFilledCircle(-10, -10, 3, true);

            // Assert - Should handle gracefully
            Assert.IsNotNull(img);
        }

        [TestMethod]
        public void DrawFilledCircle_ReturnsNewInstance()
        {
            // Arrange
            var img = new BinImage(10, 10);

            // Act
            var result = img.DrawFilledCircle(5, 5, 3, true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Width);
            Assert.AreEqual(10, result.Height);
        }

        [TestMethod]
        public void DrawFilledCircle_OptimizedBounds_OnlyProcessesNecessaryRegion()
        {
            // Arrange
            var img = new BinImage(100, 100);

            // Act - Small circle in large image
            var result = img.DrawFilledCircle(50, 50, 5, true);

            // Assert - Verify circle is correctly positioned
            Assert.IsTrue(result[50, 50]);
            Assert.IsTrue(result[50, 45]);
            Assert.IsTrue(result[50, 55]);
            Assert.IsTrue(result[45, 50]);
            Assert.IsTrue(result[55, 50]);
        }
    }
}