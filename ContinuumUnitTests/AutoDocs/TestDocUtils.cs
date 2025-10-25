using Continuum93.Emulator.AutoDocs;

namespace AutoDocs
{

    public class TestDocUtils
    {
        [Fact]
        public void TestGetReadableBytesSize()
        {
            Assert.Equal("0 bytes", DocUtils.GetReadableBytesSize(0));
            Assert.Equal("1 byte", DocUtils.GetReadableBytesSize(1));
            Assert.Equal("2 bytes", DocUtils.GetReadableBytesSize(2));
        }

        [Fact]
        public void TestCountFormatArguments()
        {
            Assert.Equal(0, DocUtils.CountFormatArguments("No arguments"));
            Assert.Equal(1, DocUtils.CountFormatArguments("One {0} argument"));
            Assert.Equal(2, DocUtils.CountFormatArguments("Two {0} arguments {1}"));
            Assert.Equal(2, DocUtils.CountFormatArguments("Non consecutive {0} arguments {3}"));
        }
    }
}
