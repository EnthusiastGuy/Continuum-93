using Continuum93.Emulator;

namespace VideoTests
{
    public class TestVideoPaging
    {
        [Theory]
        [InlineData(1, 0xFE02C0, 0xFE05C0)]
        [InlineData(2, 0xFC0880, 0xFE05C0)]
        [InlineData(3, 0xFA0E40, 0xFE05C0)]
        [InlineData(4, 0xF81400, 0xFE05C0)]
        [InlineData(5, 0xF619C0, 0xFE05C0)]
        [InlineData(6, 0xF41F80, 0xFE05C0)]
        [InlineData(7, 0xF22540, 0xFE05C0)]
        [InlineData(8, 0xF02B00, 0xFE05C0)]
        public void TestVideoPagingDistribution(byte vramPages, uint expectedPaletteAddress, uint expectedVideoAddress)
        {
            using Computer computer = new();
            computer.GRAPHICS.SetVramPages(vramPages);

            for (byte i = 0; i < vramPages; i++)
            {
                uint actualPaletteAddress = computer.GRAPHICS.GetVideoPagePaletteAddress(i);
                uint actualVideoAddress = computer.GRAPHICS.GetVideoPageAddress(i);
                Assert.Equal(expectedPaletteAddress - 768 * i, actualPaletteAddress);
                Assert.Equal(expectedVideoAddress - 129600 * i, actualVideoAddress);
            }

        }
    }
}
