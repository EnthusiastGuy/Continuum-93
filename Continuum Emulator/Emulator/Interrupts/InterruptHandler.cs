using Continuum93.Emulator;
using Continuum93.Emulator.Interrupts;
using Continuum93.Emulator.Interrupts.FileSystem;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptHandler
    {
        /// <summary>
        /// Main Interrupt handler
        /// </summary>
        /// <param name="intId">Interrupt number (0-255)</param>
        /// <param name="funcId">Interrupt function (0-255)</param>
        /// <param name="regId">Register ID (0-25)</param>
        public static void HandleInterrupt(byte intId, byte funcId, byte regId, Computer computer)
        {
            switch (intId)
            {
                case 0x00:  // MACHINE and BIOS
                    {
                        HandleMachineInterrupt(funcId, regId, computer);
                        return;
                    }
                case 0x01:  // VIDEO
                    {
                        HandleVideoInterrupt(funcId, regId, computer);
                        return;
                    }
                case 0x02:  // INPUT
                    {
                        HandleInputInterrupt(funcId, regId, computer);
                        return;
                    }
                case 0x03:  // RANDOM
                    {
                        HandleRandomInterrupt(funcId, regId, computer);
                        return;
                    }
                case 0x04:  // FILE SYSTEM
                    {
                        HandleFileSystemInterrupt(funcId, regId, computer);
                        return;
                    }
                case 0x05:  // STRINGS
                    {
                        HandleStringsInterrupt(funcId, regId, computer);
                        return;
                    }
            }
        }

        private static void HandleMachineInterrupt(byte funcId, byte regId, Computer computer)
        {
            switch (funcId)
            {
                case 0x00:  // Stop
                    {
                        InterruptsMachine.Stop(computer);
                        return;
                    }
                case 0x01:  // Clear
                    {
                        InterruptsMachine.Clear(computer);
                        return;
                    }
                case 0x03:  // ReadClock
                    {
                        InterruptsMachine.ReadClock(regId, computer);
                        return;
                    }
                case 0x10:  // ToggleFullscreen
                    {
                        InterruptsMachine.ToggleFullscreen(regId, computer);
                        return;
                    }
                case 0x20:  // ShutDown
                    {
                        InterruptsMachine.ShutDown();
                        return;
                    }
                case 0xC0:  // Build
                    {
                        InterruptsMachine.Build(regId, computer);
                        return;
                    }
                case 0xC1:  // Build BASIC
                    {
                        //InterruptsMachine.BuildBasic(regId, computer);
                        return;
                    }
                case 0xF0:  // Get CPU designation by input frequency
                    {
                        InterruptsMachine.GetCPUDesignationByFrequency(regId, computer);
                        return;
                    }
            }
        }

        // INT 0x01:  // VIDEO
        private static void HandleVideoInterrupt(byte funcId, byte regId, Computer computer)
        {
            switch (funcId)
            {
                case 0x00:  // Read video resolution
                    {
                        InterruptsVideo.ReadVideoResolution(regId, computer);
                        return;
                    }
                case 0x01:  // Read video pages count
                    {
                        InterruptsVideo.ReadVideoPagesCount(regId, computer);
                        return;
                    }
                case 0x02:  // Set video pages count
                    {
                        InterruptsVideo.SetVideoPagesCount(regId, computer);
                        return;
                    }
                case 0x03:  // Read video address
                    {
                        InterruptsVideo.ReadVideoAddress(regId, computer);
                        return;
                    }
                case 0x04:  // Read video palette address
                    {
                        InterruptsVideo.ReadVideoPaletteAddress(regId, computer);
                        return;
                    }
                case 0x05:  // Clear video page
                    {
                        InterruptsVideo.ClearVideoPage(regId, computer);
                        return;
                    }
                case 0x06:  // Draw filled rectangle
                    {
                        InterruptsVideo.DrawFilledRectangle(regId, computer);
                        return;
                    }
                case 0x07:  // Draw rectangle
                    {
                        InterruptsVideo.DrawRectangle(regId, computer);
                        return;
                    }
                case 0x08:
                    {
                        InterruptsVideo.DrawFilledRoundedRectangle(regId, computer);
                        return;
                    }
                case 0x09:
                    {
                        InterruptsVideo.DrawRoundedRectangle(regId, computer);
                        return;
                    }
                case 0x0E:  // Draw tile map sprite
                    {
                        InterruptsVideo.DrawTileMapSprite(regId, computer);
                        return;
                    }
                case 0x10:  // Sprite draw
                    {
                        InterruptsVideo.DrawSpriteToVideoPage(regId, computer);
                        return;
                    }
                case 0x11:  // Get text metrics
                    {
                        InterruptsVideoDrawText.GetTextMetrics(regId, computer);
                        return;
                    }
                case 0x12:  // Draw string
                    {
                        InterruptsVideo.DrawString(regId, computer);
                        return;
                    }
                case 0x14:  // Draw text
                    {
                        InterruptsVideoDrawText.DrawText(regId, computer);
                        return;
                    }
                case 0x15:  // Get text rectangle
                    {
                        InterruptsVideoDrawText.GetTextRectangle(regId, computer);
                        return;
                    } 
                case 0x20:  // Plot
                    {
                        InterruptsVideo.Plot(regId, computer);
                        return;
                    }
                case 0x21:  // Line
                    {
                        InterruptsVideo.Line(regId, computer);
                        return;
                    }
                case 0x22:  // Ellipse
                    {
                        InterruptsVideo.Ellipse(regId, computer);
                        return;
                    }
                case 0x23:  // Filled Ellipse
                    {
                        InterruptsVideo.DrawFilledEllipse(regId, computer);
                        return;
                    }
                case 0x24:  // LinePath
                    {
                        InterruptsVideo.LinePath(regId, computer);
                        return;
                    }
                case 0x25:  // Bezier path
                    {
                        InterruptsVideoDrawBezierPath.DrawBezierPath(regId, computer);
                        return;
                    }
                case 0x26:  // Perlin path
                    {
                        InterruptsVideoPerlinPath.DrawPerlinPath(regId, computer);
                        return;
                    }
                case 0x28:  // Area or poly fill
                    {
                        InterruptsVideoFill.FillAreaOrPolygon(regId, computer);
                        return;
                    }
                case 0x30:  // Read layers visibility
                    {
                        InterruptsVideo.ReadLayerVisibility(regId, computer);
                        return;
                    }
                case 0x31:  // Set layers visibility
                    {
                        InterruptsVideo.SetLayersVisibility(regId, computer);
                        return;
                    }
                case 0x32:  // Read buffer control mode
                    {
                        InterruptsVideo.ReadBufferControlMode(regId, computer);
                        return;
                    }
                case 0x33:  // Set buffer control mode
                    {
                        InterruptsVideo.SetBufferControlMode(regId, computer);
                        return;
                    }
                case 0x40:  // Scroll/roll
                    {
                        InterruptsVideoScrolling.Scroll(regId, computer);
                        return;
                    }
                case 0x41:  // Copy rectangle
                    {
                        InterruptsVideoCopying.CopyRectangle(regId, computer);
                        return;
                    }
            }
        }

        private static void HandleInputInterrupt(byte funcId, byte regId, Computer computer)
        {
            switch (funcId)
            {
                case 0x00:  // Read keyboard state as bits
                    {
                        InterruptsInput.ReadKeyboardStateAsBits(regId, computer);
                        return;
                    }
                case 0x01:  // Read keyboard buffer
                    {
                        InterruptsInput.ReadKeyboardBuffer(regId, computer);
                        return;
                    }
                case 0x02:  // Handle keyboard state changed
                    {
                        InterruptsInput.HandleKeyboardStateChanged(regId, computer);
                        return;
                    }
                case 0x03:  // Read mouse state
                    {
                        InterruptsInput.ReadMouseState(regId, computer);
                        return;
                    }
                case 0x04:  // Handle mouse state changed
                    {
                        InterruptsInput.HandleMouseStateChanged(regId, computer);
                        return;
                    }
                case 0x05:
                    {
                        return;
                    }
                case 0x06:  // Handle controller state changed
                    {
                        InterruptsInput.HandleControllerStateChanged(regId, computer);
                        return;
                    }
                case 0x10:  // Read keyboard state as code bytes
                    {
                        InterruptsInput.ReadKeyboardStateAsBytes(regId, computer);
                        return;
                    }
                case 0x14:  // Read gamepads state
                    {
                        InterruptsInput.ReadGamePadsState(regId, computer);
                        return;
                    }
                case 0x15:  // Read gamepads capabilities
                    {
                        InterruptsInput.ReadGamePadsCapabilities(regId, computer);
                        return;
                    }
                case 0x16:  // Read gamepads names
                    {
                        InterruptsInput.ReadGamePadsNames(regId, computer);
                        return;
                    }
            }
        }

        private static void HandleRandomInterrupt(byte funcId, byte regId, Computer computer)
        {
            switch (funcId)
            {
                case 0x00:  // Random 8 bit
                    {
                        InterruptsRandom.Random8Bit(regId, computer);
                        return;
                    }
                case 0x01:  // Random 8-bit custom
                    {
                        InterruptsRandom.Random8BitCustom(regId, computer);
                        return;
                    }
                case 0x02:  // Random 16-bit
                    {
                        InterruptsRandom.Random16Bit(regId, computer);
                        return;
                    }
                case 0x03:  // Random 16-bit custom
                    {
                        InterruptsRandom.Random16BitCustom(regId, computer);
                        return;
                    }
                case 0x04:  // Random 24 bit
                    {
                        InterruptsRandom.Random24Bit(regId, computer);
                        return;
                    }
                case 0x05:  // Random 24-bit custom
                    {
                        InterruptsRandom.Random24BitCustom(regId, computer);
                        return;
                    }
                case 0x06:  // Random 32 bit
                    {
                        InterruptsRandom.Random32Bit(regId, computer);
                        return;
                    }
                case 0x07:  // Random 32-bit custom
                    {
                        InterruptsRandom.Random32BitCustom(regId, computer);
                        return;
                    }
            }
        }

        private static void HandleFileSystemInterrupt(byte funcId, byte regId, Computer computer)
        {
            switch (funcId)
            {
                case 0x02:  // File exists
                    {
                        FileSystemGeneral.CheckIfFileExists(regId, computer);
                        return;
                    }
                case 0x03:  // Directory exists
                    {
                        FileSystemGeneral.CheckIfDirectoryExists(regId, computer);
                        return;
                    }
                case 0x04:  // List files in directory
                    {
                        FileSystemGeneral.ListFilesInDirectory(regId, computer);
                        return;
                    }
                case 0x05:  // List directories in directory
                    {
                        FileSystemGeneral.ListDirectoriesInDirectory(regId, computer);
                        return;
                    }
                case 0x06:  // Save file
                    {
                        FileSystemGeneral.SaveFile(regId, computer);
                        return;
                    }
                case 0x07:  // Load file
                    {
                        FileSystemGeneral.LoadFile(regId, computer);
                        return;
                    }
                case 0x15:  // List directories and files in directory
                    {
                        FileSystemGeneral.ListDirectoriesAndFilesInDirectory(regId, computer);
                        return;
                    }
                case 0x20:  // Get file size
                    {
                        FileSystemGeneral.GetFileSize(regId, computer);
                        return;
                    }
                case 0x30:  // Load image and palette
                    {
                        FileSystemGeneral.LoadImageAndPalette(regId, computer);
                        return;
                    }
                case 0x31:  // Load image
                    {
                        FileSystemGeneral.LoadImage(regId, computer);
                        return;
                    }
                case 0x32:  // Load palette
                    {
                        FileSystemGeneral.LoadPalette(regId, computer);
                        return;
                    }
                case 0x33:  // Load 8 bit PNG
                    {
                        FileSystemPNG.Load8BitPNG(regId, computer);
                        return;
                    }
                case 0x34:  // Load 8 bit PNG with custom transparency
                    {
                        FileSystemPNG.Load8BitPNGCustomTransparency(regId, computer);
                        return;
                    }
                case 0x35:  // Merge 8 bit PNG
                    {
                        FileSystemPNG.Merge8BitPNG(regId, computer);
                        return;
                    }
                case 0x36:  // Merge 8 bit PNG with custom transparency
                    {
                        FileSystemPNG.Merge8BitPNGCustomTransparency(regId, computer);
                        return;
                    }
                case 0x40:  // Load png font
                    {
                        FileSystemFont.LoadPNGFont(regId, computer);
                        return;
                    }
            }
        }

        private static void HandleStringsInterrupt(byte funcId, byte regId, Computer computer)
        {
            switch (funcId)
            {
                case 0x01:
                    {
                        InterruptsStrings.FloatToString(regId, computer);
                        return;
                    }
                case 0x02:
                    {
                        InterruptsStrings.UintToString(regId, computer);
                        return;
                    }
                case 0x03:
                    {
                        InterruptsStrings.IntToString(regId, computer);
                        return;
                    }
            }
        }
    }
}
