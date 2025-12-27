using AresLitho.Models;

namespace AresLithoTest.TestHelpers
{
    public static class BinImageAssert
    {
        public static void Matches(BinImage img, string[] pattern)
        {
            for (int y = 0; y < pattern.Length; y++)
                for (int x = 0; x < pattern[y].Length; x++)
                    Assert.AreEqual(
                        pattern[y][x] == '#',
                        img[x, y],
                        $"Mismatch at ({x},{y})");
        }

        /// <summary>
        /// バイト配列の完全一致を検証します。長さと各バイトの値を比較し、
        /// 不一致位置をメッセージで報告します。
        /// </summary>
        public static void BytesEqual(byte[] expected, byte[] actual)
        {
            Assert.IsNotNull(expected, "expected is null");
            Assert.IsNotNull(actual, "actual is null");
            Assert.HasCount(expected.Length, actual, "Byte array length differs");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], $"Byte differs at index {i}");
            }
        }

        /// <summary>
        /// BinImage.EncodeToBgr32 の出力が、指定したパターンに従っているかを検証します。
        /// pattern: '#' = 塗り（出力は 0x00）、'.' = 空（出力は 0xFF）
        /// 各ピクセルは 4 バイト（B,G,R,A）なので 4 バイトすべてを検証します。
        /// </summary>
        public static void MatchesBgr32(BinImage img, string[] pattern)
        {
            // 基本チェック
            Assert.IsNotNull(img);
            int width = img.Width;
            int height = img.Height;
            Assert.HasCount(height, pattern, "Pattern height differs from image height");

            for (int y = 0; y < pattern.Length; y++)
                Assert.AreEqual(width, pattern[y].Length, $"Pattern width differs at row {y}");

            byte[] bytes = img.EncodeToBgr32();
            // expected length = width * height * 4
            Assert.HasCount(width * height * 4, bytes, "Encoded BGR32 length mismatch");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int baseIndex = (y * width + x) * 4;
                    byte expectedByte = pattern[y][x] == '#' ? (byte)0x00 : (byte)0xFF;
                    // Compare all 4 channels for robustness
                    Assert.AreEqual(expectedByte, bytes[baseIndex + 0], $"B component mismatch at ({x},{y})");
                    Assert.AreEqual(expectedByte, bytes[baseIndex + 1], $"G component mismatch at ({x},{y})");
                    Assert.AreEqual(expectedByte, bytes[baseIndex + 2], $"R component mismatch at ({x},{y})");
                    Assert.AreEqual(expectedByte, bytes[baseIndex + 3], $"A component mismatch at ({x},{y})");
                }
            }
        }
    }
}
