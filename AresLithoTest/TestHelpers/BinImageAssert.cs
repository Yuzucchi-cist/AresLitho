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
                        img[x,y],
                        $"Mismatch at ({x},{y})");
        }
    }
}
