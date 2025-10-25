using Continuum93.Emulator;

namespace Tools
{
    public class MnemonicExports
    {
        [Fact]
        public void ExportSubOps()
        {
            string subOps = MnemonicTools.ExportSubOps();
            Assert.True(subOps.Length > 0);
        }
    }
}
