namespace Continuum93.ServiceModule
{
    public static class Fonts
    {
        public static ServiceFont ModernDOS_9x15;
        public static ServiceFont ModernDOS_10x15;
        public static ServiceFont ModernDOS_10x16;
        public static ServiceFont ModernDOS_12x18;
        public static ServiceFont ModernDOS_12x18_thin;

        public static void LoadFonts()
        {
            ModernDOS_9x15 = new ServiceFont("Data/fonts/ModernDOS-9x15.png");
            ModernDOS_10x15 = new ServiceFont("Data/fonts/ModernDOS-10x15.png");
            ModernDOS_10x16 = new ServiceFont("Data/fonts/ModernDOS-10x16.png");
            ModernDOS_12x18 = new ServiceFont("Data/fonts/ModernDOS-12x18.png");
            ModernDOS_12x18_thin = new ServiceFont("Data/fonts/ModernDOS-12x18-thin.png");
        }
    }
}
