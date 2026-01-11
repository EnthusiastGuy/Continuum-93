using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using Xunit;

namespace CPUTests;

public class AndInstructionTests
{
    // ----------------------------
    // Harness (line-safe)
    // ----------------------------

    private static string Program(params string[] lines)
        => string.Join("\n", lines) + "\n";

    private static Computer Run(params string[] lines)
    {
        Assembler a = new();
        a.Build(Program(lines));

        var computer = new Computer();
        byte[] code = a.GetCompiledCode();
        computer.LoadMem(code);
        computer.Run();
        return computer;
    }

    private static void AssertCarryReset(Computer c)
        => Assert.False(c.CPU.FLAGS.GetValueByName("C"));

    private static void AssertZero(Computer c, bool expected)
        => Assert.Equal(expected, c.CPU.FLAGS.GetValueByName("Z"));

    private static readonly string[] SetCarryWithAdd =
    [
        "LD A, 0x80",
        "ADD A, 0xFF", // should set carry
    ];

    // Utility to prepend a snippet (like SetCarryWithAdd) to a program
    private static string[] Prepend(string[] prefix, params string[] lines)
    {
        var result = new string[prefix.Length + lines.Length];
        Array.Copy(prefix, 0, result, 0, prefix.Length);
        Array.Copy(lines, 0, result, prefix.Length, lines.Length);
        return result;
    }

    // ----------------------------
    // 1) Carry must reset after AND
    // ----------------------------

    public static object[][] CarryResetCases =>
    [
        // 8-bit register destinations
        new object[] { "AND A, imm8",
            Prepend(SetCarryWithAdd,
                "AND A, 0xFF",
                "BREAK")
        },
        new object[] { "AND A, A",
            Prepend(SetCarryWithAdd,
                "AND A, A",
                "BREAK")
        },
        new object[] { "AND A, r8",
            Prepend(SetCarryWithAdd,
                "LD E, 0xFF",
                "AND A, E",
                "BREAK")
        },

        // 16-bit reg destinations (AB)
        new object[] { "AND AB, imm16",
            Prepend(SetCarryWithAdd,
                "AND AB, 0xFFFF",
                "BREAK")
        },
        new object[] { "AND AB, AB",
            Prepend(SetCarryWithAdd,
                "AND AB, AB",
                "BREAK")
        },
        new object[] { "AND AB, rr16",
            Prepend(SetCarryWithAdd,
                "LD EF, 0xFFFF",
                "AND AB, EF",
                "BREAK")
        },

        // 24-bit reg destinations (ABC)
        new object[] { "AND ABC, imm24",
            Prepend(SetCarryWithAdd,
                "AND ABC, 0xFFFFFF",
                "BREAK")
        },
        new object[] { "AND ABC, ABC",
            Prepend(SetCarryWithAdd,
                "AND ABC, ABC",
                "BREAK")
        },
        new object[] { "AND ABC, rrr24",
            Prepend(SetCarryWithAdd,
                "LD EFG, 0xFFFFFF",
                "AND ABC, EFG",
                "BREAK")
        },

        // 32-bit reg destinations (ABCD)
        new object[] { "AND ABCD, imm32",
            Prepend(SetCarryWithAdd,
                "AND ABCD, 0xFFFFFFFF",
                "BREAK")
        },
        new object[] { "AND ABCD, ABCD",
            Prepend(SetCarryWithAdd,
                "AND ABCD, ABCD",
                "BREAK")
        },
        new object[] { "AND ABCD, rrrr32",
            Prepend(SetCarryWithAdd,
                "LD EFGH, 0xFFFFFFFF",
                "AND ABCD, EFGH",
                "BREAK")
        },

        // Memory destination scalar widths (immediate forms you already use)
        new object[] { "AND (ptr), imm8 width=1",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "AND (BCD), 0xFF, 1",
                "BREAK")
        },
        new object[] { "AND (ptr), imm16 width=2",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "AND (BCD), 0xFFFF, 2",
                "BREAK")
        },
        new object[] { "AND (ptr), imm24 width=3",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "AND (BCD), 0xFFFFFF, 3",
                "BREAK")
        },
        new object[] { "AND (ptr), imm32 width=4",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "AND (BCD), 0xFFFFFFFF, 4",
                "BREAK")
        },

        // Memory destination scalar from registers (r/rr/rrr/rrrr)
        new object[] { "AND (ptr), r8",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD E, 0xFF",
                "AND (BCD), E",
                "BREAK")
        },
        new object[] { "AND (ptr), rr16",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD EF, 0xFFFF",
                "AND (BCD), EF",
                "BREAK")
        },
        new object[] { "AND (ptr), rrr24",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD EFG, 0xFFFFFF",
                "AND (BCD), EFG",
                "BREAK")
        },
        new object[] { "AND (ptr), rrrr32",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD EFGH, 0xFFFFFFFF",
                "AND (BCD), EFGH",
                "BREAK")
        },

        // Register destination AND from memory (if supported by your assembler)
        // These were in ExAND; keep them if your syntax matches. :contentReference[oaicite:0]{index=0}
        new object[] { "AND A, (ptr)",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD (BCD), 0xFF, 1",
                "AND A, (BCD)",
                "BREAK")
        },
        new object[] { "AND AB, (ptr)",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD (BCD), 0xFFFF, 2",
                "AND AB, (BCD)",
                "BREAK")
        },
        new object[] { "AND ABC, (ptr)",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD (BCD), 0xFFFFFF, 3",
                "AND ABC, (BCD)",
                "BREAK")
        },
        new object[] { "AND ABCD, (ptr)",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD (BCD), 0xFFFFFFFF, 4",
                "AND ABCD, (BCD)",
                "BREAK")
        },

        // Block immediate mask (count != 1..4 distinguishes from scalar width in your encoding)
        new object[] { "AND block imm32 (count=8)",
            Prepend(SetCarryWithAdd,
                "LD BCD, 20000",
                "LD (BCD), 0xFFFFFFFF, 4",
                "LD R, 4",
                "ADD BCD, R",
                "LD (BCD), 0xFFFFFFFF, 4",
                "LD BCD, 20000",
                "AND (BCD), 0x0F0F0F0F, 8",
                "BREAK")
        },

        // Block immediate mask with repeat (if your assembler supports it)
        new object[] { "AND block imm32 repeat=2 (count=4)",
            Prepend(SetCarryWithAdd,
                "LD BCD, 21000",
                "LD (BCD), 0xFFFFFFFF, 4",
                "LD R, 4",
                "ADD BCD, R",
                "LD (BCD), 0xFFFFFFFF, 4",
                "LD BCD, 21000",
                "AND (BCD), 0x00FF00FF, 4, 2",
                "BREAK")
        },
    ];

    [Theory]
    [MemberData(nameof(CarryResetCases))]
    public void And_resets_carry(string name, string[] programLines)
    {
        using var c = Run(programLines);
        AssertCarryReset(c);
    }

    // ----------------------------
    // 2) Zero flag behavior
    // ----------------------------

    public static object[][] ZeroFlagCases =>
    [
        new object[] { "A non-zero => Z=0",
            new[] { "LD A, 0x80", "AND A, 0xFF", "BREAK" }, false },

        new object[] { "A & 0 => Z=1",
            new[] { "LD A, 0x80", "AND A, 0", "BREAK" }, true },

        new object[] { "A & r(0) => Z=1",
            new[] { "LD A, 0x80", "LD E, 0", "AND A, E", "BREAK" }, true },

        new object[] { "AB & 0 => Z=1",
            new[] { "LD AB, 0x1234", "AND AB, 0", "BREAK" }, true },

        new object[] { "ABC & 0 => Z=1",
            new[] { "LD ABC, 0x123456", "AND ABC, 0", "BREAK" }, true },

        new object[] { "ABCD & 0 => Z=1",
            new[] { "LD ABCD, 0x12345678", "AND ABCD, 0", "BREAK" }, true },

        new object[] { "mem8 -> zero => Z=1",
            new[] { "LD BCD, 20000", "LD (BCD), 0x80, 1", "AND (BCD), 0, 1", "BREAK" }, true },

        new object[] { "mem16 -> zero => Z=1",
            new[] { "LD BCD, 20000", "LD (BCD), 0xFFFF, 2", "AND (BCD), 0, 2", "BREAK" }, true },

        // Register destination from memory (if supported) :contentReference[oaicite:1]{index=1}
        new object[] { "A & (ptr=0) => Z=1",
            new[] { "LD A, 0xFF", "LD BCD, 20000", "LD (BCD), 0, 1", "AND A, (BCD)", "BREAK" }, true },
    ];

    [Theory]
    [MemberData(nameof(ZeroFlagCases))]
    public void And_sets_zero_flag_correctly(string name, string[] programLines, bool expectedZ)
    {
        using var c = Run(programLines);
        AssertZero(c, expectedZ);
    }

    // ----------------------------
    // 3) Result correctness
    // ----------------------------

    public static object[][] ResultCases =>
    [
        new object[]
        {
            "A = 0x7F & 0x0F => 0x0F",
            Prepend(SetCarryWithAdd,
                "AND A, 0x0F",
                "BREAK"),
            (Action<Computer>)(c => Assert.Equal((byte)0x0F, c.CPU.REGS.A))
        },
        new object[]
        {
            "AB = 0x00FF & 0x0F0F => 0x000F",
            new[] { "LD AB, 0x00FF", "AND AB, 0x0F0F", "BREAK" },
            (Action<Computer>)(c => Assert.Equal((ushort)0x000F, c.CPU.REGS.AB))
        },
        new object[]
        {
            "mem8: 0xF0 & 0x0F => 0x00",
            new[] { "LD BCD, 20000", "LD (BCD), 0xF0, 1", "AND (BCD), 0x0F, 1", "BREAK" },
            (Action<Computer>)(c => Assert.Equal((byte)0x00, c.MEMC.Get8bitFromRAM(20000)))
        },
        new object[]
        {
            "block imm32 count=8 masks all bytes",
            new[]
            {
                "LD BCD, 20000",
                "LD (BCD), 0xFFFFFFFF, 4",
                "LD R, 4",
                "ADD BCD, R",
                "LD (BCD), 0xFFFFFFFF, 4",
                "LD BCD, 20000",
                "AND (BCD), 0x0F0F0F0F, 8",
                "BREAK"
            },
            (Action<Computer>)(c =>
            {
                for (int i = 0; i < 8; i++)
                    Assert.Equal((byte)0x0F, c.MEMC.Get8bitFromRAM(20000u + (uint)i));
            })
        },
    ];

    [Theory]
    [MemberData(nameof(ResultCases))]
    public void And_produces_expected_result(string name, string[] programLines, Action<Computer> assert)
    {
        using var c = Run(programLines);
        assert(c);
    }
}
