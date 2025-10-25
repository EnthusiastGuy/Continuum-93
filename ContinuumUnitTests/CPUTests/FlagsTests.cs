using Continuum93.Emulator;

namespace CPUTests
{
    public class FlagsTests
    {
        [Fact]
        public void TestFlagIndexNamesAssociation()
        {
            Assert.Equal(0, Flags.GetFlagIndexByName("NZ"));
            Assert.Equal(1, Flags.GetFlagIndexByName("NC"));
            Assert.Equal(2, Flags.GetFlagIndexByName("SP"));
            Assert.Equal(3, Flags.GetFlagIndexByName("NO"));
            Assert.Equal(4, Flags.GetFlagIndexByName("PE"));
            Assert.Equal(5, Flags.GetFlagIndexByName("NE"));
            Assert.Equal(6, Flags.GetFlagIndexByName("LTE"));
            Assert.Equal(7, Flags.GetFlagIndexByName("GTE"));
            Assert.Equal(8, Flags.GetFlagIndexByName("Z"));
            Assert.Equal(9, Flags.GetFlagIndexByName("C"));
            Assert.Equal(10, Flags.GetFlagIndexByName("SN"));
            Assert.Equal(11, Flags.GetFlagIndexByName("OV"));
            Assert.Equal(12, Flags.GetFlagIndexByName("PO"));
            Assert.Equal(13, Flags.GetFlagIndexByName("EQ"));
            Assert.Equal(14, Flags.GetFlagIndexByName("GT"));
            Assert.Equal(15, Flags.GetFlagIndexByName("LT"));

            Assert.Equal("NZ", Flags.GetFlagNameByIndex(0));
            Assert.Equal("NC", Flags.GetFlagNameByIndex(1));
            Assert.Equal("SP", Flags.GetFlagNameByIndex(2));
            Assert.Equal("NO", Flags.GetFlagNameByIndex(3));
            Assert.Equal("PE", Flags.GetFlagNameByIndex(4));
            Assert.Equal("NE", Flags.GetFlagNameByIndex(5));
            Assert.Equal("LTE", Flags.GetFlagNameByIndex(6));
            Assert.Equal("GTE", Flags.GetFlagNameByIndex(7));
            Assert.Equal("Z", Flags.GetFlagNameByIndex(8));
            Assert.Equal("C", Flags.GetFlagNameByIndex(9));
            Assert.Equal("SN", Flags.GetFlagNameByIndex(10));
            Assert.Equal("OV", Flags.GetFlagNameByIndex(11));
            Assert.Equal("PO", Flags.GetFlagNameByIndex(12));
            Assert.Equal("EQ", Flags.GetFlagNameByIndex(13));
            Assert.Equal("GT", Flags.GetFlagNameByIndex(14));
            Assert.Equal("LT", Flags.GetFlagNameByIndex(15));
        }

        [Fact]
        public void TestFlagValueAssignment()
        {
            Flags flags = new();

            Assert.False(flags.GetValueByName("Z"));
            Assert.True(flags.GetValueByName("NZ"));
            flags.SetValueByName("Z", true);
            Assert.True(flags.GetValueByName("Z"));
            Assert.False(flags.GetValueByName("NZ"));

            Assert.False(flags.GetValueByName("C"));
            Assert.True(flags.GetValueByName("NC"));
            flags.SetValueByName("C", true);
            Assert.True(flags.GetValueByName("C"));
            Assert.False(flags.GetValueByName("NC"));

            Assert.False(flags.GetValueByName("SN"));
            Assert.True(flags.GetValueByName("SP"));
            flags.SetValueByName("SN", true);
            Assert.True(flags.GetValueByName("SN"));
            Assert.False(flags.GetValueByName("SP"));

            Assert.False(flags.GetValueByName("OV"));
            Assert.True(flags.GetValueByName("NO"));
            flags.SetValueByName("OV", true);
            Assert.True(flags.GetValueByName("OV"));
            Assert.False(flags.GetValueByName("NO"));

            Assert.False(flags.GetValueByName("PO"));
            Assert.True(flags.GetValueByName("PE"));
            flags.SetValueByName("PO", true);
            Assert.True(flags.GetValueByName("PO"));
            Assert.False(flags.GetValueByName("PE"));

            Assert.False(flags.GetValueByName("EQ"));
            Assert.True(flags.GetValueByName("NE"));
            flags.SetValueByName("EQ", true);
            Assert.True(flags.GetValueByName("EQ"));
            Assert.False(flags.GetValueByName("NE"));

            Assert.False(flags.GetValueByName("GT"));
            Assert.True(flags.GetValueByName("LTE"));
            flags.SetValueByName("GT", true);
            Assert.True(flags.GetValueByName("GT"));
            Assert.False(flags.GetValueByName("LTE"));

            Assert.False(flags.GetValueByName("LT"));
            Assert.True(flags.GetValueByName("GTE"));
            flags.SetValueByName("LT", true);
            Assert.True(flags.GetValueByName("LT"));
            Assert.False(flags.GetValueByName("GTE"));
        }
    }
}
