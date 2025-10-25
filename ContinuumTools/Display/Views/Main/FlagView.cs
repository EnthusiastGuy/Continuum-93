using ContinuumTools.Display.Components;
using ContinuumTools.Display.ViewsUtils;
using ContinuumTools.States;
using Last_Known_Reality.Reality;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static Last_Known_Reality.Reality.GameContent;

namespace ContinuumTools.Display.Views.Main
{
    public static class FlagView
    {
        static readonly int VERTICAL_OFFSET = 248;
        static readonly int HORIZONTAL_OFFSET = 960;
        static readonly Vector2 REG_TITLE_POINT_A = new(960, Constants.TITLE_TOP_PADDING + VERTICAL_OFFSET);
        static readonly Vector2 REG_TITLE_POINT_B = new(1200, Constants.TITLE_TOP_PADDING + VERTICAL_OFFSET);

        static List<FlagLabel> flagsLabel = new();

        private static bool _flagsUpdated = false;

        public static void UpdateFlags()
        {
            _flagsUpdated = false;
        }

        public static void Update()
        {
            if (!_flagsUpdated)
            {
                flagsLabel.Clear();

                FlagLabel.OriginX = HORIZONTAL_OFFSET;
                FlagLabel.OriginY = VERTICAL_OFFSET - 4;

                for (byte i = 0; i < 8; i++)
                {
                    flagsLabel.Add(new FlagLabel()
                    {
                        Font = Fonts.SparkleMedium,
                        FlagIndex = i,
                        FlagValue = CPUState.GetBitValue(CPUState.Flags, i),
                        OldFlagValue = CPUState.GetBitValue(CPUState.OldFlags, i)
                    });
                }

                _flagsUpdated = true;
            }
        }

        public static void Draw()
        {
            Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium,
                $"Flags",
                HORIZONTAL_OFFSET,
                Constants.TITLE_TOP_PADDING + VERTICAL_OFFSET - 16, Colors.TitleIdle);
            Renderer.DrawLine(REG_TITLE_POINT_A, REG_TITLE_POINT_B, Colors.TitleIdle);

            foreach (FlagLabel label in flagsLabel)
            {
                label.Draw();
            }
        }
    }
}
