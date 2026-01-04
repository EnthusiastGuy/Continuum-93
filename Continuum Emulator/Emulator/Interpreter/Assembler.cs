using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Continuum93.Emulator.Interpreter
{
    public class Assembler
    {
        private readonly List<CLine> _lines = [];
        public AssemblerStats Stats = new();
        public CodeBlockManager BlockManager = new();
        public string Log = "";
        public string FullSource = "";
        public int Errors;

        public CLine GetCompiledLine(int line)
        {
            if (line < 0 || line >= _lines.Count)
                return null;

            return _lines[line];
        }

        public void Build(string source, string path = "")
        {
            BlockManager.Reset();
            Stats.ClearStaticData();
            _lines.Clear();
            Errors = 0;

            string[] sLines = Interpret.GetFullSourceLines(source, Path.GetDirectoryName(path));

            FullSource = string.Join(Environment.NewLine, sLines);

            CompileLog.Log(string.Format("Found {0} lines to compile.", sLines.Count()), CompileIssue.Info);

            Stats.Labels = Interpret.GetAllDefinedLabels(sLines);

            for (uint i = 0; i < sLines.Length; i++)
            {
                CLine currentLine = new(i + 1, sLines[i], BlockManager, Stats);
                if (currentLine.Error != null)
                {
                    Errors++;
                }
                _lines.Add(currentLine);
            }

            for (int i = 0; i < _lines.Count; i++)
            {
                _lines[i].UpdateLabelAddress();
                _lines[i].RefreshDBArguments();
            }

            int tCodeLen = 0;

            for (int i = 0; i < BlockManager.BlocksCount(); i++)
            {
                byte[] data = GetCompiledCodeBlock(i + 1);
                BlockManager.AddDataToBlockId(data, i);
                tCodeLen += data.Length;
            }

            CompileLog.Log(string.Format("Total program length is {0} bytes out of which {1} bytes represent actual code and {2} bytes represent #DB data.", tCodeLen, tCodeLen - Stats.DBDataLength, Stats.DBDataLength), CompileIssue.Info);

            BlockManager.Validate();
        }

        public byte[] GetCompiledCode()
        {
            byte[] code = [];

            for (int i = 0; i < _lines.Count; i++)
            {
                if (!_lines[i].PreventCompiling || _lines[i].Error != null)
                    code = DataConverter.MergeByteArrays(code, _lines[i].Data.ToArray());
            }

            return code;
        }

        public uint GetRunAddress()
        {
            return Stats.RunAddress;
        }

        public bool IsRunAddressFromOrg()
        {
            return Stats.RunFromOrg;
        }

        public int GetCompiledBlocksCount()
        {
            return BlockManager.BlocksCount();
        }

        public byte[] GetCompiledCodeBlock(int blockId)
        {
            byte[] code = Array.Empty<byte>();

            for (int i = 0; i < _lines.Count; i++)
            {
                if (_lines[i].BlockIndex == blockId && (!_lines[i].PreventCompiling || _lines[i].Error != null))
                    code = DataConverter.MergeByteArrays(code, _lines[i].Data.ToArray());
            }

            return code;
        }
    }

    public class AssemblerStats
    {
        public uint GlobalAddressPointer = 0;
        public uint OrgAddress = 0;
        public uint RunAddress = 0;
        public bool RunFromOrg = false;  // Whether the provided run address came from an org directive
        public Dictionary<string, uint> Labels = [];
        public int DBDataLength = 0;     // Statistical data on how many bytes the DB directives took

        public void ClearStaticData()
        {
            GlobalAddressPointer = 0;
            OrgAddress = 0;
            RunAddress = 0;
            DBDataLength = 0;
            RunFromOrg = false;
            Labels.Clear();
        }
    }

    public class CLine
    {
        static readonly string[] validSingleOperators = { "NOP", "DEBUG", "BREAK", "RET", "" };

        // To allow labels to be read by these instructions
        static readonly HashSet<string> globalAddressingOperations =
        [
            "CALL", "JP", "MEMC", "MEMF",
            "DJNZ", "DJNZ16", "DJNZ24", "DJNZ32",
            "CP", "LD",
            "ADD",
            "SUB",
            "DIV", "MUL",
            "FIND",
            "ISQR", "POW", "SQR",
            "REGS", "FREGS",
            "PUSH", "PUSH16", "PUSH24", "PUSH32",
            "POP", "POP16", "POP24", "POP32",
            "VCL", "VDL",
            "LDF", "LDREGS", "STREGS",
            "AND", "AND16", "AND24", "AND32",
            "OR", "OR16", "OR24", "OR32",
            "XOR", "XOR16", "XOR24", "XOR32",
            "NAND", "NAND16", "NAND24", "NAND32",
            "NOR", "NOR16", "NOR24", "NOR32",
            "XNOR", "XNOR16", "XNOR24", "XNOR32",
            "IMPLY", "IMPLY16", "IMPLY24", "IMPLY32",
            "INC", "INC16", "INC24", "INC32",
            "DEC", "DEC16", "DEC24", "DEC32",
            "RGB2HSL", "HSL2RGB", "RGB2HSB", "HSB2RGB",
            "PLAY"
        ];

        private readonly AssemblerStats _stats;

        public uint BlockOrg = 0;
        public int BlockIndex = 0;  // Compile block. Used for multiple #ORG directives
        public uint LineNr;
        public bool IsDirective;
        public bool PreventCompiling;
        public string Text;
        public uint Address;
        public bool LabelRelative;

        public string Label;
        public string Mnemonic;
        public string Directive;
        public string Comment;

        public string Op;
        public byte? SubOp;
        public string GeneralForm;
        public string TextualArgumentsUpperCase;
        public string TextualArguments;
        public List<AsmArgument> Arguments = [];
        public List<byte> Data = [];
        public string Error;

        private readonly List<DBArgument> dbArguments = new();
        private readonly CodeBlockManager _blockManager;

        public CLine(uint lineNr, string text, CodeBlockManager codeBlockManager, AssemblerStats stats)
        {
            _blockManager = codeBlockManager;
            _stats = stats;
            LineNr = lineNr;
            Text = text;
            if (text != "")
                Process();
        }

        public void Process()
        {
            Address = _stats.GlobalAddressPointer;
            Comment = Interpret.GetCommentFromLine(Text);
            Label = Interpret.GetLabelFromLine(Text);

            if (!string.IsNullOrEmpty(Label) && _stats.Labels.ContainsKey(Label))
                _stats.Labels[Label] = _stats.GlobalAddressPointer;   // Assigns the value of the current instruction line address

            Mnemonic = Interpret.GetMnemonicFromLine(Text);

            IsDirective = Interpret.IsCompilerDirective(Mnemonic);
            TextualArguments = Interpret.GetArguments(Mnemonic);
            TextualArgumentsUpperCase = TextualArguments.ToUpper();

            Op = Interpret.GetOp(Mnemonic);

            if (IsDirective)
            {
                Directive = Interpret.GetOp(Mnemonic);
                ResolveCompilerDirectives();
                BlockOrg = _stats.OrgAddress;
                BlockIndex = _blockManager.BlocksCount();
                return;
            }

            if (!Mnem.PrimaryOpExists(Op) && Op.Trim() != "")
            {
                CompileLog.Log(string.Format("Unknown mnemonic {0} found at line {1}.", Op, LineNr), CompileIssue.Warning);

            }

            BlockOrg = _stats.OrgAddress;

            ResolveArguments();
            ResolveDataOpcodes();
            BlockIndex = _blockManager.BlocksCount();
        }

        public void ResolveArguments()
        {
            if (TextualArguments == null)
                return;

            string[] args = SplitOperands(TextualArguments);

            foreach (string arg in args)
                IdentifyArgument(arg.Trim());

            // Instructions that use the shared Instructions sub-op addressing matrix.
            // These require some light normalization of numeric argument “general forms”
            // (e.g. nnn -> n/nn/nnn/nnnn depending on destination width) so the lookup
            // matches the GenericInitializer-registered variants.
            if (Op == "LD" || Op == "ADD" || Op == "SUB" || Op == "DIV") // temporary
            {
                if (Arguments.Count == 2)
                {
                    if (Arguments[0].GeneralForm == "fr" && Arguments[1].GeneralForm == "nnn")
                    {
                        Arguments[1].GeneralForm = "nnnn";
                    }
                    else if (Arguments[0].GeneralForm == "r" && Arguments[1].GeneralForm == "nnn")
                    {
                        Arguments[1].GeneralForm = "n";
                    }
                    else if (Arguments[0].GeneralForm == "rr" && Arguments[1].GeneralForm == "nnn")
                    {
                        Arguments[1].GeneralForm = "nn";
                    }
                    else if (Arguments[0].GeneralForm == "rrr" && Arguments[1].GeneralForm == "nnn")
                    {
                        Arguments[1].GeneralForm = "nnn";
                    }
                    else if (Arguments[0].GeneralForm == "rrrr" && Arguments[1].GeneralForm == "nnn")
                    {
                        Arguments[1].GeneralForm = "nnnn";
                    }
                }
                else if (Arguments.Count == 3)
                {
                    if ((Arguments[0].GeneralForm == "(nnn)" || Arguments[0].GeneralForm == "(rrr)") && Arguments[1].GeneralForm == "nnn" && Arguments[2].GeneralForm == "nnn")
                    {
                        Arguments[1].GeneralForm = "nnnn";
                        Arguments[2].GeneralForm = "n";
                    }
                }
                else if (Arguments.Count == 4)
                {
                    if ((Arguments[0].GeneralForm == "(nnn)" || Arguments[0].GeneralForm == "(rrr)") && Arguments[1].GeneralForm == "nnn" && Arguments[2].GeneralForm == "nnn" && (Arguments[3].GeneralForm == "nnn" || Arguments[3].GeneralForm == "rrr"))
                    {
                        Arguments[1].GeneralForm = "nnnn";
                        Arguments[2].GeneralForm = "n";
                    }
                    else if ((Arguments[0].GeneralForm == "(nnn)" || Arguments[0].GeneralForm == "(rrr)") && (Arguments[1].GeneralForm == "(nnn)" || Arguments[1].GeneralForm == "(rrr)") && Arguments[2].GeneralForm == "nnn" && Arguments[3].GeneralForm == "rrr")
                    {
                        Arguments[2].GeneralForm = "n";
                    }
                    else if ((Arguments[0].GeneralForm == "(nnn" || Arguments[0].GeneralForm == "(rrr") && (Arguments[1].GeneralForm == "nnn)" || Arguments[1].GeneralForm == "r)" || Arguments[1].GeneralForm == "rr)" || Arguments[1].GeneralForm == "rrr)") && Arguments[2].GeneralForm == "nnn" && Arguments[3].GeneralForm == "nnn")
                    {
                        Arguments[2].GeneralForm = "nnnn";
                        Arguments[3].GeneralForm = "n";
                    }
                }
                else if (Arguments.Count == 5)
                {
                    if ((Arguments[0].GeneralForm == "(nnn)" || Arguments[0].GeneralForm == "(rrr)") && (Arguments[1].GeneralForm == "(nnn" || Arguments[1].GeneralForm == "(rrr") && (Arguments[2].GeneralForm == "r)" || Arguments[2].GeneralForm == "rr)" || Arguments[2].GeneralForm == "rrr)" || Arguments[2].GeneralForm == "nnn)") && Arguments[3].GeneralForm == "nnn" && Arguments[4].GeneralForm == "rrr")
                    {
                        Arguments[3].GeneralForm = "n";
                    }
                    else if ((Arguments[0].GeneralForm == "(nnn" || Arguments[0].GeneralForm == "(rrr") && (Arguments[1].GeneralForm == "nnn)" || Arguments[1].GeneralForm == "r)" || Arguments[1].GeneralForm == "rr)" || Arguments[1].GeneralForm == "rrr)") && Arguments[2].GeneralForm == "nnn" && Arguments[3].GeneralForm == "nnn" && Arguments[4].GeneralForm == "nnn")
                    {
                        Arguments[2].GeneralForm = "nnnn";
                        Arguments[3].GeneralForm = "n";
                    }
                    else if ((Arguments[0].GeneralForm == "(nnn" || Arguments[0].GeneralForm == "(rrr") && (Arguments[1].GeneralForm == "nnn)" || Arguments[1].GeneralForm == "r)" || Arguments[1].GeneralForm == "rr)" || Arguments[1].GeneralForm == "rrr)") && (Arguments[2].GeneralForm == "(nnn)" || Arguments[2].GeneralForm == "(rrr)") && Arguments[3].GeneralForm == "nnn" && Arguments[4].GeneralForm == "rrr")
                    {
                        Arguments[3].GeneralForm = "n";
                    }
                }
                else if (Arguments.Count == 6)
                {
                    if ((Arguments[0].GeneralForm == "(nnn" || Arguments[0].GeneralForm == "(rrr") && (Arguments[1].GeneralForm == "nnn)" || Arguments[1].GeneralForm == "r)" || Arguments[1].GeneralForm == "rr)" || Arguments[1].GeneralForm == "rrr)") && (Arguments[2].GeneralForm == "(nnn" || Arguments[2].GeneralForm == "(rrr") && (Arguments[3].GeneralForm == "nnn)" || Arguments[3].GeneralForm == "r)" || Arguments[3].GeneralForm == "rr)" || Arguments[3].GeneralForm == "rrr)") && Arguments[4].GeneralForm == "nnn" && Arguments[5].GeneralForm == "rrr")
                    {
                        Arguments[4].GeneralForm = "n";
                    }

                }
            }

            

            var generalList = from item in Arguments
                              let values = new string[] { item.GeneralForm }
                              from x in values
                              select x;

            GeneralForm = Op + " " + string.Join(",", generalList); // Example LD rr,nnn
        }

        public static string[] SplitOperands(string input)
        {
            // 1) remove all spaces
            var noSpaces = Regex.Replace(input, @"\s+", "");

            // 2) grab tokens with optional leading +/-, stopping at the next +, - or comma
            var matches = Regex.Matches(noSpaces, @"[+\-]?[^+,\-]+");

            // 3) extract the values
            return matches
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
        }

        public void IdentifyArgument(string arg)
        {
            if (InterpretArgument.IsFlagUsingOperator(Op))
            {
                if (InterpretArgument.HasLabel(arg))
                {
                    Arguments.Add(AsmArgument.GetLabelArgument(arg, InterpretArgument.HasRelativeLabel(arg)));
                    return;
                }
                else if (InterpretArgument.IsValidFlag(arg.ToUpper()))
                {
                    Arguments.Add(AsmArgument.GetFlagArgument(Flags.GetFlagIndexByName(arg.ToUpper())));
                    return;
                }
                else
                {
                    Arguments.Add(
                        IdentifyValueOrReg(InterpretArgument.ExtractAddressingArgument(arg.ToUpper()),
                        InterpretArgument.IsAdddressingArgument(arg.ToUpper()))
                    );
                }
            }
            else
            {
                if (InterpretArgument.HasLabel(arg) && InterpretArgument.IsNotFloatNumber(arg))
                {
                    if (InterpretArgument.IsAdddressingArgument(arg))
                    {
                        Arguments.Add(
                            new AsmArgument()
                            {
                                Addressing = true,
                                GeneralForm = "(nnn)",
                                PendingLabel = InterpretArgument.ExtractAddressingArgument(arg)
                            }
                        );
                    }
                    else if (InterpretArgument.IsBeginningOfAddressingArgument(arg))
                    {
                        Arguments.Add(
                            new AsmArgument()
                            {
                                Addressing = true,
                                GeneralForm = "(nnn",
                                PendingLabel = InterpretArgument.ExtractAddressingArgument(arg)
                            }
                        );
                    }
                    else if (InterpretArgument.IsEndOfAddressingArgument(arg))
                    {
                        Arguments.Add(
                            new AsmArgument()
                            {
                                Addressing = true,
                                GeneralForm = "nnn)",
                                PendingLabel = InterpretArgument.ExtractAddressingArgument(arg)
                            }
                        );
                    }
                    else
                    {
                        Arguments.Add(AsmArgument.GetLabelArgument(arg, false));
                    }
                }
                else
                {
                    Arguments.Add(
                        IdentifyValueOrReg(
                            InterpretArgument.ExtractAddressingArgument(arg.ToUpper()),
                            InterpretArgument.IsAdddressingArgument(arg.ToUpper()),
                            InterpretArgument.IsBeginningOfAddressingArgument(arg.ToUpper()),
                            InterpretArgument.IsEndOfAddressingArgument(arg.ToUpper())
                        ));
                }
            }
        }

        public void UpdateLabelAddress()
        {
            if (string.IsNullOrEmpty(Op))
                return;

            bool hasAnyPendingLabels = false;

            for (int i = 0; i < Arguments.Count; i++)
            {
                hasAnyPendingLabels |= Arguments[i].HasPendingLabel();
            }

            if (!string.IsNullOrEmpty(Op) && Op.Equals("#DB"))
            {
                //ResolveCompilerDirectives();
                //ResolveDataOpcodes();
            }
            else if (hasAnyPendingLabels)
            {
                if (globalAddressingOperations.Contains(Op))   // Absolute addressing instructions
                {
                    for (int i = 0; i < Arguments.Count; i++)
                    {
                        if (Arguments[i].PendingLabel != null)
                        {
                            if (_stats.Labels.ContainsKey(Arguments[i].PendingLabel.Replace('~', '.')))
                            {
                                Arguments[i].Value = _stats.Labels[Arguments[i].PendingLabel];
                            }
                            else
                            {
                                CompileLog.Log($"Label {Arguments[i].PendingLabel} was not declared.", CompileIssue.Error);
                            }
                        }
                    }
                }
                else if (Op.Equals("CALLR") || Op.Equals("JR"))  // Relative calls
                {
                    for (int i = 0; i < Arguments.Count; i++)
                    {
                        if (Arguments[i].PendingLabel != null)
                        {
                            if (_stats.Labels.ContainsKey(Arguments[i].PendingLabel.Replace('~', '.')))
                            {
                                Arguments[i].Value = _stats.Labels[Arguments[i].PendingLabel.Replace('~', '.')] - Address;
                            }
                            else
                            {
                                CompileLog.Log($"Label {Arguments[i].PendingLabel} was not declared.", CompileIssue.Error);
                            }
                        }
                    }
                }

                ResolveDataOpcodes();
            }
        }

        public void ResolveDataOpcodes()
        {
            Data.Clear();
            if (!string.IsNullOrEmpty(Op))
                Data.Add(Mnem.GetPrimaryOpCode(Op));

            if (GeneralForm != null)
                SubOp = Mnem.GetSecondaryOpCode(GeneralForm);

            if (SubOp != null)
            {
                string format = DataConverter.RemoveAllSpacesIn(Mnem.GetOpFormat(GeneralForm));
                format = format.Replace('u', '0');

                byte subOpBitWidth = (byte)DataConverter.CountCharInString('o', format);

                format = string.Join(
                    DataConverter.GetBinaryOfUint((uint)SubOp, subOpBitWidth),
                    format.Split(new string('o', subOpBitWidth))
                );

                for (int i = 0; i < Arguments.Count; i++)
                {
                    char argPointer = 'A';
                    argPointer += (char)i;  // i = 0, arg = A; i = 1, arg = B etc...
                    byte bits = (byte)DataConverter.CountCharInString(argPointer, format);

                    string replace = new(argPointer, bits);

                    uint value = 0;
                    if (Arguments[i].Value.HasValue)
                    {
                        if (Arguments[i].isNegative)
                        {
                            int signed = (int)(-Arguments[i].Value.Value);      // e.g. –5
                            uint unsigned = unchecked((uint)signed);        // 0xFFFFFFFB

                            // (for bits=8, mask=0xFF; for 16→0xFFFF; for 32→0xFFFFFFFF)
                            uint mask = bits < 32
                                      ? (1u << bits) - 1
                                      : uint.MaxValue;
                            value = unsigned & mask;                       // e.g. 0xFB for 8 bits
                        }
                        else
                        {
                            value = Arguments[i].Value.Value;           // Numeric positive
                        }
                    }
                    else if (Arguments[i].RegIndex.HasValue)
                        value = Arguments[i].RegIndex.Value;        // Register index
                    else if (Arguments[i].FloatRegIndex.HasValue)
                        value = Arguments[i].FloatRegIndex.Value;   // Float register index
                    else if (Arguments[i].Flag.HasValue)
                        value = Arguments[i].Flag.Value;            // Flag index
                    else if (Arguments[i].FloatValue.HasValue)
                        value = FloatPointUtils.FloatToUint(Arguments[i].FloatValue.Value); // Floating point value

                    format = string.Join(DataConverter.GetBinaryOfUint(value, bits), format.Split(replace));
                }

                byte[] opCodes = DataConverter.GetBytesFromBinaryString(format);

                for (int i = 0; i < opCodes.Length; i++)
                    Data.Add(opCodes[i]);
            }
            else if (!validSingleOperators.Contains(GeneralForm.Trim()))
            {
                CompileLog.Log(string.Format("Unknown argument format {0} at line {1}. Found this: '{2}'", GeneralForm, LineNr, Text.Trim()), CompileIssue.Warning);
            }

            _stats.GlobalAddressPointer += (uint)Data.Count();
        }

        public AsmArgument IdentifyValueOrReg(string parameter, bool isAddressing = false, bool isBeginningAddressing = false, bool isEndAddressing = false)
        {
            string template = "{0}";
            if (isAddressing)
                template = "({0})";
            if (isBeginningAddressing)
                template = "({0}";
            else if (isEndAddressing)
                template = "{0})";

            string signRemovedParameter = parameter.Replace("+", "").Replace("-", "");
            bool isNegative = parameter.Contains("-");
            
            AsmArgument ar = new()
            {
                Addressing = isAddressing,
                Value = DataConverter.TryToGetValueOf(signRemovedParameter),
                isNegative = isNegative,
                FloatValue = DataConverter.TryToGetFloatValueOf(parameter),
            };

            if (ar.Value.HasValue || ar.FloatValue.HasValue)
            {
                ar.Text = string.Format(template, signRemovedParameter);
                ar.GeneralForm = string.Format(template, "nnn");    // TODO, determine decimal form...
                return ar;
            }

            // Is it an all purpose register?
            //string regCandidate = parameter.Replace("+", "").Replace("-", "");
            byte? reg = Mnem.TryGetRegisterIndex(signRemovedParameter);

            if (reg.HasValue)
            {
                ar.RegIndex = reg.Value;
                ar.Text = string.Format(template, signRemovedParameter);
                ar.GeneralForm = string.Format(template, new string('r', signRemovedParameter.Length));
                return ar;
            }

            // Is it a floating point register?
            byte? fReg = Mnem.TryGetFloatRegisterIndex(parameter);
            if (fReg.HasValue)
            {
                ar.FloatRegIndex = fReg.Value;
                ar.Text = string.Format(template, parameter);
                ar.GeneralForm = "fr";
                return ar;
            }

            // Or a special register? (SPC, SPR or IPO)
            if (Mnem.IsSpecialRegister(parameter))
            {
                ar.Text = string.Format(template, parameter);
                ar.GeneralForm = string.Format(template, parameter);
                return ar;
            }

            // Maybe a label. For that we need to finish parsing, just in case the label is defined forward on.
            if (DataConverter.IsLabelValid(parameter))
            {
                ar.Label = parameter;
                return ar;
            }

            // Giving up
            Error = string.Format("Unrecognized parameter nature -> '{0}'", parameter);
            return ar;  // .. such as it is
        }

        private void ResolveCompilerDirectives()
        {
            if (Directive == "#ORG")
            {
                PreventCompiling = true;
                uint? possibleValue = DataConverter.TryToGetValueOf(TextualArgumentsUpperCase);
                _stats.OrgAddress = possibleValue ?? _stats.GlobalAddressPointer;
                _stats.GlobalAddressPointer = _stats.OrgAddress;
                if (_stats.RunAddress == 0)
                {
                    _stats.RunAddress = _stats.OrgAddress;
                    _stats.RunFromOrg = true;
                }
                _blockManager.AddBlock(_stats.OrgAddress);
            }
            else if (Directive == "#RUN")
            {
                PreventCompiling = true;
                uint? possibleValue = DataConverter.TryToGetValueOf(TextualArgumentsUpperCase);
                if ((_stats.RunAddress != 0 && _stats.RunFromOrg) || _stats.RunAddress == 0)
                {
                    _stats.RunAddress = possibleValue ?? 0;
                    _stats.RunFromOrg = false;
                }
            }
            else if (Directive == "#DB")
            {
                dbArguments.Clear();
                string[] items = DataConverter.GetCSArguments(TextualArguments);

                for (int i = 0; i < items.Length; i++)
                    dbArguments.Add(new DBArgument(items[i], _stats));

                if (CompileLog.StopBuild)
                {
                    CompileLog.Log(string.Format("Compiler stopped at #DB handling for {0}", TextualArguments), CompileIssue.Info);
                }

                SetDBDataFromArguments();
                _stats.GlobalAddressPointer += (uint)Data.Count();
                _stats.DBDataLength += Data.Count();
            }
            else
            {
                CompileLog.Log(string.Format("Unknown directive {0} found at line {1}.", Directive, LineNr), CompileIssue.Error);
            }
        }

        public void RefreshDBArguments()
        {
            foreach (DBArgument argument in dbArguments)
            {
                if (argument.HasLabelData())
                    argument.RefreshLabelValue();
            }

            SetDBDataFromArguments();
        }

        public void SetDBDataFromArguments()
        {
            List<byte> data = new();

            foreach (DBArgument argument in dbArguments)
            {
                data.AddRange(argument.GetData());
            }

            if (data.Count > 0)
            {
                Data.Clear();
                Data.AddRange(data);
            }
        }

    }
    public class AsmArgument
    {
        public string Text;
        public uint? Value = null;
        public bool isNegative = false;
        public float? FloatValue = null;
        public byte? RegIndex = null;
        public byte? FloatRegIndex = null;
        public byte? Flag = null;
        public string Label = null;
        public string PendingLabel = null;
        public bool RelativeLabel = false;
        public string GeneralForm;
        public bool Addressing;

        public static AsmArgument GetUnknownLabelArgument()
        {
            return new AsmArgument() { Addressing = false, GeneralForm = "nnn" };
        }

        public static AsmArgument GetLabelArgument(string label, bool isRelative)
        {
            return new AsmArgument() { Addressing = false, GeneralForm = "nnn", PendingLabel = label };
        }

        public static AsmArgument GetFlagArgument(byte flagIndex)
        {
            return new AsmArgument() { Flag = flagIndex, GeneralForm = "ff" };
        }

        public AsmArgument()
        {
        }

        public bool HasPendingLabel()
        {
            return PendingLabel != null;
        }
    }

    public class Label
    {
        public string Text;
        public uint Address;
    }
}
