using Continuum93.Emulator;
using Continuum93.Emulator.GraphicsAccelerators;
using Microsoft.Xna.Framework;
using System.IO;

namespace Continuum93.Emulator.States
{
    public static class ImageLoadState
    {
        public static volatile string ImagePath;

        public static volatile byte ImageIntRegId;
        public static volatile uint DestinationAddress;

        public static volatile bool RequestLoadImageAndPalette = false;

        public static volatile bool RequestLoadImage = false;
        public static volatile bool RequestLoadPalette = false;

        public static volatile Color[] ColorPalette;
        public static volatile byte[] ByteRGBPalette;
        public static volatile byte[] ImageData;


        public static void Update()
        {
            if (RequestLoadImageAndPalette)
            {
                ExecuteProcessImageAndPalette();
            }
            else if (RequestLoadImage)
            {
                ExecuteProcessPixelData();
            }
            else if (RequestLoadPalette)
            {
                ExecuteProcessPalette();
            }
        }

        private static void ExecuteProcessImageAndPalette()
        {
            GFXImageProcessing.ProcessImageAndPalette(Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(ImagePath)));
            Machine.COMPUTER.LoadMemAt(DestinationAddress, ImageData);
            Machine.COMPUTER.LoadMemAt((uint)(DestinationAddress - ByteRGBPalette.Length), ByteRGBPalette);
            Machine.COMPUTER.CPU.REGS.Set8BitRegister(ImageIntRegId,
                (byte)ColorPalette.Length);
            GFXImageProcessing.ClearAll();

            Machine.COMPUTER.Unpause();
            RequestLoadImageAndPalette = false;
        }

        private static void ExecuteProcessPixelData()
        {
            GFXImageProcessing.ProcessPixelData(Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(ImagePath)));
            Machine.COMPUTER.LoadMemAt(DestinationAddress, ImageData);
            GFXImageProcessing.ClearAll();

            Machine.COMPUTER.Unpause();
            RequestLoadImage = false;
        }

        private static void ExecuteProcessPalette()
        {
            GFXImageProcessing.ProcessPalette(Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(ImageLoadState.ImagePath)));

            Machine.COMPUTER.LoadMemAt(DestinationAddress, ImageData);
            Machine.COMPUTER.CPU.REGS.Set8BitRegister(ImageIntRegId,
                (byte)ColorPalette.Length);
            GFXImageProcessing.ClearAll();
            RequestLoadPalette = false;
        }
    }
}
