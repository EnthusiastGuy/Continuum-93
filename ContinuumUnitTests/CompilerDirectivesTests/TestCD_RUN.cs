using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CompilerDirectivesTests
{

    public class TestCD_RUN
    {
        [Fact]
        public void TestSingleRunDirective()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #RUN 15
                #ORG 10
                LD A, 1
                RET
            ");

            Assert.Equal(15, (int)cp.GetRunAddress());
            Assert.False(cp.IsRunAddressFromOrg());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestSingleRunDirectiveWithPriority()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #ORG 10
                #RUN 15
                LD A, 1
                RET
            ");

            Assert.Equal(15, (int)cp.GetRunAddress());
            Assert.False(cp.IsRunAddressFromOrg());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestMultipleRunDirectives()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #RUN 100
                #ORG 10
                #RUN 25
                LD A, 1
                RET
            ");

            Assert.Equal(100, (int)cp.GetRunAddress());
            Assert.False(cp.IsRunAddressFromOrg());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestMultipleRunDirectivesWithPriority()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #ORG 10
                #RUN 100
                #RUN 25
                LD A, 1
                RET
            ");

            Assert.Equal(100, (int)cp.GetRunAddress());
            Assert.False(cp.IsRunAddressFromOrg());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestNoRunDirectiveFirstOrgFallback()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #ORG 10
                LD A, 1
                RET

                #ORG 20
                LD B, 2
                RET
            ");

            Assert.Equal(10, (int)cp.GetRunAddress());
            Assert.True(cp.IsRunAddressFromOrg());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestMultipleOrgMultipleRun()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #ORG 10
                LD A, 1
                RET

                #ORG 20
                LD B, 2
                RET

                #RUN 40
                #RUN 50 ; This should be ignored
            ");

            Assert.Equal(40, (int)cp.GetRunAddress());
            Assert.False(cp.IsRunAddressFromOrg());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestNoOrgNoRun()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 1
                RET
            ");

            Assert.Equal(0, (int)cp.GetRunAddress());
            Assert.False(cp.IsRunAddressFromOrg());
            TUtils.IncrementCountedTests("directives");
        }
    }
}
