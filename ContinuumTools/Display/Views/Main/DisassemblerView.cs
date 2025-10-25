using ContinuumTools.States;
using ContinuumTools.Utils;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ContinuumTools.Display.Views.Main
{
    public static class DisassemblerView
    {
        private static bool _dissUpdated = false;

        static Vector2 DIS_TITLE_POINT_A = new(Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 24);
        static Vector2 DIS_TITLE_POINT_B = new(543, Constants.TITLE_TOP_PADDING + 24);

        static List<DissLine> lines = new();

        public static void Update()
        {
            if (!_dissUpdated)
            {
                lines = new(Disassembled.Lines);
                _dissUpdated = true;
            }
        }

        public static void UpdateDiss()
        {
            _dissUpdated = false;
        }

        public static void Draw()
        {
            Renderer.DrawStringPlusHiRes(
                GameContent.Fonts.SparkleMedium,
                $"Disassembler  ({StringUtils.IntToHex(CPUState.IPAddress)})",
                Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 8,
                UserInput.HoveringDisassembler ? Colors.TitleHovered : Colors.TitleIdle);

            Renderer.DrawLine(DIS_TITLE_POINT_A, DIS_TITLE_POINT_B, Colors.TitleIdle);

            for (int i = 0; i < lines.Count; i++)
            {
                if (CPUState.IPAddress == lines[i].Address)
                {
                    // Mark a selection here. This is the current instruction
                    Renderer.DrawLine(
                        new Vector2(Constants.TITLE_LEFT_PADDING, Constants.REGS_TOP_PADDING + i * 12 + 6),
                        new Vector2(Constants.TITLE_LEFT_PADDING + 539, Constants.REGS_TOP_PADDING + i * 12 + 6),
                        Color.DarkCyan,
                        12
                    );
                }

                bool hoveringAddress = UserInput.HoveringDisassembler && UserInput.HoveredDisassemblerLine == i && UserInput.HoveringAddress;
                bool hoveringInstruction = UserInput.HoveringDisassembler && UserInput.HoveredDisassemblerLine == i && UserInput.HoveringInstruction;

                if (UserInput.HoveringDisassembler && UserInput.HoveredDisassemblerLine == i)
                {
                    // Marks the hovering line
                    Renderer.DrawLine(
                        new Vector2(2, Constants.REGS_TOP_PADDING + i * 12 + 6),
                        new Vector2(8, Constants.REGS_TOP_PADDING + i * 12 + 6),
                        Colors.TitleHovered,
                        12
                    );

                    Renderer.DrawLine(
                        new Vector2(8, Constants.REGS_TOP_PADDING + i * 12 + 11),
                        new Vector2(543, Constants.REGS_TOP_PADDING + i * 12 + 11),
                        Colors.TitleHovered,
                        1
                    );
                }

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    $"{lines[i].TextAddress}",
                    Constants.REGS_LEFT_PADDING, Constants.REGS_TOP_PADDING + i * 12,
                    hoveringAddress ? Colors.TitleHovered : Color.White,
                    -4
                );

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    $"{lines[i].OpCodes}",
                    Constants.REGS_LEFT_PADDING + 60, Constants.REGS_TOP_PADDING + i * 12,
                    Colors.MemoryByte,
                    -4
                );

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    $"{lines[i].Instruction}",
                    Constants.REGS_LEFT_PADDING + 300, Constants.REGS_TOP_PADDING + i * 12,
                    hoveringInstruction ? Colors.TitleHovered : Color.Aquamarine,
                    -4
                );
            }
        }
    }
}
