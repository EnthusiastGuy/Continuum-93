using Continuum_Emulator.Emulator.Controls;
using ContinuumTools.Display;
using ContinuumTools.Input;
using ContinuumTools.Input._3D_View;
using ContinuumTools.Network;
using ContinuumTools.States;
using Last_Known_Reality;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;

namespace ContinuumTools
{
    public class ContinuumTools : Game
    {
        readonly Thread NetworkThread = new(() =>
        {
            try
            {
                Client.Start();
            }
            catch (Exception e)
            {
                Reporter.PushError($"NetworkThread caught: {e}");
            }
        })
        { Priority = ThreadPriority.Normal };

        public ContinuumTools()
        {
            Renderer.RegisterGraphicsDeviceManager(new GraphicsDeviceManager(this) {
                GraphicsProfile = GraphicsProfile.HiDef
            });
            GameContent.RegisterContentManager(Content);

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Painter.Initialize();
            GameTimePlus.Initialize();
            Config.Initialize();

            InputKeyboard.Initialize();
            InputMouse.Initialize();

            Renderer.InitializeSpriteBatch();
            Renderer.RegisterGraphics();
            WindowManager.Initialize(Window); 

            NetworkThread.Start();
            Protocol.Ping();

            VirtualScreen.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GameContent.Load();
        }

        protected override void Update(GameTime gameTime)
        {
            GameTimePlus.Update(gameTime);
            Protocol.Update();
            CPUState.Update();

            InputKeyboard.Update();
            InputMouse.Update();

            if (IsActive)
            {
                VirtualScreen.UpdateInput();
                EvaluateViewsInput.Update();

                if (InputKeyboard.KeyPressed(Keys.Enter))
                {
                    Protocol.GetDissassembled();
                    Protocol.GetInstructionPointerAddress();
                }

                if (InputKeyboard.KeyPressed(Keys.F5))
                {
                    Protocol.ToggleStepByStepMode();
                }

                if (InputKeyboard.KeyPressed(Keys.F8))
                {
                    Protocol.ContinueExecution();
                }

                if (InputKeyboard.KeyPressed(Keys.F9) && !UserInput.LockStepByStep)
                {
                    UserInput.LockStepByStep = true;
                    Protocol.AdvanceStepByStep();
                }
            }

            WindowManager.Update(IsActive);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Painter.Update();
            Reporter.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Painter.Draw();
            base.Draw(gameTime);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            // Release the client to continue
            Protocol.ContinueExecution();
            // Stop the threads
            Client.Abort = true;
            Thread.Sleep(400);
            base.OnExiting(sender, args);
        }
    }
}