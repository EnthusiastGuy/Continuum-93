namespace Continuum93.ServiceModule.Themes
{
    public class ThemeData
    {
        // Background colors
        public string Background { get; set; }
        public string BackgroundTransparent { get; set; }

        // Window colors
        public string WindowBorder { get; set; }
        public string WindowTitleBarUnfocused { get; set; }
        public string WindowTitleBarFocused { get; set; }
        public string WindowTitleBarOnTop { get; set; }
        public string WindowBackgroundUnfocused { get; set; }
        public string WindowBackgroundFocused { get; set; }
        public string WindowResizeGrip { get; set; }
        public string WindowCloseButton { get; set; }
        public string WindowTitleText { get; set; }
        public string WindowCloseButtonText { get; set; }

        // Text colors
        public string TextPrimary { get; set; }
        public string TextSecondary { get; set; }
        public string TextHighlight { get; set; }
        public string TextOutline { get; set; }
        public string TextTitle { get; set; }
        public string TextInfo { get; set; }
        public string TextTitleHeader { get; set; }
        public string TextCyan { get; set; }
        public string TextAliceBlue { get; set; }
        public string TextDarkOrange { get; set; }
        public string TextGreenYellow { get; set; }
        public string TextIndianRed { get; set; }

        // Memory window colors
        public string MemoryAddressFull { get; set; }
        public string MemoryAddressZero { get; set; }
        public string MemoryAddressZeroOutline { get; set; }
        public string MemoryByteColor { get; set; }
        public string MemoryAsciiColor { get; set; }
        public string MemoryAsciiNonAsciiColor { get; set; }

        // Disassembler window colors
        public string DisassemblerAddressFull { get; set; }
        public string DisassemblerAddressZero { get; set; }
        public string DisassemblerAddressZeroOutline { get; set; }
        public string DisassemblerOpcodeColor { get; set; }
        public string DisassemblerInstructionColor { get; set; }
        public string DisassemblerHoverBorder { get; set; }
        public string DisassemblerCurrentInstructionBackground { get; set; }

        // Register window colors
        public string RegisterNameColor { get; set; }
        public string RegisterValueChangedColor { get; set; }
        public string RegisterValueUnchangedColor { get; set; }
        public string RegisterValueZeroTransparent { get; set; }
        public string RegisterMemoryDataColor { get; set; }
        public string RegisterChangedStateColor { get; set; }
        public string RegisterUnchangedStateColor { get; set; }

        // Flag window colors
        public string FlagNameColor { get; set; }
        public string FlagValueChangedColor { get; set; }
        public string FlagValueUnchangedColor { get; set; }
        public string FlagValueOneColor { get; set; }
        public string FlagValueZeroColor { get; set; }

        // Stack window colors
        public string StackValueColor { get; set; }

        // Taskbar colors
        public string TaskbarBackground { get; set; }
        public string TaskbarBorder { get; set; }
        public string TaskbarHeaderBackground { get; set; }
        public string TaskbarItemActiveBackground { get; set; }
        public string TaskbarItemMinimizedBackground { get; set; }
        public string TaskbarItemNormalBackground { get; set; }
    }
}
