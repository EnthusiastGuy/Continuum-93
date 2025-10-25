using ContinuumTools.Network;
using ContinuumTools.States;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ContinuumTools.Display.Views
{
    public static class StatusBarView
    {
        private static string _connectedStatus;
        private static Color _connectionColor;
        private static string _stepByStepStatus;
        private static Color _stepByStepColor;


        public static void Update()
        {
            _connectionColor = Client.IsConnected() ?
                Color.GreenYellow :
                Color.IndianRed;

            _connectedStatus = Client.IsConnected() ?
                "Connected" :
                "No signal";

            _stepByStepColor = CPUState.StepByStepMode ?
                Color.GreenYellow :
                Color.AliceBlue;

            _stepByStepStatus = Client.IsConnected() ? CPUState.StepByStepMode ?
                "CPU debugging" :
                "CPU running" :
                "CPU unknown";
        }

        public static void Draw()
        {
            Renderer.DrawLine(
                new Vector2(0, Renderer.CanvasHeight - 9),
                new Vector2(Renderer.CanvasWidth, Renderer.CanvasHeight - 9),
                Colors.MenuBackground,
                18
            );

            Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, "HISTORY", 6, Renderer.CanvasHeight - 15, Colors.TitleIdle);

            int currentX = 64;

            // Show History
            int width;

            List<DissLine> latestHistory = StepHistory.GetAtMost(6);

            for (int i = 0; i < latestHistory.Count; i++)
            {
                width = GameContent.Fonts.SparkleMedium.MeasureStringWidth(latestHistory[i].Instruction) + 4;
                Renderer.DrawRectangle(currentX, (int)Renderer.CanvasHeight - 16, width, 14, Colors.HistoryInstructionBackground);

                Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, latestHistory[i].Instruction, currentX + 2, Renderer.CanvasHeight - 15, Color.AliceBlue);
                currentX += width + 2;
                Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, ">", currentX, Renderer.CanvasHeight - 15, Color.AliceBlue);
                currentX += 8;
            }

            DissLine current = Disassembled.GetCurentInstruction();
            if ( current != null )
            {
                width = GameContent.Fonts.SparkleMedium.MeasureStringWidth(current.Instruction) + 4;
                Renderer.DrawLine(
                    new Vector2(currentX, Renderer.CanvasHeight - 9),
                    new Vector2(currentX + width, Renderer.CanvasHeight - 9),
                    Colors.CurrentInstructionBackground,
                    14
                );

                Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, current.Instruction, currentX + 2, Renderer.CanvasHeight - 15, Color.DarkOrange);
            }

            // Watermark
            Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, _connectedStatus, Renderer.CanvasWidth - 64, Renderer.CanvasHeight - 15, _connectionColor);
            Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, _stepByStepStatus, Renderer.CanvasWidth - 164, Renderer.CanvasHeight - 15, _stepByStepColor);
        }
    }
}
