using ContinuumTools.Display.Components;
using ContinuumTools.Display.ViewsUtils;
using ContinuumTools.States;
using ContinuumTools.Utils;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace ContinuumTools.Display.Views.Main
{
    public static class RegisterView
    {
        private static bool _regsUpdated = false;

        static Vector2 REG_TITLE_POINT_A = new(Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 24);
        static Vector2 REG_TITLE_POINT_B = new(Constants.MARGIN_LEFT + 400, Constants.TITLE_TOP_PADDING + 24);
        static List<HexLabel> labels = new();


        //static byte[] POSITIONS = new byte[] { 0, 50, 121, 212};
        //    A AB  ABC  ABCD
        //static byte[] POSITIONS = new byte[] { 0, 35, 94, 255 };
        static byte[] POSITIONS = new byte[] { 103, 136, 192, 0 };

        public static void UpdateRegs()
        {
            _regsUpdated = false;
        }

        public static void Update()
        {
            if (!_regsUpdated)
            {
                labels.Clear();

                for (int i = 0; i < 26; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        labels.Add(
                            new HexLabel()
                            {
                                Font = GameContent.Fonts.SparkleMedium,
                                Area = new Rectangle(Constants.MARGIN_LEFT + Constants.REGS_LEFT_PADDING + POSITIONS[j], Constants.REGS_TOP_PADDING + i * 12, 0, 0),
                                Text = $"{RegistryViewsUtils.GetNBitRegisterName(i, 1 + j)}:{RegistryViewsUtils.GetHexValueForNBitRegister(i, 1 + j, CPUState.RegPage0)}",
                                CharacterColor = new Color[] { RegistryViewsUtils.GetRegisterStateColor(i) },
                                Register = RegistryViewsUtils.GetNBitRegisterName(i, 1 + j),
                                Value = RegistryViewsUtils.GetDecimalValueForNBitRegister(i, 1 + j, CPUState.RegPage0),
                                HexValue = RegistryViewsUtils.GetHexValueForNBitRegister(i, 1 + j, CPUState.RegPage0),
                                OldHexValue = RegistryViewsUtils.GetHexValueForNBitRegister(i, 1 + j, CPUState.RegPageOld),
                            }
                        );
                    }
                }

                _regsUpdated = true;
            }
        }

        public static void Draw()
        {
            Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium,
                $"Register view (bank: {CPUState.RegPageIndex})",
                Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 8,
                UserInput.HoveringRegistryViewer ? Colors.TitleHovered : Colors.TitleIdle);
            Renderer.DrawLine(REG_TITLE_POINT_A, REG_TITLE_POINT_B, Colors.TitleIdle);

            foreach (HexLabel label in labels)
            {
                label.Draw();
            }

            if (UserInput.HoveringRegistryViewer && UserInput.Hovering24bitRegister)
            {
                // Marks the hovering line
                Renderer.DrawLine(
                    new Vector2(745, Constants.REGS_TOP_PADDING + UserInput.Hovered24BitRegisterLine * 12 + 6),
                    new Vector2(747, Constants.REGS_TOP_PADDING + UserInput.Hovered24BitRegisterLine * 12 + 6),
                    Colors.TitleHovered,
                    12
                );

                Renderer.DrawLine(
                    new Vector2(747, Constants.REGS_TOP_PADDING + UserInput.Hovered24BitRegisterLine * 12 + 11),
                    new Vector2(825, Constants.REGS_TOP_PADDING + UserInput.Hovered24BitRegisterLine * 12 + 11),
                    Colors.TitleHovered,
                    1
                );
            }

            for (int i = 0; i < 26; i++)
            {
                Renderer.DrawMonospaced(
                        GameContent.Fonts.SparkleMedium,
                        ">",
                        Constants.MARGIN_LEFT + Constants.REGS_LEFT_PADDING + 272, Constants.REGS_TOP_PADDING + i * 12,
                        Color.White,
                        -4
                    );

                if (UserInput.ShowAsciiRegReferencedData)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        Renderer.DrawMonospaced(
                            GameContent.Fonts.SparkleMedium,
                            "" + (char)CPUState.RegMemoryData[i * 16 + j],
                            Constants.MARGIN_LEFT + Constants.REGS_LEFT_PADDING + 281 + j * 7, Constants.REGS_TOP_PADDING + i * 12,
                            UserInput.HoveringRegistryViewer && UserInput.HoveringRegAddressedData ? Colors.TextASCIIHovered : Colors.TextASCII,
                            -4
                        );
                    }
                }
                else
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Renderer.DrawMonospaced(
                            GameContent.Fonts.SparkleMedium,
                            StringUtils.ByteToHex(CPUState.RegMemoryData[i * 16 + j]),
                            Constants.MARGIN_LEFT + Constants.REGS_LEFT_PADDING + 281 + j * 17, Constants.REGS_TOP_PADDING + i * 12,
                            UserInput.HoveringRegistryViewer && UserInput.HoveringRegAddressedData ? Colors.MemoryByteHovered : Colors.MemoryByte,
                            -4
                        );
                    }
                }
            }
        }
    }
}
