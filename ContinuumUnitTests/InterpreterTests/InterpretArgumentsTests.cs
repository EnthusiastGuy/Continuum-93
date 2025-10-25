using Continuum93.Emulator.Interpreter;

namespace InterpreterTests
{

    public class InterpretArgumentsTests
    {
        [Fact]
        public void TestIsNotFloatNumber()
        {
            Assert.True(InterpretArgument.IsNotFloatNumber(".test"));
            Assert.True(InterpretArgument.IsNotFloatNumber(".23test"));
            Assert.True(InterpretArgument.IsNotFloatNumber(".23AAF"));
            Assert.True(InterpretArgument.IsNotFloatNumber("0.23AAF"));
            Assert.True(InterpretArgument.IsNotFloatNumber("0.2F"));
            Assert.True(InterpretArgument.IsNotFloatNumber("0.2f"));

            Assert.False(InterpretArgument.IsNotFloatNumber("0.2"));
        }
    }
}
