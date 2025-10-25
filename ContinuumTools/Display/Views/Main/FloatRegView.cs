using ContinuumTools.Display.Components;
using ContinuumTools.Display.ViewsUtils;
using ContinuumTools.States;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ContinuumTools.Display.Views.Main
{
    public static class FloatRegView
    {
        static Vector2 REG_TITLE_POINT_A = new(960, Constants.TITLE_TOP_PADDING + 24);
        static Vector2 REG_TITLE_POINT_B = new(1200, Constants.TITLE_TOP_PADDING + 24);

        static List<FloatLabel> labels = new();

        private static bool _regsUpdated = false;

        public static void Update()
        {
            if (!_regsUpdated)
            {
                labels.Clear();

                for (byte i = 0; i < 16; i++)
                {
                    labels.Add(
                        new FloatLabel()
                        {
                            Font = GameContent.Fonts.SparkleMedium,
                            Area = new Rectangle(960, Constants.REGS_TOP_PADDING + i * 12, 0, 0),
                            //Text = $"{RegistryViewsUtils.GetNBitRegisterName(i, 1 + j)}:{RegistryViewsUtils.GetHexValueForNBitRegister(i, 1 + j, CPUState.RegPage0)}",
                            //CharacterColor = new Color[] { RegistryViewsUtils.GetRegisterStateColor(i) },
                            FloatValue = CPUState.FRegs[i],
                            OldFloatValue = CPUState.FRegsOld[i],
                            Register = RegistryViewsUtils.GetFloatRegisterRepresentation(i),
                        }
                    );
                }

                _regsUpdated = true;
            }
        }

        public static void UpdateRegs()
        {
            _regsUpdated = false;
        }

        public static void Draw()
        {
            Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium,
                $"Float register view",
                960, Constants.TITLE_TOP_PADDING + 8, Colors.TitleIdle);
            Renderer.DrawLine(REG_TITLE_POINT_A, REG_TITLE_POINT_B, Colors.TitleIdle);

            foreach (FloatLabel label in labels)
            {
                label.Draw();
            }
        }
    }
}
