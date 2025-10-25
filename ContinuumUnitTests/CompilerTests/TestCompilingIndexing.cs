using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CompilerTests
{
    public class TestCompilingIndexing
    {
        [Theory]
        [InlineData("LD A, (45 + 3)", "LD r,(nnn,nnn)")]
        [InlineData("LD A, (.label1 + 3)", "LD r,(nnn,nnn)")]
        [InlineData("LD A, (45 + .label2)", "LD r,(nnn,nnn)")]
        [InlineData("LD A, (.label1 + .label2)", "LD r,(nnn,nnn)")]
        [InlineData("LD A, (45 + E)", "LD r,(nnn,r)")]
        [InlineData("LD A, (.label1 + E)", "LD r,(nnn,r)")]
        [InlineData("LD A, (45 + EF)", "LD r,(nnn,rr)")]
        [InlineData("LD A, (.label1 + EF)", "LD r,(nnn,rr)")]
        [InlineData("LD A, (45 + EFG)", "LD r,(nnn,rrr)")]
        [InlineData("LD A, (.label1 + EFG)", "LD r,(nnn,rrr)")]         // * duplicate 1
        [InlineData("LD A, (EFG + 3)", "LD r,(rrr,nnn)")]
        [InlineData("LD A, (EFG + .label1)", "LD r,(rrr,nnn)")]         // * duplicate 1
        [InlineData("LD A, (EFG + X)", "LD r,(rrr,r)")]
        [InlineData("LD A, (EFG + XY)", "LD r,(rrr,rr)")]
        [InlineData("LD A, (EFG + XYZ)", "LD r,(rrr,rrr)")]

        [InlineData("LD AB, (45 + 3)", "LD rr,(nnn,nnn)")]
        [InlineData("LD AB, (.label1 + 3)", "LD rr,(nnn,nnn)")]
        [InlineData("LD AB, (45 + .label2)", "LD rr,(nnn,nnn)")]
        [InlineData("LD AB, (.label1 + .label2)", "LD rr,(nnn,nnn)")]
        [InlineData("LD AB, (45 + E)", "LD rr,(nnn,r)")]
        [InlineData("LD AB, (.label1 + E)", "LD rr,(nnn,r)")]
        [InlineData("LD AB, (45 + EF)", "LD rr,(nnn,rr)")]
        [InlineData("LD AB, (.label1 + EF)", "LD rr,(nnn,rr)")]
        [InlineData("LD AB, (45 + EFG)", "LD rr,(nnn,rrr)")]
        [InlineData("LD AB, (.label1 + EFG)", "LD rr,(nnn,rrr)")]         // * duplicate 2
        [InlineData("LD AB, (EFG + 3)", "LD rr,(rrr,nnn)")]
        [InlineData("LD AB, (EFG + .label1)", "LD rr,(rrr,nnn)")]         // * duplicate 2
        [InlineData("LD AB, (EFG + X)", "LD rr,(rrr,r)")]
        [InlineData("LD AB, (EFG + XY)", "LD rr,(rrr,rr)")]
        [InlineData("LD AB, (EFG + XYZ)", "LD rr,(rrr,rrr)")]

        [InlineData("LD ABC, (45 + 3)", "LD rrr,(nnn,nnn)")]
        [InlineData("LD ABC, (.label1 + 3)", "LD rrr,(nnn,nnn)")]
        [InlineData("LD ABC, (45 + .label2)", "LD rrr,(nnn,nnn)")]
        [InlineData("LD ABC, (.label1 + .label2)", "LD rrr,(nnn,nnn)")]
        [InlineData("LD ABC, (45 + E)", "LD rrr,(nnn,r)")]
        [InlineData("LD ABC, (.label1 + E)", "LD rrr,(nnn,r)")]
        [InlineData("LD ABC, (45 + EF)", "LD rrr,(nnn,rr)")]
        [InlineData("LD ABC, (.label1 + EF)", "LD rrr,(nnn,rr)")]
        [InlineData("LD ABC, (45 + EFG)", "LD rrr,(nnn,rrr)")]
        [InlineData("LD ABC, (.label1 + EFG)", "LD rrr,(nnn,rrr)")]         // * duplicate 3
        [InlineData("LD ABC, (EFG + 3)", "LD rrr,(rrr,nnn)")]
        [InlineData("LD ABC, (EFG + .label1)", "LD rrr,(rrr,nnn)")]         // * duplicate 3
        [InlineData("LD ABC, (EFG + X)", "LD rrr,(rrr,r)")]
        [InlineData("LD ABC, (EFG + XY)", "LD rrr,(rrr,rr)")]
        [InlineData("LD ABC, (EFG + XYZ)", "LD rrr,(rrr,rrr)")]

        [InlineData("LD ABCD, (45 + 3)", "LD rrrr,(nnn,nnn)")]
        [InlineData("LD ABCD, (.label1 + 3)", "LD rrrr,(nnn,nnn)")]
        [InlineData("LD ABCD, (45 + .label2)", "LD rrrr,(nnn,nnn)")]
        [InlineData("LD ABCD, (.label1 + .label2)", "LD rrrr,(nnn,nnn)")]
        [InlineData("LD ABCD, (45 + E)", "LD rrrr,(nnn,r)")]
        [InlineData("LD ABCD, (.label1 + E)", "LD rrrr,(nnn,r)")]
        [InlineData("LD ABCD, (45 + EF)", "LD rrrr,(nnn,rr)")]
        [InlineData("LD ABCD, (.label1 + EF)", "LD rrrr,(nnn,rr)")]
        [InlineData("LD ABCD, (45 + EFG)", "LD rrrr,(nnn,rrr)")]
        [InlineData("LD ABCD, (.label1 + EFG)", "LD rrrr,(nnn,rrr)")]         // * duplicate 4
        [InlineData("LD ABCD, (EFG + 3)", "LD rrrr,(rrr,nnn)")]
        [InlineData("LD ABCD, (EFG + .label1)", "LD rrrr,(rrr,nnn)")]         // * duplicate 4
        [InlineData("LD ABCD, (EFG + X)", "LD rrrr,(rrr,r)")]
        [InlineData("LD ABCD, (EFG + XY)", "LD rrrr,(rrr,rr)")]
        [InlineData("LD ABCD, (EFG + XYZ)", "LD rrrr,(rrr,rrr)")]

        [InlineData("LD (0x80000), (45 + 3)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000), (.label1 + 3)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000), (45 + .label2)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000), (.label1 + .label2)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000), (45 + E)", "LD (nnn),(nnn,r)")]
        [InlineData("LD (0x80000), (.label1 + E)", "LD (nnn),(nnn,r)")]
        [InlineData("LD (0x80000), (45 + EF)", "LD (nnn),(nnn,rr)")]
        [InlineData("LD (0x80000), (.label1 + EF)", "LD (nnn),(nnn,rr)")]
        [InlineData("LD (0x80000), (45 + EFG)", "LD (nnn),(nnn,rrr)")]
        [InlineData("LD (0x80000), (.label1 + EFG)", "LD (nnn),(nnn,rrr)")]         // * duplicate 5
        [InlineData("LD (0x80000), (EFG + 3)", "LD (nnn),(rrr,nnn)")]
        [InlineData("LD (0x80000), (EFG + .label1)", "LD (nnn),(rrr,nnn)")]         // * duplicate 5
        [InlineData("LD (0x80000), (EFG + X)", "LD (nnn),(rrr,r)")]
        [InlineData("LD (0x80000), (EFG + XY)", "LD (nnn),(rrr,rr)")]
        [InlineData("LD (0x80000), (EFG + XYZ)", "LD (nnn),(rrr,rrr)")]

        [InlineData("LD (.address), (45 + 3)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (.address), (.label1 + 3)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (.address), (45 + .label2)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (.address), (.label1 + .label2)", "LD (nnn),(nnn,nnn)")]
        [InlineData("LD (.address), (45 + E)", "LD (nnn),(nnn,r)")]
        [InlineData("LD (.address), (.label1 + E)", "LD (nnn),(nnn,r)")]
        [InlineData("LD (.address), (45 + EF)", "LD (nnn),(nnn,rr)")]
        [InlineData("LD (.address), (.label1 + EF)", "LD (nnn),(nnn,rr)")]
        [InlineData("LD (.address), (45 + EFG)", "LD (nnn),(nnn,rrr)")]
        [InlineData("LD (.address), (.label1 + EFG)", "LD (nnn),(nnn,rrr)")]         // * duplicate 5
        [InlineData("LD (.address), (EFG + 3)", "LD (nnn),(rrr,nnn)")]
        [InlineData("LD (.address), (EFG + .label1)", "LD (nnn),(rrr,nnn)")]         // * duplicate 5
        [InlineData("LD (.address), (EFG + X)", "LD (nnn),(rrr,r)")]
        [InlineData("LD (.address), (EFG + XY)", "LD (nnn),(rrr,rr)")]
        [InlineData("LD (.address), (EFG + XYZ)", "LD (nnn),(rrr,rrr)")]
        //
        [InlineData("LD (0x80000 + 0x01), 45, 1", "LD (nnn,nnn),nnnn,n")]
        [InlineData("LD (.address + 0x01), 45, 1", "LD (nnn,nnn),nnnn,n")]
        [InlineData("LD (0x80000 + .address), 45, 1", "LD (nnn,nnn),nnnn,n")]
        [InlineData("LD (.address + .address), 45, 1", "LD (nnn,nnn),nnnn,n")]
        [InlineData("LD (0x80000 + 0x01), 45, 1, 300", "LD (nnn,nnn),nnnn,n,nnn")]
        [InlineData("LD (.address + 0x01), 45, 1, 300", "LD (nnn,nnn),nnnn,n,nnn")]
        [InlineData("LD (0x80000 + .address), 45, 1, 300", "LD (nnn,nnn),nnnn,n,nnn")]
        [InlineData("LD (.address + .address), 45, 1, 300", "LD (nnn,nnn),nnnn,n,nnn")]

        [InlineData("LD (0x80000 + 0x01), A, 1", "LD (nnn,nnn),r,nnn")]
        [InlineData("LD (.address + 0x01), A, 1", "LD (nnn,nnn),r,nnn")]
        [InlineData("LD (0x80000 + .address), A, 1", "LD (nnn,nnn),r,nnn")]
        [InlineData("LD (.address + .address), A, 1", "LD (nnn,nnn),r,nnn")]

        [InlineData("LD (0x80000 + 0x01), AB, 1", "LD (nnn,nnn),rr,nnn")]
        [InlineData("LD (.address + 0x01), AB, 1", "LD (nnn,nnn),rr,nnn")]
        [InlineData("LD (0x80000 + .address), AB, 1", "LD (nnn,nnn),rr,nnn")]
        [InlineData("LD (.address + .address), AB, 1", "LD (nnn,nnn),rr,nnn")]

        [InlineData("LD (0x80000 + 0x01), ABC, 1", "LD (nnn,nnn),rrr,nnn")]
        [InlineData("LD (.address + 0x01), ABC, 1", "LD (nnn,nnn),rrr,nnn")]
        [InlineData("LD (0x80000 + .address), ABC, 1", "LD (nnn,nnn),rrr,nnn")]
        [InlineData("LD (.address + .address), ABC, 1", "LD (nnn,nnn),rrr,nnn")]

        [InlineData("LD (0x80000 + 0x01), ABCD, 1", "LD (nnn,nnn),rrrr,nnn")]
        [InlineData("LD (.address + 0x01), ABCD, 1", "LD (nnn,nnn),rrrr,nnn")]
        [InlineData("LD (0x80000 + .address), ABCD, 1", "LD (nnn,nnn),rrrr,nnn")]
        [InlineData("LD (.address + .address), ABCD, 1", "LD (nnn,nnn),rrrr,nnn")]

        [InlineData("LD (0x80000 + 0x01), (0x70000)", "LD (nnn,nnn),(nnn)")]
        [InlineData("LD (0x80000 + 0x01), (.address)", "LD (nnn,nnn),(nnn)")]
        [InlineData("LD (.address + 0x01), (0x70000)", "LD (nnn,nnn),(nnn)")]
        [InlineData("LD (.address + 0x01), (.address)", "LD (nnn,nnn),(nnn)")]
        [InlineData("LD (0x80000 + .address), (0x70000)", "LD (nnn,nnn),(nnn)")]
        [InlineData("LD (0x80000 + .address), (.address)", "LD (nnn,nnn),(nnn)")]
        [InlineData("LD (.address + .address), (0x70000)", "LD (nnn,nnn),(nnn)")]
        [InlineData("LD (.address + .address), (.address)", "LD (nnn,nnn),(nnn)")]

        [InlineData("LD (0x80000 + 0x01), (0x70000 + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000 + 0x01), (.address + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + 0x01), (0x70000 + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + 0x01), (.address + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000 + .address), (0x70000 + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000 + .address), (.address + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + .address), (0x70000 + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + .address), (.address + 3000)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000 + 0x01), (0x70000 + .address)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000 + 0x01), (.address + .address)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + 0x01), (0x70000 + .address)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + 0x01), (.address + .address)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000 + .address), (0x70000 + .address)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (0x80000 + .address), (.address + .address)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + .address), (0x70000 + .address)", "LD (nnn,nnn),(nnn,nnn)")]
        [InlineData("LD (.address + .address), (.address + .address)", "LD (nnn,nnn),(nnn,nnn)")]

        [InlineData("LD (0x80000 + 0x01), (0x70000 + A)", "LD (nnn,nnn),(nnn,r)")]
        [InlineData("LD (0x80000 + 0x01), (.address + A)", "LD (nnn,nnn),(nnn,r)")]
        [InlineData("LD (.address + 0x01), (0x70000 + A)", "LD (nnn,nnn),(nnn,r)")]
        [InlineData("LD (.address + 0x01), (.address + A)", "LD (nnn,nnn),(nnn,r)")]
        [InlineData("LD (0x80000 + .address), (0x70000 + A)", "LD (nnn,nnn),(nnn,r)")]
        [InlineData("LD (0x80000 + .address), (.address + A)", "LD (nnn,nnn),(nnn,r)")]
        [InlineData("LD (.address + .address), (0x70000 + A)", "LD (nnn,nnn),(nnn,r)")]
        [InlineData("LD (.address + .address), (.address + A)", "LD (nnn,nnn),(nnn,r)")]

        [InlineData("LD (0x80000 + 0x01), (0x70000 + AB)", "LD (nnn,nnn),(nnn,rr)")]
        [InlineData("LD (0x80000 + 0x01), (.address + AB)", "LD (nnn,nnn),(nnn,rr)")]
        [InlineData("LD (.address + 0x01), (0x70000 + AB)", "LD (nnn,nnn),(nnn,rr)")]
        [InlineData("LD (.address + 0x01), (.address + AB)", "LD (nnn,nnn),(nnn,rr)")]
        [InlineData("LD (0x80000 + .address), (0x70000 + AB)", "LD (nnn,nnn),(nnn,rr)")]
        [InlineData("LD (0x80000 + .address), (.address + AB)", "LD (nnn,nnn),(nnn,rr)")]
        [InlineData("LD (.address + .address), (0x70000 + AB)", "LD (nnn,nnn),(nnn,rr)")]
        [InlineData("LD (.address + .address), (.address + AB)", "LD (nnn,nnn),(nnn,rr)")]

        [InlineData("LD (0x80000 + 0x01), (0x70000 + ABC)", "LD (nnn,nnn),(nnn,rrr)")]
        [InlineData("LD (0x80000 + 0x01), (.address + ABC)", "LD (nnn,nnn),(nnn,rrr)")]
        [InlineData("LD (.address + 0x01), (0x70000 + ABC)", "LD (nnn,nnn),(nnn,rrr)")]
        [InlineData("LD (.address + 0x01), (.address + ABC)", "LD (nnn,nnn),(nnn,rrr)")]
        [InlineData("LD (0x80000 + .address), (0x70000 + ABC)", "LD (nnn,nnn),(nnn,rrr)")]
        [InlineData("LD (0x80000 + .address), (.address + ABC)", "LD (nnn,nnn),(nnn,rrr)")]
        [InlineData("LD (.address + .address), (0x70000 + ABC)", "LD (nnn,nnn),(nnn,rrr)")]
        [InlineData("LD (.address + .address), (.address + ABC)", "LD (nnn,nnn),(nnn,rrr)")]

        [InlineData("LD (0x80000 + 0x01), (ABC)", "LD (nnn,nnn),(rrr)")]
        [InlineData("LD (.address + 0x01), (ABC)", "LD (nnn,nnn),(rrr)")]
        [InlineData("LD (0x80000 + .address), (ABC)", "LD (nnn,nnn),(rrr)")]
        [InlineData("LD (.address + .address), (ABC)", "LD (nnn,nnn),(rrr)")]

        [InlineData("LD (0x80000 + 0x01), (ABC + 0x70000)", "LD (nnn,nnn),(rrr,nnn)")]
        [InlineData("LD (0x80000 + 0x01), (ABC + .address)", "LD (nnn,nnn),(rrr,nnn)")]
        [InlineData("LD (.address + 0x01), (ABC + 0x70000)", "LD (nnn,nnn),(rrr,nnn)")]
        [InlineData("LD (.address + 0x01), (ABC + .address)", "LD (nnn,nnn),(rrr,nnn)")]
        [InlineData("LD (0x80000 + .address), (ABC + 0x70000)", "LD (nnn,nnn),(rrr,nnn)")]
        [InlineData("LD (0x80000 + .address), (ABC + .address)", "LD (nnn,nnn),(rrr,nnn)")]
        [InlineData("LD (.address + .address), (ABC + 0x70000)", "LD (nnn,nnn),(rrr,nnn)")]
        [InlineData("LD (.address + .address), (ABC + .address)", "LD (nnn,nnn),(rrr,nnn)")]

        [InlineData("LD (0x80000 + 0x01), (ABC + X)", "LD (nnn,nnn),(rrr,r)")]
        [InlineData("LD (.address + 0x01), (ABC + X)", "LD (nnn,nnn),(rrr,r)")]
        [InlineData("LD (0x80000 + .address), (ABC + X)", "LD (nnn,nnn),(rrr,r)")]
        [InlineData("LD (.address + .address), (ABC + X)", "LD (nnn,nnn),(rrr,r)")]

        [InlineData("LD (0x80000 + 0x01), (ABC + XY)", "LD (nnn,nnn),(rrr,rr)")]
        [InlineData("LD (.address + 0x01), (ABC + XY)", "LD (nnn,nnn),(rrr,rr)")]
        [InlineData("LD (0x80000 + .address), (ABC + XY)", "LD (nnn,nnn),(rrr,rr)")]
        [InlineData("LD (.address + .address), (ABC + XY)", "LD (nnn,nnn),(rrr,rr)")]

        [InlineData("LD (0x80000 + 0x01), (ABC + XYZ)", "LD (nnn,nnn),(rrr,rrr)")]
        [InlineData("LD (.address + 0x01), (ABC + XYZ)", "LD (nnn,nnn),(rrr,rrr)")]
        [InlineData("LD (0x80000 + .address), (ABC + XYZ)", "LD (nnn,nnn),(rrr,rrr)")]
        [InlineData("LD (.address + .address), (ABC + XYZ)", "LD (nnn,nnn),(rrr,rrr)")]

        [InlineData("LD (0x80000 + 0x01), F0", "LD (nnn,nnn),fr")]
        [InlineData("LD (.address + 0x01), F0", "LD (nnn,nnn),fr")]
        [InlineData("LD (0x80000 + .address), F0", "LD (nnn,nnn),fr")]
        [InlineData("LD (.address + .address), F0", "LD (nnn,nnn),fr")]
        //
        [InlineData("LD (0x80000 + X), 45, 1", "LD (nnn,r),nnnn,n")]
        [InlineData("LD (.address + X), 45, 1", "LD (nnn,r),nnnn,n")]
        [InlineData("LD (0x80000 + X), 45, 1, 300", "LD (nnn,r),nnnn,n,nnn")]
        [InlineData("LD (.address + X), 45, 1, 300", "LD (nnn,r),nnnn,n,nnn")]

        [InlineData("LD (0x80000 + X), A, 1", "LD (nnn,r),r,nnn")]
        [InlineData("LD (.address + X), A, 1", "LD (nnn,r),r,nnn")]

        [InlineData("LD (0x80000 + X), AB, 1", "LD (nnn,r),rr,nnn")]
        [InlineData("LD (.address + X), AB, 1", "LD (nnn,r),rr,nnn")]

        [InlineData("LD (0x80000 + X), ABC, 1", "LD (nnn,r),rrr,nnn")]
        [InlineData("LD (.address + X), ABC, 1", "LD (nnn,r),rrr,nnn")]

        [InlineData("LD (0x80000 + X), ABCD, 1", "LD (nnn,r),rrrr,nnn")]
        [InlineData("LD (.address + X), ABCD, 1", "LD (nnn,r),rrrr,nnn")]

        [InlineData("LD (0x80000 + X), (0x70000)", "LD (nnn,r),(nnn)")]
        [InlineData("LD (0x80000 + X), (.address)", "LD (nnn,r),(nnn)")]
        [InlineData("LD (.address + X), (0x70000)", "LD (nnn,r),(nnn)")]
        [InlineData("LD (.address + X), (.address)", "LD (nnn,r),(nnn)")]

        [InlineData("LD (0x80000 + X), (0x70000 + 3000)", "LD (nnn,r),(nnn,nnn)")]
        [InlineData("LD (0x80000 + X), (.address + 3000)", "LD (nnn,r),(nnn,nnn)")]
        [InlineData("LD (.address + X), (0x70000 + 3000)", "LD (nnn,r),(nnn,nnn)")]
        [InlineData("LD (.address + X), (.address + 3000)", "LD (nnn,r),(nnn,nnn)")]
        [InlineData("LD (0x80000 + X), (0x70000 + .address)", "LD (nnn,r),(nnn,nnn)")]
        [InlineData("LD (0x80000 + X), (.address + .address)", "LD (nnn,r),(nnn,nnn)")]
        [InlineData("LD (.address + X), (0x70000 + .address)", "LD (nnn,r),(nnn,nnn)")]
        [InlineData("LD (.address + X), (.address + .address)", "LD (nnn,r),(nnn,nnn)")]

        [InlineData("LD (0x80000 + X), (0x70000 + A)", "LD (nnn,r),(nnn,r)")]
        [InlineData("LD (0x80000 + X), (.address + A)", "LD (nnn,r),(nnn,r)")]
        [InlineData("LD (.address + X), (0x70000 + A)", "LD (nnn,r),(nnn,r)")]
        [InlineData("LD (.address + X), (.address + A)", "LD (nnn,r),(nnn,r)")]

        [InlineData("LD (0x80000 + X), (0x70000 + AB)", "LD (nnn,r),(nnn,rr)")]
        [InlineData("LD (0x80000 + X), (.address + AB)", "LD (nnn,r),(nnn,rr)")]
        [InlineData("LD (.address + X), (0x70000 + AB)", "LD (nnn,r),(nnn,rr)")]
        [InlineData("LD (.address + X), (.address + AB)", "LD (nnn,r),(nnn,rr)")]

        [InlineData("LD (0x80000 + X), (0x70000 + ABC)", "LD (nnn,r),(nnn,rrr)")]
        [InlineData("LD (0x80000 + X), (.address + ABC)", "LD (nnn,r),(nnn,rrr)")]
        [InlineData("LD (.address + X), (0x70000 + ABC)", "LD (nnn,r),(nnn,rrr)")]
        [InlineData("LD (.address + X), (.address + ABC)", "LD (nnn,r),(nnn,rrr)")]

        [InlineData("LD (0x80000 + X), (ABC)", "LD (nnn,r),(rrr)")]
        [InlineData("LD (.address + X), (ABC)", "LD (nnn,r),(rrr)")]

        [InlineData("LD (0x80000 + X), (ABC + 0x70000)", "LD (nnn,r),(rrr,nnn)")]
        [InlineData("LD (0x80000 + X), (ABC + .address)", "LD (nnn,r),(rrr,nnn)")]
        [InlineData("LD (.address + X), (ABC + 0x70000)", "LD (nnn,r),(rrr,nnn)")]
        [InlineData("LD (.address + X), (ABC + .address)", "LD (nnn,r),(rrr,nnn)")]

        [InlineData("LD (0x80000 + X), (ABC + X)", "LD (nnn,r),(rrr,r)")]
        [InlineData("LD (.address + X), (ABC + X)", "LD (nnn,r),(rrr,r)")]

        [InlineData("LD (0x80000 + X), (ABC + XY)", "LD (nnn,r),(rrr,rr)")]
        [InlineData("LD (.address + X), (ABC + XY)", "LD (nnn,r),(rrr,rr)")]

        [InlineData("LD (0x80000 + X), (ABC + XYZ)", "LD (nnn,r),(rrr,rrr)")]
        [InlineData("LD (.address + X), (ABC + XYZ)", "LD (nnn,r),(rrr,rrr)")]

        [InlineData("LD (0x80000 + X), F0", "LD (nnn,r),fr")]
        [InlineData("LD (.address + X), F0", "LD (nnn,r),fr")]
        //
        [InlineData("LD (0x80000 + XY), 45, 1", "LD (nnn,rr),nnnn,n")]
        [InlineData("LD (.address + XY), 45, 1", "LD (nnn,rr),nnnn,n")]
        [InlineData("LD (0x80000 + XY), 45, 1, 300", "LD (nnn,rr),nnnn,n,nnn")]
        [InlineData("LD (.address + XY), 45, 1, 300", "LD (nnn,rr),nnnn,n,nnn")]

        [InlineData("LD (0x80000 + XY), A, 1", "LD (nnn,rr),r,nnn")]
        [InlineData("LD (.address + XY), A, 1", "LD (nnn,rr),r,nnn")]

        [InlineData("LD (0x80000 + XY), AB, 1", "LD (nnn,rr),rr,nnn")]
        [InlineData("LD (.address + XY), AB, 1", "LD (nnn,rr),rr,nnn")]

        [InlineData("LD (0x80000 + XY), ABC, 1", "LD (nnn,rr),rrr,nnn")]
        [InlineData("LD (.address + XY), ABC, 1", "LD (nnn,rr),rrr,nnn")]

        [InlineData("LD (0x80000 + XY), ABCD, 1", "LD (nnn,rr),rrrr,nnn")]
        [InlineData("LD (.address + XY), ABCD, 1", "LD (nnn,rr),rrrr,nnn")]

        [InlineData("LD (0x80000 + XY), (0x70000)", "LD (nnn,rr),(nnn)")]
        [InlineData("LD (0x80000 + XY), (.address)", "LD (nnn,rr),(nnn)")]
        [InlineData("LD (.address + XY), (0x70000)", "LD (nnn,rr),(nnn)")]
        [InlineData("LD (.address + XY), (.address)", "LD (nnn,rr),(nnn)")]

        [InlineData("LD (0x80000 + XY), (0x70000 + 3000)", "LD (nnn,rr),(nnn,nnn)")]
        [InlineData("LD (0x80000 + XY), (.address + 3000)", "LD (nnn,rr),(nnn,nnn)")]
        [InlineData("LD (.address + XY), (0x70000 + 3000)", "LD (nnn,rr),(nnn,nnn)")]
        [InlineData("LD (.address + XY), (.address + 3000)", "LD (nnn,rr),(nnn,nnn)")]
        [InlineData("LD (0x80000 + XY), (0x70000 + .address)", "LD (nnn,rr),(nnn,nnn)")]
        [InlineData("LD (0x80000 + XY), (.address + .address)", "LD (nnn,rr),(nnn,nnn)")]
        [InlineData("LD (.address + XY), (0x70000 + .address)", "LD (nnn,rr),(nnn,nnn)")]
        [InlineData("LD (.address + XY), (.address + .address)", "LD (nnn,rr),(nnn,nnn)")]

        [InlineData("LD (0x80000 + XY), (0x70000 + A)", "LD (nnn,rr),(nnn,r)")]
        [InlineData("LD (0x80000 + XY), (.address + A)", "LD (nnn,rr),(nnn,r)")]
        [InlineData("LD (.address + XY), (0x70000 + A)", "LD (nnn,rr),(nnn,r)")]
        [InlineData("LD (.address + XY), (.address + A)", "LD (nnn,rr),(nnn,r)")]

        [InlineData("LD (0x80000 + XY), (0x70000 + AB)", "LD (nnn,rr),(nnn,rr)")]
        [InlineData("LD (0x80000 + XY), (.address + AB)", "LD (nnn,rr),(nnn,rr)")]
        [InlineData("LD (.address + XY), (0x70000 + AB)", "LD (nnn,rr),(nnn,rr)")]
        [InlineData("LD (.address + XY), (.address + AB)", "LD (nnn,rr),(nnn,rr)")]

        [InlineData("LD (0x80000 + XY), (0x70000 + ABC)", "LD (nnn,rr),(nnn,rrr)")]
        [InlineData("LD (0x80000 + XY), (.address + ABC)", "LD (nnn,rr),(nnn,rrr)")]
        [InlineData("LD (.address + XY), (0x70000 + ABC)", "LD (nnn,rr),(nnn,rrr)")]
        [InlineData("LD (.address + XY), (.address + ABC)", "LD (nnn,rr),(nnn,rrr)")]

        [InlineData("LD (0x80000 + XY), (ABC)", "LD (nnn,rr),(rrr)")]
        [InlineData("LD (.address + XY), (ABC)", "LD (nnn,rr),(rrr)")]

        [InlineData("LD (0x80000 + XY), (ABC + 0x70000)", "LD (nnn,rr),(rrr,nnn)")]
        [InlineData("LD (0x80000 + XY), (ABC + .address)", "LD (nnn,rr),(rrr,nnn)")]
        [InlineData("LD (.address + XY), (ABC + 0x70000)", "LD (nnn,rr),(rrr,nnn)")]
        [InlineData("LD (.address + XY), (ABC + .address)", "LD (nnn,rr),(rrr,nnn)")]

        [InlineData("LD (0x80000 + XY), (ABC + X)", "LD (nnn,rr),(rrr,r)")]
        [InlineData("LD (.address + XY), (ABC + X)", "LD (nnn,rr),(rrr,r)")]

        [InlineData("LD (0x80000 + XY), (ABC + XY)", "LD (nnn,rr),(rrr,rr)")]
        [InlineData("LD (.address + XY), (ABC + XY)", "LD (nnn,rr),(rrr,rr)")]

        [InlineData("LD (0x80000 + XY), (ABC + XYZ)", "LD (nnn,rr),(rrr,rrr)")]
        [InlineData("LD (.address + XY), (ABC + XYZ)", "LD (nnn,rr),(rrr,rrr)")]

        [InlineData("LD (0x80000 + XY), F0", "LD (nnn,rr),fr")]
        [InlineData("LD (.address + XY), F0", "LD (nnn,rr),fr")]
        //
        [InlineData("LD (0x80000 + XYZ), 45, 1", "LD (nnn,rrr),nnnn,n")]
        [InlineData("LD (.address + XYZ), 45, 1", "LD (nnn,rrr),nnnn,n")]
        [InlineData("LD (0x80000 + XYZ), 45, 1, 300", "LD (nnn,rrr),nnnn,n,nnn")]
        [InlineData("LD (.address + XYZ), 45, 1, 300", "LD (nnn,rrr),nnnn,n,nnn")]

        [InlineData("LD (0x80000 + XYZ), A, 1", "LD (nnn,rrr),r,nnn")]
        [InlineData("LD (.address + XYZ), A, 1", "LD (nnn,rrr),r,nnn")]

        [InlineData("LD (0x80000 + XYZ), AB, 1", "LD (nnn,rrr),rr,nnn")]
        [InlineData("LD (.address + XYZ), AB, 1", "LD (nnn,rrr),rr,nnn")]

        [InlineData("LD (0x80000 + XYZ), ABC, 1", "LD (nnn,rrr),rrr,nnn")]
        [InlineData("LD (.address + XYZ), ABC, 1", "LD (nnn,rrr),rrr,nnn")]

        [InlineData("LD (0x80000 + XYZ), ABCD, 1", "LD (nnn,rrr),rrrr,nnn")]
        [InlineData("LD (.address + XYZ), ABCD, 1", "LD (nnn,rrr),rrrr,nnn")]

        [InlineData("LD (0x80000 + XYZ), (0x70000)", "LD (nnn,rrr),(nnn)")]
        [InlineData("LD (0x80000 + XYZ), (.address)", "LD (nnn,rrr),(nnn)")]
        [InlineData("LD (.address + XYZ), (0x70000)", "LD (nnn,rrr),(nnn)")]
        [InlineData("LD (.address + XYZ), (.address)", "LD (nnn,rrr),(nnn)")]

        [InlineData("LD (0x80000 + XYZ), (0x70000 + 3000)", "LD (nnn,rrr),(nnn,nnn)")]
        [InlineData("LD (0x80000 + XYZ), (.address + 3000)", "LD (nnn,rrr),(nnn,nnn)")]
        [InlineData("LD (.address + XYZ), (0x70000 + 3000)", "LD (nnn,rrr),(nnn,nnn)")]
        [InlineData("LD (.address + XYZ), (.address + 3000)", "LD (nnn,rrr),(nnn,nnn)")]
        [InlineData("LD (0x80000 + XYZ), (0x70000 + .address)", "LD (nnn,rrr),(nnn,nnn)")]
        [InlineData("LD (0x80000 + XYZ), (.address + .address)", "LD (nnn,rrr),(nnn,nnn)")]
        [InlineData("LD (.address + XYZ), (0x70000 + .address)", "LD (nnn,rrr),(nnn,nnn)")]
        [InlineData("LD (.address + XYZ), (.address + .address)", "LD (nnn,rrr),(nnn,nnn)")]

        [InlineData("LD (0x80000 + XYZ), (0x70000 + A)", "LD (nnn,rrr),(nnn,r)")]
        [InlineData("LD (0x80000 + XYZ), (.address + A)", "LD (nnn,rrr),(nnn,r)")]
        [InlineData("LD (.address + XYZ), (0x70000 + A)", "LD (nnn,rrr),(nnn,r)")]
        [InlineData("LD (.address + XYZ), (.address + A)", "LD (nnn,rrr),(nnn,r)")]

        [InlineData("LD (0x80000 + XYZ), (0x70000 + AB)", "LD (nnn,rrr),(nnn,rr)")]
        [InlineData("LD (0x80000 + XYZ), (.address + AB)", "LD (nnn,rrr),(nnn,rr)")]
        [InlineData("LD (.address + XYZ), (0x70000 + AB)", "LD (nnn,rrr),(nnn,rr)")]
        [InlineData("LD (.address + XYZ), (.address + AB)", "LD (nnn,rrr),(nnn,rr)")]

        [InlineData("LD (0x80000 + XYZ), (0x70000 + ABC)", "LD (nnn,rrr),(nnn,rrr)")]
        [InlineData("LD (0x80000 + XYZ), (.address + ABC)", "LD (nnn,rrr),(nnn,rrr)")]
        [InlineData("LD (.address + XYZ), (0x70000 + ABC)", "LD (nnn,rrr),(nnn,rrr)")]
        [InlineData("LD (.address + XYZ), (.address + ABC)", "LD (nnn,rrr),(nnn,rrr)")]

        [InlineData("LD (0x80000 + XYZ), (ABC)", "LD (nnn,rrr),(rrr)")]
        [InlineData("LD (.address + XYZ), (ABC)", "LD (nnn,rrr),(rrr)")]

        [InlineData("LD (0x80000 + XYZ), (ABC + 0x70000)", "LD (nnn,rrr),(rrr,nnn)")]
        [InlineData("LD (0x80000 + XYZ), (ABC + .address)", "LD (nnn,rrr),(rrr,nnn)")]
        [InlineData("LD (.address + XYZ), (ABC + 0x70000)", "LD (nnn,rrr),(rrr,nnn)")]
        [InlineData("LD (.address + XYZ), (ABC + .address)", "LD (nnn,rrr),(rrr,nnn)")]

        [InlineData("LD (0x80000 + XYZ), (ABC + X)", "LD (nnn,rrr),(rrr,r)")]
        [InlineData("LD (.address + XYZ), (ABC + X)", "LD (nnn,rrr),(rrr,r)")]

        [InlineData("LD (0x80000 + XYZ), (ABC + XY)", "LD (nnn,rrr),(rrr,rr)")]
        [InlineData("LD (.address + XYZ), (ABC + XY)", "LD (nnn,rrr),(rrr,rr)")]

        [InlineData("LD (0x80000 + XYZ), (ABC + XYZ)", "LD (nnn,rrr),(rrr,rrr)")]
        [InlineData("LD (.address + XYZ), (ABC + XYZ)", "LD (nnn,rrr),(rrr,rrr)")]

        [InlineData("LD (0x80000 + XYZ), F0", "LD (nnn,rrr),fr")]
        [InlineData("LD (.address + XYZ), F0", "LD (nnn,rrr),fr")]
        //
        [InlineData("LD (XYZ + 55), 45, 1", "LD (rrr,nnn),nnnn,n")]
        [InlineData("LD (XYZ + .address), 45, 1", "LD (rrr,nnn),nnnn,n")]
        [InlineData("LD (XYZ + 55), 45, 1, 300", "LD (rrr,nnn),nnnn,n,nnn")]
        [InlineData("LD (XYZ + .address), 45, 1, 300", "LD (rrr,nnn),nnnn,n,nnn")]

        [InlineData("LD (XYZ + 55), A, 1", "LD (rrr,nnn),r,nnn")]
        [InlineData("LD (XYZ + .address), A, 1", "LD (rrr,nnn),r,nnn")]

        [InlineData("LD (XYZ + 55), AB, 1", "LD (rrr,nnn),rr,nnn")]
        [InlineData("LD (XYZ + .address), AB, 1", "LD (rrr,nnn),rr,nnn")]

        [InlineData("LD (XYZ + 55), ABC, 1", "LD (rrr,nnn),rrr,nnn")]
        [InlineData("LD (XYZ + .address), ABC, 1", "LD (rrr,nnn),rrr,nnn")]

        [InlineData("LD (XYZ + 55), ABCD, 1", "LD (rrr,nnn),rrrr,nnn")]
        [InlineData("LD (XYZ + .address), ABCD, 1", "LD (rrr,nnn),rrrr,nnn")]

        [InlineData("LD (XYZ + 55), (0x70000)", "LD (rrr,nnn),(nnn)")]
        [InlineData("LD (XYZ + 55), (.address)", "LD (rrr,nnn),(nnn)")]
        [InlineData("LD (XYZ + .address), (0x70000)", "LD (rrr,nnn),(nnn)")]
        [InlineData("LD (XYZ + .address), (.address)", "LD (rrr,nnn),(nnn)")]

        [InlineData("LD (XYZ + 55), (0x70000 + 3000)", "LD (rrr,nnn),(nnn,nnn)")]
        [InlineData("LD (XYZ + 55), (.address + 3000)", "LD (rrr,nnn),(nnn,nnn)")]
        [InlineData("LD (XYZ + .address), (0x70000 + 3000)", "LD (rrr,nnn),(nnn,nnn)")]
        [InlineData("LD (XYZ + .address), (.address + 3000)", "LD (rrr,nnn),(nnn,nnn)")]
        [InlineData("LD (XYZ + 55), (0x70000 + .address)", "LD (rrr,nnn),(nnn,nnn)")]
        [InlineData("LD (XYZ + 55), (.address + .address)", "LD (rrr,nnn),(nnn,nnn)")]
        [InlineData("LD (XYZ + .address), (0x70000 + .address)", "LD (rrr,nnn),(nnn,nnn)")]
        [InlineData("LD (XYZ + .address), (.address + .address)", "LD (rrr,nnn),(nnn,nnn)")]

        [InlineData("LD (XYZ + 55), (0x70000 + A)", "LD (rrr,nnn),(nnn,r)")]
        [InlineData("LD (XYZ + 55), (.address + A)", "LD (rrr,nnn),(nnn,r)")]
        [InlineData("LD (XYZ + .address), (0x70000 + A)", "LD (rrr,nnn),(nnn,r)")]
        [InlineData("LD (XYZ + .address), (.address + A)", "LD (rrr,nnn),(nnn,r)")]

        [InlineData("LD (XYZ + 55), (0x70000 + AB)", "LD (rrr,nnn),(nnn,rr)")]
        [InlineData("LD (XYZ + 55), (.address + AB)", "LD (rrr,nnn),(nnn,rr)")]
        [InlineData("LD (XYZ + .address), (0x70000 + AB)", "LD (rrr,nnn),(nnn,rr)")]
        [InlineData("LD (XYZ + .address), (.address + AB)", "LD (rrr,nnn),(nnn,rr)")]

        [InlineData("LD (XYZ + 55), (0x70000 + ABC)", "LD (rrr,nnn),(nnn,rrr)")]
        [InlineData("LD (XYZ + 55), (.address + ABC)", "LD (rrr,nnn),(nnn,rrr)")]
        [InlineData("LD (XYZ + .address), (0x70000 + ABC)", "LD (rrr,nnn),(nnn,rrr)")]
        [InlineData("LD (XYZ + .address), (.address + ABC)", "LD (rrr,nnn),(nnn,rrr)")]

        [InlineData("LD (XYZ + 55), (ABC)", "LD (rrr,nnn),(rrr)")]
        [InlineData("LD (XYZ + .address), (ABC)", "LD (rrr,nnn),(rrr)")]

        [InlineData("LD (XYZ + 55), (ABC + 0x70000)", "LD (rrr,nnn),(rrr,nnn)")]
        [InlineData("LD (XYZ + 55), (ABC + .address)", "LD (rrr,nnn),(rrr,nnn)")]
        [InlineData("LD (XYZ + .address), (ABC + 0x70000)", "LD (rrr,nnn),(rrr,nnn)")]
        [InlineData("LD (XYZ + .address), (ABC + .address)", "LD (rrr,nnn),(rrr,nnn)")]

        [InlineData("LD (XYZ + 55), (ABC + X)", "LD (rrr,nnn),(rrr,r)")]
        [InlineData("LD (XYZ + .address), (ABC + X)", "LD (rrr,nnn),(rrr,r)")]

        [InlineData("LD (XYZ + 55), (ABC + XY)", "LD (rrr,nnn),(rrr,rr)")]
        [InlineData("LD (XYZ + .address), (ABC + XY)", "LD (rrr,nnn),(rrr,rr)")]

        [InlineData("LD (XYZ + 55), (ABC + XYZ)", "LD (rrr,nnn),(rrr,rrr)")]
        [InlineData("LD (XYZ + .address), (ABC + XYZ)", "LD (rrr,nnn),(rrr,rrr)")]

        [InlineData("LD (XYZ + 55), F0", "LD (rrr,nnn),fr")]
        [InlineData("LD (XYZ + .address), F0", "LD (rrr,nnn),fr")]
        //
        [InlineData("LD (XYZ + A), 45, 1", "LD (rrr,r),nnnn,n")]
        [InlineData("LD (XYZ + A), 45, 1, 300", "LD (rrr,r),nnnn,n,nnn")]

        [InlineData("LD (XYZ + A), A, 1", "LD (rrr,r),r,nnn")]
        [InlineData("LD (XYZ + A), AB, 1", "LD (rrr,r),rr,nnn")]
        [InlineData("LD (XYZ + A), ABC, 1", "LD (rrr,r),rrr,nnn")]
        [InlineData("LD (XYZ + A), ABCD, 1", "LD (rrr,r),rrrr,nnn")]

        [InlineData("LD (XYZ + A), (0x70000)", "LD (rrr,r),(nnn)")]
        [InlineData("LD (XYZ + A), (.address)", "LD (rrr,r),(nnn)")]

        [InlineData("LD (XYZ + A), (0x70000 + 3000)", "LD (rrr,r),(nnn,nnn)")]
        [InlineData("LD (XYZ + A), (.address + 3000)", "LD (rrr,r),(nnn,nnn)")]
        [InlineData("LD (XYZ + A), (0x70000 + .address)", "LD (rrr,r),(nnn,nnn)")]
        [InlineData("LD (XYZ + A), (.address + .address)", "LD (rrr,r),(nnn,nnn)")]

        [InlineData("LD (XYZ + A), (0x70000 + A)", "LD (rrr,r),(nnn,r)")]
        [InlineData("LD (XYZ + A), (.address + A)", "LD (rrr,r),(nnn,r)")]

        [InlineData("LD (XYZ + A), (0x70000 + AB)", "LD (rrr,r),(nnn,rr)")]
        [InlineData("LD (XYZ + A), (.address + AB)", "LD (rrr,r),(nnn,rr)")]

        [InlineData("LD (XYZ + A), (0x70000 + ABC)", "LD (rrr,r),(nnn,rrr)")]
        [InlineData("LD (XYZ + A), (.address + ABC)", "LD (rrr,r),(nnn,rrr)")]

        [InlineData("LD (XYZ + A), (ABC)", "LD (rrr,r),(rrr)")]

        [InlineData("LD (XYZ + A), (ABC + 0x70000)", "LD (rrr,r),(rrr,nnn)")]
        [InlineData("LD (XYZ + A), (ABC + .address)", "LD (rrr,r),(rrr,nnn)")]

        [InlineData("LD (XYZ + A), (ABC + X)", "LD (rrr,r),(rrr,r)")]
        [InlineData("LD (XYZ + A), (ABC + XY)", "LD (rrr,r),(rrr,rr)")]
        [InlineData("LD (XYZ + A), (ABC + XYZ)", "LD (rrr,r),(rrr,rrr)")]

        [InlineData("LD (XYZ + A), F0", "LD (rrr,r),fr")]
        //
        [InlineData("LD (XYZ + AB), 45, 1", "LD (rrr,rr),nnnn,n")]
        [InlineData("LD (XYZ + AB), 45, 1, 300", "LD (rrr,rr),nnnn,n,nnn")]

        [InlineData("LD (XYZ + AB), A, 1", "LD (rrr,rr),r,nnn")]
        [InlineData("LD (XYZ + AB), AB, 1", "LD (rrr,rr),rr,nnn")]
        [InlineData("LD (XYZ + AB), ABC, 1", "LD (rrr,rr),rrr,nnn")]
        [InlineData("LD (XYZ + AB), ABCD, 1", "LD (rrr,rr),rrrr,nnn")]

        [InlineData("LD (XYZ + AB), (0x70000)", "LD (rrr,rr),(nnn)")]
        [InlineData("LD (XYZ + AB), (.address)", "LD (rrr,rr),(nnn)")]

        [InlineData("LD (XYZ + AB), (0x70000 + 3000)", "LD (rrr,rr),(nnn,nnn)")]
        [InlineData("LD (XYZ + AB), (.address + 3000)", "LD (rrr,rr),(nnn,nnn)")]
        [InlineData("LD (XYZ + AB), (0x70000 + .address)", "LD (rrr,rr),(nnn,nnn)")]
        [InlineData("LD (XYZ + AB), (.address + .address)", "LD (rrr,rr),(nnn,nnn)")]

        [InlineData("LD (XYZ + AB), (0x70000 + A)", "LD (rrr,rr),(nnn,r)")]
        [InlineData("LD (XYZ + AB), (.address + A)", "LD (rrr,rr),(nnn,r)")]

        [InlineData("LD (XYZ + AB), (0x70000 + AB)", "LD (rrr,rr),(nnn,rr)")]
        [InlineData("LD (XYZ + AB), (.address + AB)", "LD (rrr,rr),(nnn,rr)")]

        [InlineData("LD (XYZ + AB), (0x70000 + ABC)", "LD (rrr,rr),(nnn,rrr)")]
        [InlineData("LD (XYZ + AB), (.address + ABC)", "LD (rrr,rr),(nnn,rrr)")]

        [InlineData("LD (XYZ + AB), (ABC)", "LD (rrr,rr),(rrr)")]

        [InlineData("LD (XYZ + AB), (ABC + 0x70000)", "LD (rrr,rr),(rrr,nnn)")]
        [InlineData("LD (XYZ + AB), (ABC + .address)", "LD (rrr,rr),(rrr,nnn)")]

        [InlineData("LD (XYZ + AB), (ABC + X)", "LD (rrr,rr),(rrr,r)")]
        [InlineData("LD (XYZ + AB), (ABC + XY)", "LD (rrr,rr),(rrr,rr)")]
        [InlineData("LD (XYZ + AB), (ABC + XYZ)", "LD (rrr,rr),(rrr,rrr)")]

        [InlineData("LD (XYZ + AB), F0", "LD (rrr,rr),fr")]
        //
        [InlineData("LD (XYZ + ABC), 45, 1", "LD (rrr,rrr),nnnn,n")]
        [InlineData("LD (XYZ + ABC), 45, 1, 300", "LD (rrr,rrr),nnnn,n,nnn")]

        [InlineData("LD (XYZ + ABC), A, 1", "LD (rrr,rrr),r,nnn")]
        [InlineData("LD (XYZ + ABC), AB, 1", "LD (rrr,rrr),rr,nnn")]
        [InlineData("LD (XYZ + ABC), ABC, 1", "LD (rrr,rrr),rrr,nnn")]
        [InlineData("LD (XYZ + ABC), ABCD, 1", "LD (rrr,rrr),rrrr,nnn")]

        [InlineData("LD (XYZ + ABC), (0x70000)", "LD (rrr,rrr),(nnn)")]
        [InlineData("LD (XYZ + ABC), (.address)", "LD (rrr,rrr),(nnn)")]

        [InlineData("LD (XYZ + ABC), (0x70000 + 3000)", "LD (rrr,rrr),(nnn,nnn)")]
        [InlineData("LD (XYZ + ABC), (.address + 3000)", "LD (rrr,rrr),(nnn,nnn)")]
        [InlineData("LD (XYZ + ABC), (0x70000 + .address)", "LD (rrr,rrr),(nnn,nnn)")]
        [InlineData("LD (XYZ + ABC), (.address + .address)", "LD (rrr,rrr),(nnn,nnn)")]

        [InlineData("LD (XYZ + ABC), (0x70000 + A)", "LD (rrr,rrr),(nnn,r)")]
        [InlineData("LD (XYZ + ABC), (.address + A)", "LD (rrr,rrr),(nnn,r)")]

        [InlineData("LD (XYZ + ABC), (0x70000 + AB)", "LD (rrr,rrr),(nnn,rr)")]
        [InlineData("LD (XYZ + ABC), (.address + AB)", "LD (rrr,rrr),(nnn,rr)")]

        [InlineData("LD (XYZ + ABC), (0x70000 + ABC)", "LD (rrr,rrr),(nnn,rrr)")]
        [InlineData("LD (XYZ + ABC), (.address + ABC)", "LD (rrr,rrr),(nnn,rrr)")]

        [InlineData("LD (XYZ + ABC), (ABC)", "LD (rrr,rrr),(rrr)")]

        [InlineData("LD (XYZ + ABC), (ABC + 0x70000)", "LD (rrr,rrr),(rrr,nnn)")]
        [InlineData("LD (XYZ + ABC), (ABC + .address)", "LD (rrr,rrr),(rrr,nnn)")]

        [InlineData("LD (XYZ + ABC), (ABC + X)", "LD (rrr,rrr),(rrr,r)")]
        [InlineData("LD (XYZ + ABC), (ABC + XY)", "LD (rrr,rrr),(rrr,rr)")]
        [InlineData("LD (XYZ + ABC), (ABC + XYZ)", "LD (rrr,rrr),(rrr,rrr)")]

        [InlineData("LD (XYZ + ABC), F0", "LD (rrr,rrr),fr")]
        //
        [InlineData("LD F0, (0x70000 + 3000)", "LD fr,(nnn,nnn)")]
        [InlineData("LD F0, (.address + 3000)", "LD fr,(nnn,nnn)")]
        [InlineData("LD F0, (0x70000 + .address)", "LD fr,(nnn,nnn)")]
        [InlineData("LD F0, (.address + .address)", "LD fr,(nnn,nnn)")]

        [InlineData("LD F0, (0x70000 + A)", "LD fr,(nnn,r)")]
        [InlineData("LD F0, (.address + A)", "LD fr,(nnn,r)")]

        [InlineData("LD F0, (0x70000 + AB)", "LD fr,(nnn,rr)")]
        [InlineData("LD F0, (.address + AB)", "LD fr,(nnn,rr)")]

        [InlineData("LD F0, (0x70000 + ABC)", "LD fr,(nnn,rrr)")]
        [InlineData("LD F0, (.address + ABC)", "LD fr,(nnn,rrr)")]

        [InlineData("LD F0, (ABC + 0x70000)", "LD fr,(rrr,nnn)")]
        [InlineData("LD F0, (ABC + .address)", "LD fr,(rrr,nnn)")]

        [InlineData("LD F0, (ABC + X)", "LD fr,(rrr,r)")]
        [InlineData("LD F0, (ABC + XY)", "LD fr,(rrr,rr)")]
        [InlineData("LD F0, (ABC + XYZ)", "LD fr,(rrr,rrr)")]

        [InlineData("LD F0, F0", "LD fr,fr")]
        public void Test_LD_Indexing_GeneralForm(string source, string expectedGeneralForm)
        {
            var assembler = new Assembler();
            using var computer = new Computer();

            assembler.Build(source);
            var line = assembler.GetCompiledLine(0);

            Assert.Equal(expectedGeneralForm, line.GeneralForm);
        }
    }
}
