using ContinuumTools.Display.Sections;
using ContinuumTools.Display.Sections.Manager;
using ContinuumTools.Display.Views;
using ContinuumTools.Input._3D_View;
using ContinuumTools.Network;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ContinuumTools.Display
{
    public class Painter
    {
        private static RenderTarget2D VirtualScreenTarget;

        public static void Initialize()
        {
            VirtualScreenTarget = new RenderTarget2D(Renderer.GetGraphicsDevice(), 650, 365);
        }

        public static void Update()
        {
            SectionMain.Update();
            SectionVideo.Update();
        }

        public static void Draw()
        {
            Renderer.SetRenderTarget(VirtualScreenTarget);
            Renderer.GetGraphicsDevice().Clear(Color.Transparent);
            Renderer.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.Default
            );

            // Draw VirtualScreen to a backbuffer
            if (SectionManager.ActiveSection == "video")
                VirtualScreen.Draw();

            Renderer.End();

            Renderer.SetRenderTarget(null);

            Renderer.GetGraphicsDevice().Clear(Client.IsConnected() ? Colors.BackgroundConnected : Colors.BackgroundDisconnected);
            Renderer.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.Default
            );

            // Draw UI here
            SectionMain.Draw();
            SectionVideo.Draw();
            MenuView.Draw();
            StatusBarView.Draw();

            if (SectionManager.ActiveSection == "video")
                Renderer.Draw(VirtualScreenTarget, new Rectangle(8, 29, 650, 365), Color.White);

            Renderer.End();
        }
    }
}
