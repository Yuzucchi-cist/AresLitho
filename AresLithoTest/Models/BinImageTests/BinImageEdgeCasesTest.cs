using AresLitho.Models;

namespace AresLithoTest.Models.BinImageTests
{
    [TestClass]
    public class BinImageEdgeCasesTest
    {
        [TestMethod]
        public void Constructor_MaxIntDimensions_ThrowsOutOfMemoryException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<OutOfMemoryException>(() =>
            {
                var img = new BinImage(int.MaxValue, int.MaxValue);
            });
        }

        [TestMethod]
        public void Constructor_FromByteArray_NullArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                var img = new BinImage(null, 3, 3);
            });
        }

        [TestMethod]
        public void Constructor_FromByteArray_MismatchedDimensions_UsesAvailableData()
        {
            // Arrange
            byte[] bytes = new byte[] { 1, 0, 1, 0 }; // 4 bytes
            int width = 3;
            int height = 2; // expects 6 bytes

            // Act & Assert - Should handle gracefully or throw
            try
            {
                var img = new BinImage(bytes, width, height);
                // If it doesn't throw, verify it handles the mismatch
                Assert.IsTrue(img[0, 0]);
                Assert.IsFalse(img[1, 0]);
            }
            catch (IndexOutOfRangeException)
            {
                // Expected behavior for insufficient data
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Constructor_FromBoolArray_NullArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                var img = new BinImage((bool[,])null);
            });
        }

        [TestMethod]
        public void GetPixel_WithSetPixel_RoundTripCorrectly()
        {
            // Arrange
            var img = new BinImage(10, 10);
            var testCases = new[]
            {
                (0, 0, true),
                (9, 9, true),
                (5, 5, false),
                (0, 9, true),
                (9, 0, false)
            };

            // Act & Assert
            foreach (var (x, y, value) in testCases)
            {
                img.SetPixel(x, y, value);
                Assert.AreEqual(value, img.GetPixel(x, y),
                    $"Round trip failed at ({x},{y})");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetPixel_OutOfBounds_ThrowsException()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            var _ = img.GetPixel(10, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void SetPixel_OutOfBounds_ThrowsException()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act
            img.SetPixel(10, 10, true);
        }

        [TestMethod]
        public void Width_AfterConstruction_MatchesParameter()
        {
            // Arrange & Act
            var img = new BinImage(17, 23);

            // Assert
            Assert.AreEqual(17, img.Width);
        }

        [TestMethod]
        public void Height_AfterConstruction_MatchesParameter()
        {
            // Arrange & Act
            var img = new BinImage(17, 23);

            // Assert
            Assert.AreEqual(23, img.Height);
        }

        [TestMethod]
        public void Invert_InvalidParameters_HandlesGracefully()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(1, 1, true);

            // Act - Try various combinations
            var r1 = img.Invert(true, true);
            var r2 = img.Invert(false, false);

            // Assert - Should not crash
            Assert.IsNotNull(r1);
            Assert.IsNotNull(r2);
        }

        [TestMethod]
        public void Resize_NegativeWidth_ThrowsException()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act & Assert
            Assert.ThrowsException<OverflowException>(() =>
            {
                var result = img.Resize(-1, 5);
            });
        }

        [TestMethod]
        public void Resize_NegativeHeight_ThrowsException()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act & Assert
            Assert.ThrowsException<OverflowException>(() =>
            {
                var result = img.Resize(5, -1);
            });
        }

        [TestMethod]
        public void Scale_NegativeWidth_ThrowsException()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act & Assert
            Assert.ThrowsException<OverflowException>(() =>
            {
                var result = img.Scale(-1, 5);
            });
        }

        [TestMethod]
        public void Scale_NegativeHeight_ThrowsException()
        {
            // Arrange
            var img = new BinImage(5, 5);

            // Act & Assert
            Assert.ThrowsException<OverflowException>(() =>
            {
                var result = img.Scale(5, -1);
            });
        }

        [TestMethod]
        public void EncodeToBgr32_ConsecutiveCalls_ReturnConsistentResults()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(1, 1, true);

            // Act
            var bytes1 = img.EncodeToBgr32();
            var bytes2 = img.EncodeToBgr32();

            // Assert
            Assert.AreEqual(bytes1.Length, bytes2.Length);
            for (int i = 0; i < bytes1.Length; i++)
            {
                Assert.AreEqual(bytes1[i], bytes2[i], $"Byte {i} differs");
            }
        }

        [TestMethod]
        public void EncodeToBinary_ConsecutiveCalls_ReturnConsistentResults()
        {
            // Arrange
            var img = new BinImage(3, 3);
            img.SetPixel(1, 1, true);

            // Act
            var bytes1 = img.EncodeToBinary();
            var bytes2 = img.EncodeToBinary();

            // Assert
            Assert.AreEqual(bytes1.Length, bytes2.Length);
            for (int i = 0; i < bytes1.Length; i++)
            {
                Assert.AreEqual(bytes1[i], bytes2[i], $"Byte {i} differs");
            }
        }

        [TestMethod]
        public void DrawLine_VeryLongLine_CompletesSuccessfully()
        {
            // Arrange
            var img = new BinImage(1000, 1000);

            // Act
            var result = img.DrawLine(0, 0, 999, 999);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result[0, 0]);
            Assert.IsTrue(result[999, 999]);
        }

        [TestMethod]
        public void Composite_BothImagesEmpty_RemainsEmpty()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(3, 3);

            // Act
            var result = base_img.Composite(source);

            // Assert
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Assert.IsFalse(result[x, y]);
                }
            }
        }

        [TestMethod]
        public void Composite_ZeroSizeSource_DoesNotCrash()
        {
            // Arrange
            var base_img = new BinImage(5, 5);
            var source = new BinImage(0, 0);

            // Act
            var result = base_img.Composite(source);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Composite_ZeroSizeBase_WithNormalSource_HandlesSafely()
        {
            // Arrange
            var base_img = new BinImage(0, 0);
            var source = new BinImage(3, 3);
            source.SetPixel(1, 1, true);

            // Act
            var result = base_img.Composite(source);

            // Assert
            Assert.AreEqual(0, result.Width);
            Assert.AreEqual(0, result.Height);
        }
    }
}