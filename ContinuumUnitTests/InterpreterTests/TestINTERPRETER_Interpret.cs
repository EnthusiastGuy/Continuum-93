
using Continuum93.Emulator.Interpreter;

namespace InterpreterTests
{

    public class TestINTERPRETER_Interpret
    {
        [Fact]
        public void TestGetLabels()
        {
            Assert.Empty(Interpret.GetLabels("LD ABC, .label1"));
            Assert.Empty(Interpret.GetLabels("  #DB .label1, .label2, .label3       "));
            Assert.Empty(Interpret.GetLabels("      .      "));
            Assert.Empty(Interpret.GetLabels("      .   .label     "));
            Assert.Empty(Interpret.GetLabels("    #DB  \".\"      "));
            Assert.Single(Interpret.GetLabels(".label1"));
            Assert.Single(Interpret.GetLabels(" .label1"));
            Assert.Single(Interpret.GetLabels(" .label1 "));
            Assert.Single(Interpret.GetLabels("     .label1"));
            Assert.Single(Interpret.GetLabels("     .label1     "));
            Assert.Single(Interpret.GetLabels(".label1.label2"));
            Assert.Single(Interpret.GetLabels(" .label1.label2  "));
            Assert.Single(Interpret.GetLabels("     .label1.label2 "));
            Assert.Equal(2, Interpret.GetLabels(".label1 .label2").Count);
            Assert.Equal(2, Interpret.GetLabels("    .label1 .label2    ").Count);
            Assert.Equal(2, Interpret.GetLabels("    .label1            .label2    ").Count);
            Assert.Equal(2, Interpret.GetLabels("    .label1            .label2     LD ABC, .label1").Count);

            Assert.Equal(".label1", Interpret.GetLabels("     .label1")[0]);
            Assert.Equal(
                new string[] { ".label1", ".label2", ".label3", ".label" },
                Interpret.GetLabels("     .label1 .label2        .label3 .label 4   ")
            );

        }

        [Fact]
        public void TestGetMnemonicFromLine()
        {
            Assert.Equal("LD A, B", Interpret.GetMnemonicFromLine("LD A, B"));
            Assert.Equal("LD A, B", Interpret.GetMnemonicFromLine("        LD A, B           "));
            Assert.Equal("LD A, B", Interpret.GetMnemonicFromLine("        LD A, B        ; I got some comment here!!   "));
            Assert.Equal("LD A, B", Interpret.GetMnemonicFromLine("        LD A, B        ; I got some comment here;;;!!   "));
            Assert.Equal("LD A, B", Interpret.GetMnemonicFromLine(".again        LD A, B        ; I got some comment here;;;!!   "));
            Assert.Equal("LD A, B", Interpret.GetMnemonicFromLine("  .again  LD A, B     ; comment ; .test"));
            Assert.Equal("", Interpret.GetMnemonicFromLine("  .justALabel"));
            Assert.Equal("", Interpret.GetMnemonicFromLine("  .justALabel               ; ... and a comment"));
        }

        [Fact]
        public void TestGetOp()
        {
            Assert.Equal("ADD", Interpret.GetOp("ADD H, 2"));
            Assert.Equal("CALL", Interpret.GetOp("       Call ~setTime       "));
        }

        [Fact]
        public void TestGetArguments()
        {
            Assert.Equal("H, 2", Interpret.GetArguments("ADD H, 2"));
            Assert.Equal("~setTime", Interpret.GetArguments("       Call ~setTime       "));
        }
    }
}
