using Continuum93.Emulator;
using Continuum93.Emulator.AutoDocs;
using Continuum93.Emulator.Colors;
using Continuum93.Emulator.Controls;
using Continuum93.Emulator.Settings;
using Continuum93.Emulator.Window;
using Continuum93.Tools;
using Continuum93.Utils;
using Continuum93.CodeAnalysis.Network;
using Continuum93.Emulator.Audio.Samples;
using Continuum93.Emulator.Audio.XSound;
using Continuum93.Emulator.Audio.XSound.Debug;
using Continuum93.Emulator.AutoDocs.MetaInfo;
using Continuum93.Emulator.AutoDocs.styling;
using Continuum93.Emulator.States;
using Continuum93.Tools.InstructionCycleBenchmark;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Threading;
using Continuum93.ServiceModule;

namespace Continuum93
{
    public sealed class Continuum : Game
    {
        private readonly bool _mouseEnabled;
        private readonly bool _debugEnabled;
        private readonly bool _startFullscreen;

        public Continuum(string[] args)
        {
            Log.WriteLine("Continuum started.");
            Log.WriteLine("Loaded configuration file");
            if (args.Length > 0)
                UtilsManager.Arguments = args;

            Renderer.RegisterGraphicsDeviceManager(new GraphicsDeviceManager(this) { GraphicsProfile = GraphicsProfile.Reach }, this);
            Log.WriteLine("Registered the GDM");

            Content.RootDirectory = "Content";

            _mouseEnabled = !SettingsManager.GetBoleanSettingsValue("disableMouse");
            _debugEnabled = true;// SettingsManager.GetBoleanSettingsValue("enableDebugging");
            _startFullscreen = SettingsManager.GetBoleanSettingsValue("fullscreen");

            IsMouseVisible = _mouseEnabled;

        }

        protected override void Initialize()
        {
            SettingsManager.LoadConfig();       // This must be the very first thing done on startup
            WindowManager.Initialize(Window);
            Log.WriteLine("Initialized window manager");
            Renderer.InitializeSpriteBatch();
            WindowManager.SetWindowSize(3.0f);
            Renderer.GetGraphicsDeviceManager().SynchronizeWithVerticalRetrace = true;

            Palettes.Initialize();

            Log.WriteLine("Initialized projection texture");
            InputKeyboard.Initialize();
            InputGamepad.Initialize();
            Log.WriteLine("Initialized keyboard");

            double fps = 60;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / fps);


            MachineProcess.Start();
            //CycleBenchmark.Run(); // Comment line above and expect results in the log

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Renderer.InterlaceEffect = Content.Load<Effect>("InterlaceShader");
            //Watcher.WatchDirectoryOfFile(SettingsManager.GetSettingValue("bootProgram"));
            Renderer.CrtEffect = Content.Load<Effect>("Shaders/CrtEffect");
            Renderer.CrtEffect.Parameters["TextureSize"]?.SetValue(new Vector2(Constants.V_WIDTH, Constants.V_HEIGHT));

            Renderer.SetFullScreen(_startFullscreen);

            if (_debugEnabled)
            {
                Thread.Sleep(10);
                Log.WriteLine("Starting server");
                Server.Start();
                Log.WriteLine("Starting machine");
            }

            ServiceGraphics.Initialize();
        }

        // Deprecated, kept for reference
        private void UpdateAdmin()
        {
            if (InputKeyboard.KeyPressed(Keys.D))
            {
                IntLib.Initialize();    // Move to initialize if not working
                ASMMetaInfo.Initialize();

                Directory.CreateDirectory("docs");
                File.WriteAllText(Path.Combine("docs", "styles.css"), DocStyle.GetDefaultStyle());
                File.WriteAllText(Path.Combine("docs", "stylesInfo.css"), DocStyle.GetInfoStyle());

                File.WriteAllText(Path.Combine("docs", "assembly.html"), ASMReferenceGenerator.GenerateInstructionPages());
                File.WriteAllText(Path.Combine("docs", "assembly_compact.html"), ASMMetaGenerator.GenerateMetaInfo());
                File.WriteAllText(Path.Combine("docs", "assembly_compact_brief.html"), ASMMetaGenerator.GenerateMetaInfo(true));

                File.WriteAllText(Path.Combine("docs", "interrupts.html"), IntLib.GenerateInterruptPages());
                File.WriteAllText(Path.Combine("docs", "sampleInstructions.txt"), DocsInstructionSamples.Generate());

                File.WriteAllText(Path.Combine("docs", "operatorsTable.txt"), ASMReferenceGenerator.GenerateOPSTable());

                Exit(); // Exit after generating
            }
            else if (InputKeyboard.KeyPressed(Keys.F))
            {
                UtilsManager.ConvertFont("DoctorJack.png");
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (UtilsManager.HasArguments())
            {
                UtilsManager.ProcessArguments();
                Machine.COMPUTER.Stop();
                Exit();
                return;
            }

            InputKeyboard.Update();
            //InputGamepad.Update();
            GamepadStateExts.Update();
            WindowManager.Update();
            if (_mouseEnabled) { InputMouse.Update(); }
            ImageLoadState.Update();
            Service.INPUT.Update(gameTime);
            Service.GRAPHICS.Update(gameTime);

            GameTimePlus.Update(gameTime);

            //UpdateAdmin();

            if (State.FullScreenRequest)
            {
                Renderer.ToggleFullscreen();
                State.FullScreenRequest = false;
                State.IsFullScreen = Renderer.IsFullScreen();
                if (!State.IsFullScreen)
                {
                    WindowManager.SetWindowSize(2.0f);
                }
            }

            if (State.ShutDownRequested)
            {
                Exit();
            }

            if (InputKeyboard.KeyPressed(Keys.F20))
            {
                XSoundDebugger.SavePlotData(new XSoundParams()
                {
                    Frequency = 440.0f,
                    EnvelopeSustain = 0.0091f * 1,
                    SoundVolume = 1.0f,
                    WaveType = XSoundParams.WaveTypes.COMPLEX,
                    WaveTypeValues = { { XSoundParams.WaveTypes.TAN, 5 }, { XSoundParams.WaveTypes.RINGMODULATION, 95 } },
                    DutyCycle = 0.8f, DutyCycleRamp = 0.0f,
                    EnvelopeAttack = 0, EnvelopeDecay = 0,
                });
            };

            /*if (InputKeyboard.KeyPressed(Keys.Delete))
            {
                string bench = Benchmark.GetAllStatisticsDetailed();
            };*/

            /*
            if (InputKeyboard.KeyPressed(Keys.D0))
            {
                // Convert splash.png
                ImageConvertor.ConvertPNGToByteArray(@"Data\filesystem\splash.png");
            }
            */

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Service.STATE.UseServiceView)
            {
                Service.GRAPHICS.Draw();    // Includes drawing the emulated machine screen inside the service view
            } else {
                Machine.COMPUTER?.GRAPHICS.Draw();  // Normal, non-service rendering: full-screen emulated machine
            }

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            //double calculatedFrequency = CPUBenchmark.GetInstructionsPerSecond();
            //Log.WriteLine($"Emulator stopped. Average frequency: {calculatedFrequency:0} Hz ({calculatedFrequency / 1_000_000:0.00} Mhz)");
            Machine.COMPUTER.Stop();
            Machine.COMPUTER?.Dispose();
            MachineProcess.Stop();
            Server.Stop();
            Thread.Sleep(100);
            Log.FlushData();
            base.OnExiting(sender, args);
        }
    }
}