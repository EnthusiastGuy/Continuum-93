using Continuum93.Emulator;
using Continuum93.Emulator.AutoDocs;
using Continuum93.Emulator.AutoDocs.MetaInfo;
using Continuum93.Emulator.Mnemonics;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Emulator.AutoDocs
{
    public static class ASMMetaGenerator
    {
        public static string GenerateMetaInfo(bool brief = false)
        {
            string response = HTML.H1("Instructions list");

            List<ASMMeta> sortedAsmMetas = ASMMetaInfo.MetaData
                .OrderBy(asm => asm.Operators != null && asm.Operators.Count > 0 ? asm.Operators[0] : string.Empty)
                .ToList();

            int totalInstructions = 0;
            int totalVariations = 0;
            int sectionTotals = 0;

            foreach (ASMMeta asmMeta in sortedAsmMetas)
            {
                totalInstructions++;
                response += HTML.H2(asmMeta.Format);

                response += HTML.Table(
                    HTML.MetaRow("Description:", asmMeta.Description) +
                    HTML.MetaRow("Applications:", asmMeta.Application)
                );

                string subTable = "";

                if (!brief)
                {
                    foreach (KeyValuePair<string, Oper> op in Mnem.OPS)
                    {
                        string opMatch = op.Key.Split(" ")[0].Trim();
                        if (!op.Value.IsPrimary && asmMeta.Operators.Contains(opMatch))
                        {
                            string format = op.Value.Format;
                            byte SubOp = Mnem.GetSecondaryOpCode(op.Key).Value;
                            byte subOpBitWidth = (byte)DataConverter.CountCharInString('o', format);
                            string formatNoSpaces = string.Join("", format.Split(" "));

                            format = string.Join(
                                DataConverter.GetBinaryOfUint(SubOp, subOpBitWidth),
                                format.Split(new string('o', subOpBitWidth))
                            );
                            string compactFormatBits = string.Join("", format.Split(" "));

                            int instructionSize = 1 + (compactFormatBits.Length / 8);

                            string correctedFormat = ASMReferenceGenerator.GetCorrectedFormat(op.Key, format);

                            string mainOp = correctedFormat.Split(" ")[0];
                            string operands = correctedFormat.Split(" ")[1];

                            Oper parent = GetParent(mainOp);

                            formatNoSpaces = DataConverter.GetBinaryOfUint(Mnem.GetPrimaryOpCode(mainOp), 8) + formatNoSpaces;
                            compactFormatBits = DataConverter.GetBinaryOfUint(Mnem.GetPrimaryOpCode(mainOp), 8) + compactFormatBits;

                            string[] operandList = operands.Split(",");

                            for (byte i = 0; i < operandList.Length; i++)
                            {
                                operandList[i] = ASMReferenceGenerator.GetOperandDescription(operandList[i], i);
                            }

                            string description = "";

                            for (byte i = 0; i < parent.Description.Length; i++)
                            {
                                if (DocUtils.CountFormatArguments(parent.Description[i]) == operandList.Length)
                                {
                                    description = string.Format(parent.Description[i], operandList);
                                    break;
                                }
                            }

                            totalVariations++;
                            sectionTotals++;
                            subTable += GetSubOpInfo(op.Value, description);
                        }
                    }
                }
                
                if (subTable != "")
                {
                    response += HTML.P(
                        HTML.B($"Specific instructions ({sectionTotals})")
                    ) + HTML.Table(subTable);
                }

                response += HTML.HR;


                sectionTotals = 0;
            }

            response += "<br />" + HTML.Table(
                HTML.MetaRow("Total instructions", $"{totalInstructions}") +
                HTML.MetaRow("Total variations", $"{totalVariations}")
            );

            return HTML.HTMLRoot("Assembly instructions info", response, "stylesInfo.css");
        }

        private static Oper GetParent(string mnem)
        {
            foreach (KeyValuePair<string, Oper> op in Mnem.OPS)
            {
                string opMatch = op.Key.Split(" ")[0].Trim();
                if (op.Value.IsPrimary && opMatch.Equals(mnem))
                {
                    return op.Value;
                }
            }

            return null;
        }

        private static string GetSubOpInfo(Oper oper, string description)
        {
            string response = "";

            response += HTML.MetaStyledRow(oper.Mnemonic, description);

            return response;
        }
    }
}
