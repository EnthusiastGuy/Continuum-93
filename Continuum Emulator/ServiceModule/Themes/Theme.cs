using Continuum93.ServiceModule;
using Microsoft.Xna.Framework;

namespace Continuum93.ServiceModule.Themes
{
    public class Theme
    {
        // Font
        public ServiceFont PrimaryFont { get; set; }

        // Window colors
        public Color WindowBorder { get; set; } = Color.Gray;
        public Color WindowTitleBarUnfocused { get; set; } = new Color(30, 30, 30);
        public Color WindowTitleBarFocused { get; set; } = new Color(40, 40, 80);
        public Color WindowTitleBarOnTop { get; set; } = new Color(60, 60, 120);
        public Color WindowBackgroundUnfocused { get; set; } = new Color(5, 5, 5, 250);
        public Color WindowBackgroundFocused { get; set; } = new Color(5, 5, 5, 128);
        public Color WindowResizeGrip { get; set; } = Color.DimGray;
        public Color WindowCloseButton { get; set; } = Color.Red;
        public Color WindowTitleText { get; set; } = Color.DarkOrange;
        public Color WindowCloseButtonText { get; set; } = Color.White;

        // Text colors
        public Color TextPrimary { get; set; } = Color.White;
        public Color TextSecondary { get; set; } = Color.Yellow;
        public Color TextHighlight { get; set; } = Color.Yellow;

        public Color VideoPaletteNumber { get; set; } = Color.Red;
        public Color TextOutline { get; set; } = Color.Black;
        public Color TextTitle { get; set; } = Color.Yellow;
        public Color TextInfo { get; set; } = Color.Khaki;
        public Color TextTitleHeader { get; set; } = Color.YellowGreen;
        public Color TextCyan { get; set; } = Color.Cyan;
        
        public Color TextDarkOrange { get; set; } = Color.DarkOrange;
        public Color TextGreenYellow { get; set; } = Color.GreenYellow;
        public Color TextIndianRed { get; set; } = Color.IndianRed;

        // Memory window colors
        public Color MemoryAddressFull { get; set; } = new Color(80, 160, 255);
        public Color MemoryAddressZero { get; set; } = new Color(80, 160, 255, 22);
        public Color MemoryAddressZeroOutline { get; set; } = new Color(0, 0, 0, 22);
        public Color MemoryByteColor { get; set; } = new Color(40, 80, 160);
        public Color MemoryAsciiColor { get; set; } = new Color(200, 200, 200);
        public Color MemoryAsciiNonAsciiColor { get; set; } = new Color(139, 69, 19); // Brown for non-ASCII characters

        // Disassembler window colors
        public Color DisassemblerAddressFull { get; set; } = new Color(80, 160, 255);
        public Color DisassemblerAddressZero { get; set; } = new Color(80, 160, 255, 22);
        public Color DisassemblerAddressZeroOutline { get; set; } = new Color(0, 0, 0, 22);
        public Color DisassemblerOpcodeColor { get; set; } = new Color(40, 80, 160);
        public Color DisassemblerInstructionColor { get; set; } = new Color(255, 180, 50);
        public Color DisassemblerHoverBorder { get; set; } = new Color(80, 220, 80);
        public Color DisassemblerCurrentInstructionBackground { get; set; } = new Color(0, 139, 139, 100);

        // Register window colors
        public Color RegisterNameColor { get; set; } = new Color(0.5f, 0.5f, 1f);
        public Color RegisterValueChangedColor { get; set; } = Color.DarkOrange;
        public Color RegisterValueUnchangedColor { get; set; } = Color.White;
        public Color NumberLeadingZeroes { get; set; } = new Color(20, 40, 64, 1);
        public Color RegisterMemoryDataColor { get; set; } = new Color(40, 80, 160);
        public Color RegisterChangedStateColor { get; set; } = Color.OrangeRed;
        public Color RegisterUnchangedStateColor { get; set; } = Color.White;

        // Flag window colors
        public Color FlagNameColor { get; set; } = new Color(0.5f, 0.5f, 1f);
        public Color FlagValueChangedColor { get; set; } = Color.DarkOrange;
        public Color FlagValueUnchangedColor { get; set; } = Color.White;
        public Color FlagValueOneColor { get; set; } = Color.Green;
        public Color FlagValueZeroColor { get; set; } = Color.Red;

        // Stack window colors
        public Color StackValueColor { get; set; } = new Color(127, 255, 212); // Aquamarine
        public Color StackLastValueColor { get; set; } = new Color(64, 128, 160); // Darker blue

        // Taskbar colors
        public Color TaskbarBackground { get; set; } = new Color(10, 10, 20, 220);
        public Color TaskbarBorder { get; set; } = new Color(80, 80, 120);
        public Color TaskbarHeaderBackground { get; set; } = new Color(30, 30, 60);
        public Color TaskbarItemActiveBackground { get; set; } = new Color(80, 80, 140);
        public Color TaskbarItemMinimizedBackground { get; set; } = new Color(20, 20, 20);
        public Color TaskbarItemNormalBackground { get; set; } = new Color(40, 40, 60);

        // Background colors
        public Color Background { get; set; } = Color.Black;
        public Color BackgroundTransparent { get; set; } = Color.Transparent;
    }
}
