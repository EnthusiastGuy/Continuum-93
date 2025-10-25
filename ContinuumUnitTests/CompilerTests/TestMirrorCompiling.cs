namespace CompilerTests
{
    public class TestMirrorCompiling
    {
        /*
        [Fact]
        public void TestMirrorCompilingLD()
        {
            var assembler = new Assembler();
            using var computer = new Computer();
            assembler.Build("LD A, (.label1 + EFG)");
            var line1 = assembler.GetCompiledLine(0);
            assembler.Build("LD A, (EFG + .label1)");
            var line2 = assembler.GetCompiledLine(0);

            Assert.Equal("LD r,(rrr,nnn)", line1.GeneralForm);
            Assert.Equal("LD r,(rrr,nnn)", line2.GeneralForm);
        }*/
    }
}
