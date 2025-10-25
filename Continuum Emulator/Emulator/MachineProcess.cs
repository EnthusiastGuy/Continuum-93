using Continuum93.Emulator;
using Continuum93.Emulator.Run;
using Continuum93.Emulator.Settings;
using System;
using System.Threading;

namespace Continuum93.Emulator
{
    /// <summary>
    /// Manages the emulator's main execution thread (the 'Machine Process').
    /// This static class ensures the emulator machine runs independently in the background.
    /// </summary>
    public static class MachineProcess
    {
        private static Thread machineThread;

        public static void Start()
        {
            if (machineThread == null || !machineThread.IsAlive)
            {
                machineThread = new Thread(() =>
                {
                    try
                    {
                        Machine.InitializeComputer();
                        Log.WriteLine("Initialized palettes");
                        Machine.COMPUTER.GRAPHICS.InitializeTexture();
                        Log.WriteLine("Machine boot success");

                        // Load and run the program specified as the 'bootProgram' in settings.
                        Runner.LoadAndRun(SettingsManager.GetPathSettingValue("bootProgram"));
                        Machine.COMPUTER.Run();
                        Log.WriteLine("Machine halted execution");
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine("Machine threw exception: " + e.Message);
                    }
                })
                {
                    Priority = ThreadPriority.AboveNormal
                };

                machineThread.Start();
            }
        }

        public static void Stop()
        {
            if (machineThread != null && machineThread.IsAlive)
            {
                Machine.COMPUTER?.Stop();
                machineThread.Join();
                Machine.COMPUTER?.Dispose();
                machineThread = null;
            }
        }
    }
}
