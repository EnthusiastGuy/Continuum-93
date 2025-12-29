using System;
using System.Collections.Generic;
using System.IO;
using Continuum93.Emulator;
using Continuum93.ServiceModule.UI;
using GameWindowManager = Continuum93.Emulator.Window.WindowManager;
using UIManager = Continuum93.ServiceModule.UI.WindowManager;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Continuum93.ServiceModule
{
    /// <summary>
    /// Persists service-mode window layouts, main window size, and immediate text.
    /// </summary>
    public static class ServiceLayoutManager
    {
        private const string LayoutPath = "Data/serviceState/layout.json";
        private static UIManager _windowManager;
        private static ImmediateWindow _immediateWindow;
        private static int _pendingMainWidth;
        private static int _pendingMainHeight;
        private static int _preServiceWidth;
        private static int _preServiceHeight;
        private static bool _hasPreServiceSize;

        public static void Initialize(UIManager wm, ImmediateWindow immediateWindow)
        {
            _windowManager = wm;
            _immediateWindow = immediateWindow;
            Directory.CreateDirectory(Path.GetDirectoryName(LayoutPath)!);
        }

        public static void LoadIfPresent()
        {
            if (_windowManager == null)
                return;

            if (!File.Exists(LayoutPath))
                return;

            try
            {
                var json = File.ReadAllText(LayoutPath);
                var state = JsonSerializer.Deserialize(json, ServiceLayoutContext.Default.ServiceLayoutState);
                if (state == null)
                    return;

                // Store desired main window size; apply only when service mode is entered
                _pendingMainWidth = state.MainWidth;
                _pendingMainHeight = state.MainHeight;

                if (state.Windows != null)
                {
                    foreach (var dto in state.Windows)
                    {
                        var w = _windowManager.Windows.Find(win => string.Equals(win.Title, dto.Title, StringComparison.OrdinalIgnoreCase));
                        if (w != null)
                        {
                            w.ApplyLayout(dto.X, dto.Y, dto.Width, dto.Height, dto.Visible);
                        }
                    }
                }

                if (_immediateWindow != null && !string.IsNullOrEmpty(state.ImmediateText))
                {
                    _immediateWindow.SetText(state.ImmediateText);
                }
            }
            catch
            {
                // Ignore malformed state; user can reposition and we'll save again.
            }
        }

        public static void Save()
        {
            if (_windowManager == null)
                return;

            var state = new ServiceLayoutState
            {
                MainWidth = GameWindowManager.GetClientWidth(),
                MainHeight = GameWindowManager.GetClientHeight(),
                Windows = new List<WindowLayoutDto>(),
                ImmediateText = _immediateWindow?.GetText() ?? string.Empty
            };

            foreach (var w in _windowManager.Windows)
            {
                state.Windows.Add(new WindowLayoutDto
                {
                    Title = w.Title,
                    X = w.X,
                    Y = w.Y,
                    Width = w.Width,
                    Height = w.Height,
                    Visible = w.Visible
                });
            }

            var json = JsonSerializer.Serialize(state, ServiceLayoutContext.Default.ServiceLayoutState);
            File.WriteAllText(LayoutPath, json);
        }

        /// <summary>
        /// Apply the saved window size when entering service mode.
        /// </summary>
        public static void OnServiceModeEntered()
        {
            if (_pendingMainWidth > 0 && _pendingMainHeight > 0)
            {
                Renderer.SetPreferredBackBufferSize(_pendingMainWidth, _pendingMainHeight);
            }
        }

        /// <summary>
        /// Capture current window size before switching into service mode.
        /// </summary>
        public static void CapturePreServiceSize()
        {
            _preServiceWidth = GameWindowManager.GetClientWidth();
            _preServiceHeight = GameWindowManager.GetClientHeight();
            _hasPreServiceSize = true;
        }

        /// <summary>
        /// Restore window size when leaving service mode, if captured.
        /// </summary>
        public static void RestorePreServiceSize()
        {
            if (_hasPreServiceSize && _preServiceWidth > 0 && _preServiceHeight > 0)
            {
                Renderer.SetPreferredBackBufferSize(_preServiceWidth, _preServiceHeight);
            }
        }
    }

    public class ServiceLayoutState
    {
        public int MainWidth { get; set; }
        public int MainHeight { get; set; }
        public List<WindowLayoutDto> Windows { get; set; } = [];
        public string ImmediateText { get; set; }
    }

    public class WindowLayoutDto
    {
        public string Title { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Visible { get; set; }
    }
}

