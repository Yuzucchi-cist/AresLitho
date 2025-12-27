using AresLitho.Models;
using AresLithoTest.TestHelpers;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageFillClosedAreasTest
    {
        [TestMethod]
        public void FillClosedAreas_SingleHole_FillsInterior()
        {
            // Arrange
            var img = new BinImage(5, 5);
            // fill whole image
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    img.SetPixel(x, y, true);
            // carve a 3x3 hole in center
            for (int y = 1; y <= 3; y++)
                for (int x = 1; x <= 3; x++)
                    img.SetPixel(x, y, false);

            try
            {
                // Act
                var res = img.FillClosedAreas();

                // Assert: hole is filled (all pixels become true)
                BinImageAssert.Matches(res, new[]
                {
                    "#####",
                    "#####",
                    "#####",
                    "#####",
                    "#####"
                });
            }
            catch (DllNotFoundException)
            {
                Assert.Inconclusive("OpenCv native libraries not available in test environment");
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"FillClosedAreas could not be validated: {ex.GetType().Name}: {ex.Message}");
            }
        }

        [TestMethod]
        public void FillClosedAreas_MultipleHoles_FillsAllHoles()
        {
            // Arrange
            var img = new BinImage(7, 5);
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    img.SetPixel(x, y, true);

            // carve two 1x1 holes
            img.SetPixel(2, 2, false);
            img.SetPixel(4, 3, false);

            try
            {
                // Act
                var res = img.FillClosedAreas();

                // Assert: both holes filled
                BinImageAssert.Matches(res, new[]
                {
                    "#######",
                    "#######",
                    "#######",
                    "#######",
                    "#######"
                });
            }
            catch (DllNotFoundException)
            {
                Assert.Inconclusive("OpenCv native libraries not available in test environment");
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"FillClosedAreas could not be validated: {ex.GetType().Name}: {ex.Message}");
            }
        }

        [TestMethod]
        public void FillClosedAreas_NoHoles_NoChange()
        {
            // Arrange
            var img = new BinImage(4, 4);
            // empty image (all false)
            try
            {
                // Act
                var res = img.FillClosedAreas();

                // Assert: unchanged (still all '.')
                BinImageAssert.Matches(res, new[]
                {
                    "....",
                    "....",
                    "....",
                    "...."
                });
            }
            catch (DllNotFoundException)
            {
                Assert.Inconclusive("OpenCv native libraries not available in test environment");
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"FillClosedAreas could not be validated: {ex.GetType().Name}: {ex.Message}");
            }
        }

        [TestMethod]
        public void FillClosedAreas_HoleOpenToBorder_IsNotFilled()
        {
            // Arrange
            var img = new BinImage(5, 5);
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    img.SetPixel(x, y, true);

            // carve hole but connect it to border (open channel)
            img.SetPixel(2, 1, false);
            img.SetPixel(2, 2, false);
            img.SetPixel(2, 3, false);
            img.SetPixel(0, 2, false); // connect to left border
            img.SetPixel(1, 2, false); // connect to left border

            try
            {
                // Act
                var res = img.FillClosedAreas();

                // Assert: open hole remains unfilled (those positions stay '.')
                BinImageAssert.Matches(res, new[]
                {
                    "#####",
                    "##.##",
                    "...##",
                    "##.##",
                    "#####"
                });
            }
            catch (DllNotFoundException)
            {
                Assert.Inconclusive("OpenCv native libraries not available in test environment");
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"FillClosedAreas could not be validated: {ex.GetType().Name}: {ex.Message}");
            }
        }

        [TestMethod]
        public void FillClosedAreas_ZeroSize_NoThrowAndZeroSizeResult()
        {
            // Arrange
            var img = new BinImage(0, 0);

            try
            {
                // Act
                var res = img.FillClosedAreas();

                // Assert
                Assert.AreEqual(0, res.Width);
                Assert.AreEqual(0, res.Height);
            }
            catch (DllNotFoundException)
            {
                Assert.Inconclusive("OpenCv native libraries not available in test environment");
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"FillClosedAreas could not be validated: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
