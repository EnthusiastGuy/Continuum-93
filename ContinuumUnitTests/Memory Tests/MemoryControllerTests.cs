using Continuum93.Emulator;

namespace Memory_Tests
{

    public class MemoryControllerTests
    {
        [Fact]
        public void TestFetch()
        {
            using Computer computer = new();


            computer.LoadMem(new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x10, 0xFF, 0xBA, 0xAF, 0xFE, 0xBD });
            Assert.Equal(0, (double)computer.CPU.REGS.IPO);
            Assert.Equal(0xABCD, computer.MEMC.Fetch16());
            Assert.Equal(0xEF0110, (double)computer.MEMC.Fetch24());
            Assert.Equal(0xFF, computer.MEMC.Fetch());
            Assert.Equal(0xBAAFFEBD, (double)computer.MEMC.Fetch32());
            Assert.Equal(10, (double)computer.CPU.REGS.IPO);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestGetSigned16bitFromRAM()
        {
            using Computer computer = new();


            computer.LoadMem(new byte[] { 0xFF, 0xFF });
            Assert.Equal(-1, computer.MEMC.GetSigned16bitFromRAM(0));

            computer.LoadMem(new byte[] { 0x80, 0x00 });
            Assert.Equal(-32768, computer.MEMC.GetSigned16bitFromRAM(0));

            computer.LoadMem(new byte[] { 0x7F, 0xFF });
            Assert.Equal(32767, computer.MEMC.GetSigned16bitFromRAM(0));

            computer.LoadMem(new byte[] { 0, 0 });
            Assert.Equal(0, computer.MEMC.GetSigned16bitFromRAM(0));
        }

        [Fact]
        public void TestGetSigned24bitFromRAM()
        {
            using Computer computer = new();


            computer.LoadMem(new byte[] { 0xFF, 0xFF, 0xFF });
            Assert.Equal(-1, computer.MEMC.GetSigned24bitFromRAM(0));

            computer.LoadMem(new byte[] { 0x80, 0x00, 0x00 });
            Assert.Equal(-8388608, computer.MEMC.GetSigned24bitFromRAM(0));

            computer.LoadMem(new byte[] { 0x7F, 0xFF, 0xFF });
            Assert.Equal(8388607, computer.MEMC.GetSigned24bitFromRAM(0));

            computer.LoadMem(new byte[] { 0xFE, 0x7D, 0x48 });
            Assert.Equal(-99000, computer.MEMC.GetSigned24bitFromRAM(0));

            computer.LoadMem(new byte[] { 0, 0, 0 });
            Assert.Equal(0, computer.MEMC.GetSigned24bitFromRAM(0));
        }

        [Fact]
        public void TestFetch24Signed()
        {
            using Computer computer = new();


            computer.LoadMem(new byte[] { 0xFF, 0xFF, 0xFF });
            Assert.Equal(-1, computer.MEMC.Fetch24Signed());


            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestGetFromRAM()
        {
            using Computer computer = new();


            computer.LoadMem(new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x10, 0xFF, 0xBA, 0xAF, 0xFE, 0xBD });

            Assert.Equal(0xAB, computer.MEMC.Get8bitFromRAM(0));
            Assert.Equal(0xBD, computer.MEMC.Get8bitFromRAM(9));

            Assert.Equal(0xABCD, computer.MEMC.Get16bitFromRAM(0));
            Assert.Equal(0xFEBD, computer.MEMC.Get16bitFromRAM(8));

            Assert.Equal(0xABCDEF, (long)computer.MEMC.Get24bitFromRAM(0));
            Assert.Equal(0xAFFEBD, (long)computer.MEMC.Get24bitFromRAM(7));

            Assert.Equal(0xABCDEF01, (long)computer.MEMC.Get32bitFromRAM(0));
            Assert.Equal(0xBAAFFEBD, (long)computer.MEMC.Get32bitFromRAM(6));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestGetFromRAMWrapAround()
        {
            using Computer computer = new();


            computer.LoadMemAt(0xFFFFFE, new byte[] { 0xAB, 0xCD });
            computer.LoadMemAt(0, new byte[] { 0xEF, 0x01 });

            Assert.Equal(0xCD00, computer.MEMC.Get16bitFromRAM(0xFFFFFF));

            /*Assert.Equal(0xAB, computer.MEMC.Get8bitFromRAM(0));
            Assert.Equal(0xBD, computer.MEMC.Get8bitFromRAM(9));

            Assert.Equal(0xABCD, computer.MEMC.Get16bitFromRAM(0));
            Assert.Equal(0xFEBD, computer.MEMC.Get16bitFromRAM(8));

            Assert.Equal(0xABCDEF, (long)computer.MEMC.Get24bitFromRAM(0));
            Assert.Equal(0xAFFEBD, (long)computer.MEMC.Get24bitFromRAM(7));

            Assert.Equal(0xABCDEF01, (long)computer.MEMC.Get32bitFromRAM(0));
            Assert.Equal(0xBAAFFEBD, (long)computer.MEMC.Get32bitFromRAM(6));*/

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestSetToRAM()
        {
            using Computer computer = new();


            computer.MEMC.Set8bitToRAM(10000, 0xAB);
            computer.MEMC.Set16bitToRAM(11000, 0xABCD);
            computer.MEMC.Set24bitToRAM(12000, 0xABCDEF);
            computer.MEMC.Set32bitToRAM(13000, 0xABCDEF01);

            Assert.Equal(0xAB, computer.MEMC.Get8bitFromRAM(10000));
            Assert.Equal(0xABCD, computer.MEMC.Get16bitFromRAM(11000));
            Assert.Equal(0xABCDEF, (long)computer.MEMC.Get24bitFromRAM(12000));
            Assert.Equal(0xABCDEF01, (long)computer.MEMC.Get32bitFromRAM(13000));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestGetStringAt()
        {
            using Computer computer = new();


            computer.LoadMem(new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64, 0x21, 0 });
            string actual = computer.MEMC.GetStringAt(0);

            Assert.Equal("Hello world!", actual);

            computer.LoadMem(new byte[] { 0 });
            actual = computer.MEMC.GetStringAt(0);

            Assert.Equal("", actual);

            computer.LoadMemAt(100000, new byte[] { 0x53, 0x70, 0x65, 0x63, 0x69, 0x66, 0x69, 0x63, 0x20, 0x61, 0x64, 0x64, 0x72, 0x65, 0x73, 0x73, 0 });
            actual = computer.MEMC.GetStringAt(100000);

            Assert.Equal("Specific address", actual);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestClearAllRAM()
        {
            using Computer computer = new();


            for (int i = 0; i < computer.MEMC.RAM.Data.Length; i++)
                computer.MEMC.RAM.Data[i] = 255;

            computer.MEMC.ClearAllRAM();

            for (int i = 0; i < computer.MEMC.RAM.Data.Length; i++)
            {
                if (computer.MEMC.RAM.Data[i] != 0)
                {
                    Assert.True(false, string.Format("Memory cell at #{0} not cleared", i));
                }
            }

            TUtils.IncrementCountedTests("exec");
        }
    }
}
