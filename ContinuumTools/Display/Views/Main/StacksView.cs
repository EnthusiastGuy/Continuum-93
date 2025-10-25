using ContinuumTools.States;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Views.Main
{
    public static class StacksView
    {
        static Vector2 REG_STACK_POINT_A = new(Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 344 + 24);
        static Vector2 REG_STACK_POINT_B = new(Constants.MARGIN_LEFT + 400, Constants.TITLE_TOP_PADDING + 344 + 24);

        public static void Update()
        {
            // Nothing to update yet
        }

        public static void Draw()
        {
            Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, "Stacks", Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 352,
                UserInput.HoveringStacksView ? Colors.TitleHovered : Colors.TitleIdle);
            Renderer.DrawLine(REG_STACK_POINT_A, REG_STACK_POINT_B, Colors.TitleIdle);
            Renderer.DrawMonospaced(GameContent.Fonts.SparkleMedium, "Regs", Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 372, Color.White, -4);
            Renderer.DrawMonospaced(GameContent.Fonts.SparkleMedium, "Calls", Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 444, Color.White, -4);

            for (int i = 0; i < Stacks.RegisterStack.Count; i++)
            {
                if (i >= 120)
                    break;

                int y = i / 20;
                int x = i % 20;

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    Stacks.RegisterStack[i],
                    Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING + 45 + x * 17, Constants.REGS_TOP_PADDING + 344 + y * 12,
                    Color.Aquamarine,
                    -4
                );
            }

            for (int i = 0; i < Stacks.CallStack.Count; i++)
            {
                if (i >= 64)
                    break;

                int y = i / 7;
                int x = i % 7;

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    Stacks.CallStack[i],
                    Constants.MARGIN_LEFT + Constants.TITLE_LEFT_PADDING + 45 + x * 49, Constants.REGS_TOP_PADDING + 416 + y * 12,
                    Color.Aquamarine,
                    -4
                );
            }
        }
    }
}
