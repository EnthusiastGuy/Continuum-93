
using Continuum93.ServiceModule;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text.Json;

namespace Continuum93.ServiceModule.Themes
{
    public static class ThemeLoader
    {
        public static Theme Load(string path)
        {
            string json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = ThemeJsonContext.Default
            };

            var data = JsonSerializer.Deserialize<ThemeData>(json, options);

            var theme = new Theme();
            
            // Background colors
            if (!string.IsNullOrEmpty(data.Background))
                theme.Background = ColorParser.Parse(data.Background);
            if (!string.IsNullOrEmpty(data.BackgroundTransparent))
                theme.BackgroundTransparent = ColorParser.Parse(data.BackgroundTransparent);

            // Window colors
            if (!string.IsNullOrEmpty(data.WindowBorder))
                theme.WindowBorder = ColorParser.Parse(data.WindowBorder);
            if (!string.IsNullOrEmpty(data.WindowTitleBarUnfocused))
                theme.WindowTitleBarUnfocused = ColorParser.Parse(data.WindowTitleBarUnfocused);
            if (!string.IsNullOrEmpty(data.WindowTitleBarFocused))
                theme.WindowTitleBarFocused = ColorParser.Parse(data.WindowTitleBarFocused);
            if (!string.IsNullOrEmpty(data.WindowTitleBarOnTop))
                theme.WindowTitleBarOnTop = ColorParser.Parse(data.WindowTitleBarOnTop);
            if (!string.IsNullOrEmpty(data.WindowBackgroundUnfocused))
                theme.WindowBackgroundUnfocused = ColorParser.Parse(data.WindowBackgroundUnfocused);
            if (!string.IsNullOrEmpty(data.WindowBackgroundFocused))
                theme.WindowBackgroundFocused = ColorParser.Parse(data.WindowBackgroundFocused);
            if (!string.IsNullOrEmpty(data.WindowResizeGrip))
                theme.WindowResizeGrip = ColorParser.Parse(data.WindowResizeGrip);
            if (!string.IsNullOrEmpty(data.WindowCloseButton))
                theme.WindowCloseButton = ColorParser.Parse(data.WindowCloseButton);
            if (!string.IsNullOrEmpty(data.WindowTitleText))
                theme.WindowTitleText = ColorParser.Parse(data.WindowTitleText);
            if (!string.IsNullOrEmpty(data.WindowCloseButtonText))
                theme.WindowCloseButtonText = ColorParser.Parse(data.WindowCloseButtonText);

            // Text colors
            if (!string.IsNullOrEmpty(data.TextPrimary))
                theme.TextPrimary = ColorParser.Parse(data.TextPrimary);
            if (!string.IsNullOrEmpty(data.TextSecondary))
                theme.TextSecondary = ColorParser.Parse(data.TextSecondary);
            if (!string.IsNullOrEmpty(data.TextHighlight))
                theme.TextHighlight = ColorParser.Parse(data.TextHighlight);
            if (!string.IsNullOrEmpty(data.TextOutline))
                theme.TextOutline = ColorParser.Parse(data.TextOutline);
            if (!string.IsNullOrEmpty(data.TextTitle))
                theme.TextTitle = ColorParser.Parse(data.TextTitle);
            if (!string.IsNullOrEmpty(data.TextInfo))
                theme.TextInfo = ColorParser.Parse(data.TextInfo);
            if (!string.IsNullOrEmpty(data.TextTitleHeader))
                theme.TextTitleHeader = ColorParser.Parse(data.TextTitleHeader);
            if (!string.IsNullOrEmpty(data.TextCyan))
                theme.TextCyan = ColorParser.Parse(data.TextCyan);
            if (!string.IsNullOrEmpty(data.TextAliceBlue))
                theme.VideoPaletteNumber = ColorParser.Parse(data.TextAliceBlue);
            if (!string.IsNullOrEmpty(data.TextDarkOrange))
                theme.TextDarkOrange = ColorParser.Parse(data.TextDarkOrange);
            if (!string.IsNullOrEmpty(data.TextGreenYellow))
                theme.TextGreenYellow = ColorParser.Parse(data.TextGreenYellow);
            if (!string.IsNullOrEmpty(data.TextIndianRed))
                theme.TextIndianRed = ColorParser.Parse(data.TextIndianRed);

            // Memory window colors
            if (!string.IsNullOrEmpty(data.MemoryAddressFull))
                theme.MemoryAddressFull = ColorParser.Parse(data.MemoryAddressFull);
            if (!string.IsNullOrEmpty(data.MemoryAddressZero))
                theme.MemoryAddressZero = ColorParser.Parse(data.MemoryAddressZero);
            if (!string.IsNullOrEmpty(data.MemoryAddressZeroOutline))
                theme.MemoryAddressZeroOutline = ColorParser.Parse(data.MemoryAddressZeroOutline);
            if (!string.IsNullOrEmpty(data.MemoryByteColor))
                theme.MemoryByteColor = ColorParser.Parse(data.MemoryByteColor);
            if (!string.IsNullOrEmpty(data.MemoryAsciiColor))
                theme.MemoryAsciiColor = ColorParser.Parse(data.MemoryAsciiColor);
            if (!string.IsNullOrEmpty(data.MemoryAsciiNonAsciiColor))
                theme.MemoryAsciiNonAsciiColor = ColorParser.Parse(data.MemoryAsciiNonAsciiColor);

            // Disassembler window colors
            if (!string.IsNullOrEmpty(data.DisassemblerAddressFull))
                theme.DisassemblerAddressFull = ColorParser.Parse(data.DisassemblerAddressFull);
            if (!string.IsNullOrEmpty(data.DisassemblerAddressZero))
                theme.DisassemblerAddressZero = ColorParser.Parse(data.DisassemblerAddressZero);
            if (!string.IsNullOrEmpty(data.DisassemblerAddressZeroOutline))
                theme.DisassemblerAddressZeroOutline = ColorParser.Parse(data.DisassemblerAddressZeroOutline);
            if (!string.IsNullOrEmpty(data.DisassemblerOpcodeColor))
                theme.DisassemblerOpcodeColor = ColorParser.Parse(data.DisassemblerOpcodeColor);
            if (!string.IsNullOrEmpty(data.DisassemblerInstructionColor))
                theme.DisassemblerInstructionColor = ColorParser.Parse(data.DisassemblerInstructionColor);
            if (!string.IsNullOrEmpty(data.DisassemblerHoverBorder))
                theme.DisassemblerHoverBorder = ColorParser.Parse(data.DisassemblerHoverBorder);
            if (!string.IsNullOrEmpty(data.DisassemblerCurrentInstructionBackground))
                theme.DisassemblerCurrentInstructionBackground = ColorParser.Parse(data.DisassemblerCurrentInstructionBackground);

            // Register window colors
            if (!string.IsNullOrEmpty(data.RegisterNameColor))
                theme.RegisterNameColor = ColorParser.Parse(data.RegisterNameColor);
            if (!string.IsNullOrEmpty(data.RegisterValueChangedColor))
                theme.RegisterValueChangedColor = ColorParser.Parse(data.RegisterValueChangedColor);
            if (!string.IsNullOrEmpty(data.RegisterValueUnchangedColor))
                theme.RegisterValueUnchangedColor = ColorParser.Parse(data.RegisterValueUnchangedColor);
            if (!string.IsNullOrEmpty(data.RegisterValueZeroTransparent))
                theme.NumberLeadingZeroes = ColorParser.Parse(data.RegisterValueZeroTransparent);
            if (!string.IsNullOrEmpty(data.RegisterMemoryDataColor))
                theme.RegisterMemoryDataColor = ColorParser.Parse(data.RegisterMemoryDataColor);
            if (!string.IsNullOrEmpty(data.RegisterChangedStateColor))
                theme.RegisterChangedStateColor = ColorParser.Parse(data.RegisterChangedStateColor);
            if (!string.IsNullOrEmpty(data.RegisterUnchangedStateColor))
                theme.RegisterUnchangedStateColor = ColorParser.Parse(data.RegisterUnchangedStateColor);

            // Flag window colors
            if (!string.IsNullOrEmpty(data.FlagNameColor))
                theme.FlagNameColor = ColorParser.Parse(data.FlagNameColor);
            if (!string.IsNullOrEmpty(data.FlagValueChangedColor))
                theme.FlagValueChangedColor = ColorParser.Parse(data.FlagValueChangedColor);
            if (!string.IsNullOrEmpty(data.FlagValueUnchangedColor))
                theme.FlagValueUnchangedColor = ColorParser.Parse(data.FlagValueUnchangedColor);
            if (!string.IsNullOrEmpty(data.FlagValueOneColor))
                theme.FlagValueOneColor = ColorParser.Parse(data.FlagValueOneColor);
            if (!string.IsNullOrEmpty(data.FlagValueZeroColor))
                theme.FlagValueZeroColor = ColorParser.Parse(data.FlagValueZeroColor);

            // Stack window colors
            if (!string.IsNullOrEmpty(data.StackValueColor))
                theme.StackValueColor = ColorParser.Parse(data.StackValueColor);

            // Taskbar colors
            if (!string.IsNullOrEmpty(data.TaskbarBackground))
                theme.TaskbarBackground = ColorParser.Parse(data.TaskbarBackground);
            if (!string.IsNullOrEmpty(data.TaskbarBorder))
                theme.TaskbarBorder = ColorParser.Parse(data.TaskbarBorder);
            if (!string.IsNullOrEmpty(data.TaskbarHeaderBackground))
                theme.TaskbarHeaderBackground = ColorParser.Parse(data.TaskbarHeaderBackground);
            if (!string.IsNullOrEmpty(data.TaskbarItemActiveBackground))
                theme.TaskbarItemActiveBackground = ColorParser.Parse(data.TaskbarItemActiveBackground);
            if (!string.IsNullOrEmpty(data.TaskbarItemMinimizedBackground))
                theme.TaskbarItemMinimizedBackground = ColorParser.Parse(data.TaskbarItemMinimizedBackground);
            if (!string.IsNullOrEmpty(data.TaskbarItemNormalBackground))
                theme.TaskbarItemNormalBackground = ColorParser.Parse(data.TaskbarItemNormalBackground);

            // Set the font reference
            theme.PrimaryFont = Fonts.ModernDOS_12x18_thin;

            return theme;
        }

        public static void Save(Theme theme, string path)
        {
            var data = new ThemeData
            {
                // Background colors
                Background = ColorParser.Format(theme.Background),
                BackgroundTransparent = ColorParser.Format(theme.BackgroundTransparent),

                // Window colors
                WindowBorder = ColorParser.Format(theme.WindowBorder),
                WindowTitleBarUnfocused = ColorParser.Format(theme.WindowTitleBarUnfocused),
                WindowTitleBarFocused = ColorParser.Format(theme.WindowTitleBarFocused),
                WindowTitleBarOnTop = ColorParser.Format(theme.WindowTitleBarOnTop),
                WindowBackgroundUnfocused = ColorParser.Format(theme.WindowBackgroundUnfocused),
                WindowBackgroundFocused = ColorParser.Format(theme.WindowBackgroundFocused),
                WindowResizeGrip = ColorParser.Format(theme.WindowResizeGrip),
                WindowCloseButton = ColorParser.Format(theme.WindowCloseButton),
                WindowTitleText = ColorParser.Format(theme.WindowTitleText),
                WindowCloseButtonText = ColorParser.Format(theme.WindowCloseButtonText),

                // Text colors
                TextPrimary = ColorParser.Format(theme.TextPrimary),
                TextSecondary = ColorParser.Format(theme.TextSecondary),
                TextHighlight = ColorParser.Format(theme.TextHighlight),
                TextOutline = ColorParser.Format(theme.TextOutline),
                TextTitle = ColorParser.Format(theme.TextTitle),
                TextInfo = ColorParser.Format(theme.TextInfo),
                TextTitleHeader = ColorParser.Format(theme.TextTitleHeader),
                TextCyan = ColorParser.Format(theme.TextCyan),
                TextAliceBlue = ColorParser.Format(theme.VideoPaletteNumber),
                TextDarkOrange = ColorParser.Format(theme.TextDarkOrange),
                TextGreenYellow = ColorParser.Format(theme.TextGreenYellow),
                TextIndianRed = ColorParser.Format(theme.TextIndianRed),

                // Memory window colors
                MemoryAddressFull = ColorParser.Format(theme.MemoryAddressFull),
                MemoryAddressZero = ColorParser.Format(theme.MemoryAddressZero),
                MemoryAddressZeroOutline = ColorParser.Format(theme.MemoryAddressZeroOutline),
                MemoryByteColor = ColorParser.Format(theme.MemoryByteColor),
                MemoryAsciiColor = ColorParser.Format(theme.MemoryAsciiColor),
                MemoryAsciiNonAsciiColor = ColorParser.Format(theme.MemoryAsciiNonAsciiColor),

                // Disassembler window colors
                DisassemblerAddressFull = ColorParser.Format(theme.DisassemblerAddressFull),
                DisassemblerAddressZero = ColorParser.Format(theme.DisassemblerAddressZero),
                DisassemblerAddressZeroOutline = ColorParser.Format(theme.DisassemblerAddressZeroOutline),
                DisassemblerOpcodeColor = ColorParser.Format(theme.DisassemblerOpcodeColor),
                DisassemblerInstructionColor = ColorParser.Format(theme.DisassemblerInstructionColor),
                DisassemblerHoverBorder = ColorParser.Format(theme.DisassemblerHoverBorder),
                DisassemblerCurrentInstructionBackground = ColorParser.Format(theme.DisassemblerCurrentInstructionBackground),

                // Register window colors
                RegisterNameColor = ColorParser.Format(theme.RegisterNameColor),
                RegisterValueChangedColor = ColorParser.Format(theme.RegisterValueChangedColor),
                RegisterValueUnchangedColor = ColorParser.Format(theme.RegisterValueUnchangedColor),
                RegisterValueZeroTransparent = ColorParser.Format(theme.NumberLeadingZeroes),
                RegisterMemoryDataColor = ColorParser.Format(theme.RegisterMemoryDataColor),
                RegisterChangedStateColor = ColorParser.Format(theme.RegisterChangedStateColor),
                RegisterUnchangedStateColor = ColorParser.Format(theme.RegisterUnchangedStateColor),

                // Flag window colors
                FlagNameColor = ColorParser.Format(theme.FlagNameColor),
                FlagValueChangedColor = ColorParser.Format(theme.FlagValueChangedColor),
                FlagValueUnchangedColor = ColorParser.Format(theme.FlagValueUnchangedColor),
                FlagValueOneColor = ColorParser.Format(theme.FlagValueOneColor),
                FlagValueZeroColor = ColorParser.Format(theme.FlagValueZeroColor),

                // Stack window colors
                StackValueColor = ColorParser.Format(theme.StackValueColor),

                // Taskbar colors
                TaskbarBackground = ColorParser.Format(theme.TaskbarBackground),
                TaskbarBorder = ColorParser.Format(theme.TaskbarBorder),
                TaskbarHeaderBackground = ColorParser.Format(theme.TaskbarHeaderBackground),
                TaskbarItemActiveBackground = ColorParser.Format(theme.TaskbarItemActiveBackground),
                TaskbarItemMinimizedBackground = ColorParser.Format(theme.TaskbarItemMinimizedBackground),
                TaskbarItemNormalBackground = ColorParser.Format(theme.TaskbarItemNormalBackground)
            };

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = ThemeJsonContext.Default,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }
    }
}
