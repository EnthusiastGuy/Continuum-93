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

namespace Continuum93
{
    public class Continuum : Game
    {
        private SpriteBatch _serviceSpriteBatch;

        private float _serviceAnim;          // 0 = normal, 1 = debug
        private const float ServiceAnimSpeed = 5.0f; // how fast it moves between states

        public Continuum(string[] args)
        {
            Log.WriteLine("Continuum started.");
            Log.WriteLine("Loaded configuration file");
            if (args.Length > 0)
                UtilsManager.Arguments = args;

            Renderer.RegisterGraphicsDeviceManager(new GraphicsDeviceManager(this) { GraphicsProfile = GraphicsProfile.Reach });
            Log.WriteLine("Registered the GDM");

            Content.RootDirectory = "Content";
            if (!SettingsManager.GetBoleanSettingsValue("disableMouse")) { IsMouseVisible = false; }

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
            //Renderer.InterlaceEffect = Content.Load<Effect>("InterlaceShader");
            //Watcher.WatchDirectoryOfFile(SettingsManager.GetSettingValue("bootProgram"));

            // SpriteBatch used for the integrated Service UI
            _serviceSpriteBatch = new SpriteBatch(GraphicsDevice);

            Renderer.SetFullScreen(SettingsManager.GetBoleanSettingsValue("fullscreen"));

            if (SettingsManager.GetBoleanSettingsValue("enableDebugging"))
            {
                Thread.Sleep(10);
                Log.WriteLine("Starting server");
                Server.Start();
                Log.WriteLine("Starting machine");
            }
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
            }

            InputKeyboard.Update();
            //InputGamepad.Update();
            GamepadStateExts.Update();
            WindowManager.Update();
            if (!SettingsManager.GetBoleanSettingsValue("disableMouse")) { InputMouse.Update(); }
            ImageLoadState.Update();
            GameTimePlus.Update(gameTime);

            // Service mode code
            if (InputKeyboard.KeyPressed(State.ServiceKey)) // Toggle service mode
            {
                State.ToggleServiceMode();
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float target = State.ServiceMode ? 1f : 0f;

            if (_serviceAnim < target)
            {
                _serviceAnim = Math.Min(target, _serviceAnim + ServiceAnimSpeed * dt);
            }
            else if (_serviceAnim > target)
            {
                _serviceAnim = Math.Max(target, _serviceAnim - ServiceAnimSpeed * dt);
            }

            // End service mode code


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

        // TODO, cleanup when done with this and move logic to a dedicated ServiceModeRenderer class
        protected override void Draw(GameTime gameTime)
        {
            var gfx = Machine.COMPUTER?.GRAPHICS;

            // We want to use the service-layout renderer both:
            // - when service mode is ON
            // - while we are animating back (_serviceAnim > 0)
            bool useServiceView = (State.ServiceMode || _serviceAnim > 0.001f) && gfx != null;

            if (useServiceView)
            {
                // Making sure the 480x270 texture is up-to-date
                gfx.UpdateProjectionOnly();

                var projection = gfx.VideoProjection;

                if (projection == null)
                {
                    // Nothing to show yet
                    Renderer.DrawBlank();
                    base.Draw(gameTime);
                    return;
                }

                // --- FULLSCREEN RECT (normal mode with pillars/bars) ---
                var rectFull = Renderer.GetDestinationRectangle(projection.Width, projection.Height);

                // --- SERVICE RECT (final position in service mode) ---
                const int padding = 16;
                var rectService = new Rectangle(
                    padding,
                    padding,
                    projection.Width,   // 480
                    projection.Height   // 270
                );

                // Easing (feels nicer than linear):
                float t = _serviceAnim;
                float eased = t * t * (3f - 2f * t); // smoothstep 0..1

                // Clear the fullscreen backbuffer for the service layout,
                // LERPing from black (normal mode) to dark gray (service mode)
                // TODO, move the colors to some form of theme/settings
                Color bgColor = Color.Lerp(Color.Black, Color.DarkGray, eased);
                Renderer.Clear(bgColor);

                // Interpolate between fullscreen and service rect
                Rectangle destRect = LerpRect(rectFull, rectService, eased);

                _serviceSpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.Opaque,
                    SamplerState.PointClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone
                );

                // Live view of the emulator with animated position/size
                _serviceSpriteBatch.Draw(
                    projection,
                    destRect,
                    Color.White
                );

                _serviceSpriteBatch.End();

                base.Draw(gameTime);
                return;
            }

            // Normal, non-service rendering: full-screen emulated machine
            Machine.COMPUTER.GRAPHICS.Draw();

            base.Draw(gameTime);
        }



        private static Rectangle LerpRect(Rectangle from, Rectangle to, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            int x = (int)MathHelper.Lerp(from.X, to.X, t);
            int y = (int)MathHelper.Lerp(from.Y, to.Y, t);
            int w = (int)MathHelper.Lerp(from.Width, to.Width, t);
            int h = (int)MathHelper.Lerp(from.Height, to.Height, t);
            return new Rectangle(x, y, w, h);
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