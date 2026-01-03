using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Continuum93.Emulator;
using Continuum93.Emulator.AutoDocs;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator.Mnemonics;
using ContinuumUnitTests._Tools;
using System.Text;
using Xunit;

namespace DebuggerTests.Instructions
{
    [Collection("Debugger tests")]
    public class TestDisassemblerInstructions
    {
        [Fact]
        public void TestGeneralInstructionsDisassembly()
        {
            string autoInstructions = DebugInstructionSamples.Generate();

            string[] instructionLines = autoInstructions.Split([ "\r\n", "\r", "\n" ], StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(instructionLines);
            int errors = 0;

            string report = "";

            Assembler cp = new();
            using Computer computer = new();

            int lineNumber = 1;

            foreach (string instruction in instructionLines)
            {
                computer.Clear();
                cp.Build(instruction);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMemAt(0, compiled);

                ContinuumDebugger.DebugInstructionsCount = 1;
                ContinuumDebugger.RunAt(0, computer, true);
                string dissassembly = ContinuumDebugger.GetDissassembled().Trim();

                string state = "    ";

                if (!instruction.Equals(dissassembly))
                {
                    errors++;
                    state = "NOK ";
                } else
                {
                    state = "OK  ";
                }

                string hexString = BitConverter.ToString(compiled).Replace("-", ", ");

                report += $"{lineNumber,5} {state} Compiled: \"{instruction}\" to \"{hexString}\", disassembled back to: \"{dissassembly}\"{Constants.CR}";
                lineNumber++;
            }

            ReportsManager.SaveReport("DisassemblyValidity", report);

            Assert.Equal(0, errors);
        }

        [Fact]
        public void TestGeneralInstructionsDisassemblyTable()
        {
            string autoInstructions = DebugInstructionSamples.Generate();
            string[] instructionLines = autoInstructions
                .Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(instructionLines);

            var rows = new List<(string Nr, string Status, string Instr, string Hex, string Diss)>();

            Assembler cp = new();
            using Computer computer = new();
            int lineNumber = 1;
            int errors = 0;

            //–– 1) Collect data into a small table
            foreach (string instruction in instructionLines)
            {
                computer.Clear();
                cp.Build(instruction);
                byte[] compiled = cp.GetCompiledCode();
                computer.LoadMemAt(0, compiled);

                ContinuumDebugger.DebugInstructionsCount = 1;
                ContinuumDebugger.RunAt(0, computer, true);
                string dissassembly = ContinuumDebugger.GetDissassembled().Trim();

                bool ok = instruction.Equals(dissassembly);
                if (!ok) errors++;

                string status = ok ? "OK" : "NOK";
                string hexString = BitConverter.ToString(compiled).Replace("-", ", ");

                rows.Add((
                    Nr: lineNumber.ToString().PadLeft(5),
                    Status: status,
                    Instr: instruction,
                    Hex: hexString,
                    Diss: dissassembly
                ));

                lineNumber++;
            }

            //–– 2) Compute max‐width per column (including header text)
            string hdrNr = "Nr.";
            string hdrSt = "Status";
            string hdrInst = "Compiled instruction";
            string hdrHex = "Bytecodes (hex)";
            string hdrDiss = "Disassembled to";

            int wNr = Math.Max(hdrNr.Length, rows.Max(r => r.Nr.Length));
            int wSt = Math.Max(hdrSt.Length, rows.Max(r => r.Status.Length));
            int wInst = Math.Max(hdrInst.Length, rows.Max(r => r.Instr.Length));
            int wHex = Math.Max(hdrHex.Length, rows.Max(r => r.Hex.Length));
            int wDiss = Math.Max(hdrDiss.Length, rows.Max(r => r.Diss.Length));

            //–– 3) Build a single format‐string that pads each column
            string fmt =
                $"{{0,-{wNr}}}\t{{1,-{wSt}}}\t{{2,-{wInst}}}\t{{3,-{wHex}}}\t{{4,-{wDiss}}}";

            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Format(fmt, hdrNr, hdrSt, hdrInst, hdrHex, hdrDiss));

            // Separator (just a line of dashes as long as the total width + tabs)
            int totalTextWidth = wNr + wSt + wInst + wHex + wDiss;
            // plus 4 tabs between columns (1 char each in most consoles)
            sb.AppendLine(new string('-', totalTextWidth + 4));

            // Rows
            foreach (var r in rows)
            {
                sb.AppendLine(string.Format(fmt, r.Nr, r.Status, r.Instr, r.Hex, r.Diss));
            }

            string report = sb.ToString();

            ReportsManager.SaveReport("DisassemblyValidityTable", report);

            Assert.Equal(0, errors);
        }

        [Fact]
        public void TestGeneralInstructionsRaw()
        {
            string autoInstructions = DebugInstructionSamples.GenerateRawInstructions();
            string[] instructionLines = autoInstructions
                .Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(instructionLines);

            // 1) Split each line into (Instruction, Format) tuples
            var rows = instructionLines
                .Select(line =>
                {
                    var parts = line.Split(';');
                    return (
                        Instr: parts[0].Trim(),
                        Format: parts.Length > 1 ? parts[1].Trim() : ""
                    );
                })
                .ToList();

            Assert.NotEmpty(rows);

            // 2) Compute max‐widths (including header)
            const string hdrNr = "Nr.";
            const string hdrInstr = "Instruction";
            const string hdrFormat = "Format";

            int wNr = 5;  // fixed to 5 chars, space‑padded
            int wInstr = Math.Max(hdrInstr.Length, rows.Max(r => r.Instr.Length));
            int wFormat = Math.Max(hdrFormat.Length, rows.Max(r => r.Format.Length));

            // 3) Build our format string (pad each col, left‑justified)
            string fmt = $"{{0,-{wNr}}}   {{1,-{wInstr}}}   {{2,-{wFormat}}}";

            var sb = new StringBuilder();
            // —— compute and emit unused‐bits statistic ——
            int totalBits = rows.Sum(r => r.Format.Count(c => c != ' '));
            int unusedBits = rows.Sum(r => r.Format.Count(c => c == 'u'));
            double unusedPct = totalBits > 0
                ? (double)unusedBits / totalBits * 100
                : 0;
            sb.AppendLine($"Unused bits: {unusedPct:F2}%");
            sb.AppendLine();
            // Header
            sb.AppendLine(string.Format(fmt, hdrNr, hdrInstr, hdrFormat));
            // Separator
            sb.AppendLine(new string('-', wNr + 3 + wInstr + 3 + wFormat));
            // Rows
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                string nr = (i + 1).ToString().PadLeft(5);
                sb.AppendLine(string.Format(fmt, nr, r.Instr, r.Format));
            }

            // 4) Output
            string report = sb.ToString();

            ReportsManager.SaveReport("RawInstructionsFormat", report);
        }

        [Fact]
        public void TestGeneralInstructionsSorted()
        {
            string autoInstructions = DebugInstructionSamples.GenerateRawInstructions();
            string[] instructionLines = autoInstructions
                .Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);

            // 1) Split each line into (Instruction, Format) tuples and get primary and secondary op codes
            var rows = instructionLines
                .Select(line =>
                {
                    var parts = line.Split(';');
                    string mnemonic = parts[0].Trim();
                    string format = parts.Length > 1 ? parts[1].Trim() : "";

                    // Get primary and secondary op codes from the mnemonic
                    byte primaryOpCode = 255;
                    byte? secondaryOpCode = null;
                    
                    if (Mnem.OPS.TryGetValue(mnemonic, out Oper op))
                    {
                        if (op.IsPrimary)
                        {
                            // Primary instruction: use its op code as primary, no secondary
                            primaryOpCode = op.OpCode;
                            secondaryOpCode = 255; // No secondary op code for primary instructions
                        }
                        else
                        {
                            // Non-primary instruction: use ParentCode as primary, OpCodes[0] as secondary
                            primaryOpCode = op.ParentCode ?? 255;
                            secondaryOpCode = op.OpCodes != null && op.OpCodes.Length > 0 
                                ? op.OpCodes[0] 
                                : (byte?)255;
                        }
                    }
                    else
                    {
                        // If mnemonic not found, use 255 for both
                        primaryOpCode = 255;
                        secondaryOpCode = 255;
                    }

                    return (
                        Instr: mnemonic,
                        Format: format,
                        PrimaryOpCode: primaryOpCode,
                        SecondaryOpCode: secondaryOpCode ?? 255
                    );
                })
                .OrderBy(r => r.PrimaryOpCode)      // First sort by primary op code (ADD, LD, etc.)
                .ThenBy(r => r.SecondaryOpCode)     // Then by secondary op code (_r_n, _r_r, etc.)
                .ThenBy(r => r.Instr)               // Finally by instruction name for same op codes
                .ToList();

            Assert.NotEmpty(rows);

            // 2) Compute max‐widths (including header)
            const string hdrNr = "Nr.";
            const string hdrInstr = "Instruction";
            const string hdrFormat = "Format";
            const string hdrSubOp = "SubOp";

            int wNr = 5;  // fixed to 5 chars, space‑padded
            int wInstr = Math.Max(hdrInstr.Length, rows.Max(r => r.Instr.Length));
            int wFormat = Math.Max(hdrFormat.Length, rows.Max(r => r.Format.Length));
            int wSubOp = Math.Max(hdrSubOp.Length, Math.Max(3, rows.Max(r => r.SecondaryOpCode == 255 ? 3 : r.SecondaryOpCode.ToString().Length))); // "N/A" is 3 chars

            // 3) Build our format string (pad each col, left‑justified)
            string fmt = $"{{0,-{wNr}}}   {{1,-{wInstr}}}   {{2,-{wFormat}}}   {{3,-{wSubOp}}}";

            var sb = new StringBuilder();
            // —— compute and emit unused‐bits statistic ——
            int totalBits = rows.Sum(r => r.Format.Count(c => c != ' '));
            int unusedBits = rows.Sum(r => r.Format.Count(c => c == 'u'));
            double unusedPct = totalBits > 0
                ? (double)unusedBits / totalBits * 100
                : 0;
            sb.AppendLine($"Unused bits: {unusedPct:F2}%");
            sb.AppendLine();
            // Header
            sb.AppendLine(string.Format(fmt, hdrNr, hdrInstr, hdrFormat, hdrSubOp));
            // Separator
            sb.AppendLine(new string('-', wNr + 3 + wInstr + 3 + wFormat + 3 + wSubOp));
            // Rows
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                string nr = (i + 1).ToString().PadLeft(5);
                string subOpStr = r.SecondaryOpCode == 255 ? "N/A" : r.SecondaryOpCode.ToString();
                sb.AppendLine(string.Format(fmt, nr, r.Instr, r.Format, subOpStr));
            }

            // 4) Output
            string report = sb.ToString();

            ReportsManager.SaveReport("RawInstructionsSorted", report);
        }

        [Fact]
        public void TestGeneralInstructionsMemoryToMemory()
        {
            string autoInstructions = DebugInstructionSamples.GenerateRawInstructions();
            string[] instructionLines = autoInstructions
                .Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(instructionLines);

            // 1) Define the modes we care about
            var twoParam = new[] { "(nnn),(nnn)", "(nnn),(rrr)", "(rrr),(nnn)", "(rrr),(rrr)" };
            var oneParam = new[] { "(nnn)", "(rrr)" };

            // 2) Group by mnemonic, collecting any matching mode strings
            var modesByInstr = new Dictionary<string, HashSet<string>>();
            foreach (var line in instructionLines)
            {
                var instrPart = line.Split(';')[0].Trim();
                var parts = instrPart.Split(' ', 2);
                var mnemonic = parts[0];
                var operands = parts.Length > 1 ? parts[1].Trim() : "";

                if (!modesByInstr.ContainsKey(mnemonic))
                    modesByInstr[mnemonic] = new HashSet<string>();

                var modes = modesByInstr[mnemonic];

                // two‐parameter checks
                foreach (var mode in twoParam)
                    if (operands.Equals(mode))
                        modes.Add(mode);

                // one‐parameter checks (no comma => single operand)
                if (!operands.Contains(","))
                {
                    foreach (var mode in oneParam)
                        if (operands.Contains(mode))
                            modes.Add(mode);
                }
            }

            // 3) Prepare an ordered list of rows with a text summary or "NONE"
            var rows = modesByInstr
                .OrderBy(kv => kv.Key)
                .Select(kv => new {
                    Mnemonic = kv.Key,
                    Modes = kv.Value.Count > 0
                                ? string.Join("; ", kv.Value)
                                : "NONE"
                })
                .ToList();

            // 4) Compute column widths (Nr fixed at 5 chars)
            const string hdrNr = "Nr.";
            const string hdrInstr = "Instruction";
            const string hdrModes = "Modes";

            int wNr = 5;
            int wInstr = Math.Max(hdrInstr.Length, rows.Max(r => r.Mnemonic.Length));
            int wModes = Math.Max(hdrModes.Length, rows.Max(r => r.Modes.Length));

            // 5) Build a single composite format string
            string fmt = $"{{0,-{wNr}}}   {{1,-{wInstr}}}   {{2,-{wModes}}}";

            var sb = new StringBuilder();

            // 6) Header + separator
            sb.AppendLine(string.Format(fmt, hdrNr, hdrInstr, hdrModes));
            sb.AppendLine(new string('-', wNr + 3 + wInstr + 3 + wModes));

            // 7) Numbered rows
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                string nr = (i + 1).ToString().PadLeft(5);
                sb.AppendLine(string.Format(fmt, nr, row.Mnemonic, row.Modes));
            }

            // 8) Final report string
            string report = sb.ToString();

            ReportsManager.SaveReport("MemoryInstructions", report);
        }

        [Fact]
        public void TestUniqueInstructionsReport()
        {
            string uniqueInstructions = DebugInstructionSamples.GenerateUniqueInstructionsList();

            // 1) Parse CSV and group by opcode
            var entries = uniqueInstructions
                .Split([ "\r\n", "\r", "\n" ], StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var p = line.Split(',');
                    return new
                    {
                        Dec = int.Parse(p[0]),
                        Hex = p[1],
                        Name = p[2]
                    };
                })
                .ToList();

            var groups = entries
                .GroupBy(e => e.Dec)
                .OrderBy(g => g.Key)
                .ToList();

            // 2) Compute stats
            int n = groups.Count;       // unique opcodes
            int m = entries.Count;      // total variations

            // 3) Measure column widths
            int hexW = Math.Max("Hex".Length, groups.Max(g => g.First().Hex.Length));
            int nameW = Math.Max("(+ variations)".Length,
                                  groups.Max(g => string.Join(", ", g.Select(e => e.Name)).Length));

            // 4) Build format string (4‑wide decimal, padded left; then hex; then names)
            string fmt = $"{{0,4}}   {{1,-{hexW}}}   {{2,-{nameW}}}";
            string pre = $" Opcode       Instruction";

            var sb = new StringBuilder();

            // 5) Header and stats
            sb.AppendLine($"{n} instructions found (vacant opcodes: {256 - n})");
            sb.AppendLine($"{m} total variations");
            sb.AppendLine();
            sb.AppendLine(pre);
            sb.AppendLine(string.Format(fmt, "Dec", "Hex", "(+ variations)"));
            sb.AppendLine(new string('-', 4 + 3 + hexW + 3 + nameW));

            // 6) Rows
            int lastDec = -1;
            foreach (var g in groups)
            {
                // detect gap
                int gap = g.Key - lastDec - 1;
                if (gap > 0)
                {
                    // use “missing opcode(s)” for clarity
                    string label = gap == 1 ? "vacant opcode" : "vacant opcodes";
                    sb.AppendLine($"> {gap} {label}");
                }

                // then the normal data row
                var names = string.Join(", ", g.Select(e => e.Name));
                sb.AppendLine(string.Format(
                    fmt,
                    g.Key.ToString().PadLeft(4),
                    g.First().Hex,
                    names
                ));

                lastDec = g.Key;
            }

            string report = sb.ToString();

            ReportsManager.SaveReport("Opcode-Instructions", report);
        }

        [Fact]
        public void TestInstruction()
        {
            Assembler cp = new();
            using Computer computer = new();
            string instruction = "DIV F8, Y";

            computer.Clear();
            cp.Build(instruction);

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMemAt(0, compiled);

            ContinuumDebugger.DebugInstructionsCount = 1;
            ContinuumDebugger.RunAt(0, computer, true);
            string dissassembly = ContinuumDebugger.GetDissassembled().Trim();

            Assert.Equal(instruction, dissassembly);
        }

    }
}
