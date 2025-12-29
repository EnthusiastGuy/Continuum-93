using System;
using System.Collections.Generic;
using System.Text;
using Continuum93.Emulator;
using Continuum93.Emulator.Controls;
using Continuum93.Emulator.Interpreter;
using Continuum93.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Continuum93.ServiceModule.UI
{
    /// <summary>
    /// Simple immediate-mode editor and runner for assembly snippets.
    /// Text is persistent while the game runs; Ctrl+Enter compiles to 0xC00000 and jumps there.
    /// </summary>
    public class ImmediateWindow : Window
    {
        private const uint ImmediateAddress = 0xC00000;
        private readonly ServiceFont _font = Fonts.ModernDOS_12x18;
        private readonly int _lineHeight;
        private readonly int _charWidth;
        private const float CaretBlinkSeconds = 0.5f;
        private const float StatusSeconds = 3f;

        private readonly List<string> _lines = new() { "" };
        private int _cursorLine;
        private int _cursorColumn;
        private int _firstVisibleLine;

        private float _blinkTimer;
        private bool _caretVisible = true;

        private string _statusMessage = "";
        private float _statusTimer;

        public ImmediateWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
            _lineHeight = _font.GlyphCellHeight;
            _charWidth = _font.GlyphCellWidth;
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _blinkTimer += dt;
            if (_blinkTimer >= CaretBlinkSeconds)
            {
                _blinkTimer = 0f;
                _caretVisible = !_caretVisible;
            }

            if (_statusTimer > 0f)
            {
                _statusTimer -= dt;
                if (_statusTimer <= 0f)
                    _statusMessage = "";
            }

            if (!Service.STATE.UseServiceView || !IsFocused)
                return;

            var current = InputKeyboard.GetCurrentKeyboarsState();
            var previous = InputKeyboard.GetPreviousKeyboardState();

            bool ctrlDown = IsCtrlDown(current);

            // Compile + run
            if (ctrlDown && IsNewPress(Keys.Enter, current, previous))
            {
                CompileAndRun();
                return;
            }

            HandleNavigation(current, previous);
            HandleEditing(current, previous);

            EnsureCursorVisible(GetLinesPerPage());
        }

        private void HandleNavigation(KeyboardState current, KeyboardState previous)
        {
            if (IsNewPress(Keys.Left, current, previous))
                MoveLeft();
            if (IsNewPress(Keys.Right, current, previous))
                MoveRight();
            if (IsNewPress(Keys.Up, current, previous))
                MoveUp();
            if (IsNewPress(Keys.Down, current, previous))
                MoveDown();
            if (IsNewPress(Keys.Home, current, previous))
                MoveLineStart();
            if (IsNewPress(Keys.End, current, previous))
                MoveLineEnd();
        }

        private void HandleEditing(KeyboardState current, KeyboardState previous)
        {
            bool shift = IsShiftDown(current);
            bool ctrl = IsCtrlDown(current);

            if (!ctrl)
            {
                if (IsNewPress(Keys.Enter, current, previous))
                {
                    InsertNewLine();
                    return;
                }

                if (IsNewPress(Keys.Tab, current, previous))
                {
                    InsertText("    "); // 4 spaces
                    return;
                }
            }

            if (IsNewPress(Keys.Back, current, previous))
            {
                Backspace();
                return;
            }

            if (IsNewPress(Keys.Delete, current, previous))
            {
                Delete();
                return;
            }

            // Characters
            TryInsertCharacter(current, previous, shift);
        }

        private void TryInsertCharacter(KeyboardState current, KeyboardState previous, bool shift)
        {
            // Letters
            for (Keys key = Keys.A; key <= Keys.Z; key++)
            {
                if (IsNewPress(key, current, previous))
                {
                    char c = (char)((shift ? 'A' : 'a') + (key - Keys.A));
                    InsertChar(c);
                    return;
                }
            }

            // Digits + shifted symbols
            if (IsNewPress(Keys.D0, current, previous)) InsertChar(shift ? ')' : '0');
            else if (IsNewPress(Keys.D1, current, previous)) InsertChar(shift ? '!' : '1');
            else if (IsNewPress(Keys.D2, current, previous)) InsertChar(shift ? '@' : '2');
            else if (IsNewPress(Keys.D3, current, previous)) InsertChar(shift ? '#' : '3');
            else if (IsNewPress(Keys.D4, current, previous)) InsertChar(shift ? '$' : '4');
            else if (IsNewPress(Keys.D5, current, previous)) InsertChar(shift ? '%' : '5');
            else if (IsNewPress(Keys.D6, current, previous)) InsertChar(shift ? '^' : '6');
            else if (IsNewPress(Keys.D7, current, previous)) InsertChar(shift ? '&' : '7');
            else if (IsNewPress(Keys.D8, current, previous)) InsertChar(shift ? '*' : '8');
            else if (IsNewPress(Keys.D9, current, previous)) InsertChar(shift ? '(' : '9');
            else if (IsNewPress(Keys.NumPad0, current, previous)) InsertChar('0');
            else if (IsNewPress(Keys.NumPad1, current, previous)) InsertChar('1');
            else if (IsNewPress(Keys.NumPad2, current, previous)) InsertChar('2');
            else if (IsNewPress(Keys.NumPad3, current, previous)) InsertChar('3');
            else if (IsNewPress(Keys.NumPad4, current, previous)) InsertChar('4');
            else if (IsNewPress(Keys.NumPad5, current, previous)) InsertChar('5');
            else if (IsNewPress(Keys.NumPad6, current, previous)) InsertChar('6');
            else if (IsNewPress(Keys.NumPad7, current, previous)) InsertChar('7');
            else if (IsNewPress(Keys.NumPad8, current, previous)) InsertChar('8');
            else if (IsNewPress(Keys.NumPad9, current, previous)) InsertChar('9');
            else if (IsNewPress(Keys.Space, current, previous)) InsertChar(' ');
            else if (IsNewPress(Keys.OemComma, current, previous)) InsertChar(shift ? '<' : ',');
            else if (IsNewPress(Keys.OemPeriod, current, previous)) InsertChar(shift ? '>' : '.');
            else if (IsNewPress(Keys.OemQuestion, current, previous)) InsertChar(shift ? '?' : '/');
            else if (IsNewPress(Keys.OemSemicolon, current, previous)) InsertChar(shift ? ':' : ';');
            else if (IsNewPress(Keys.OemQuotes, current, previous)) InsertChar(shift ? '"' : '\'');
            else if (IsNewPress(Keys.OemOpenBrackets, current, previous)) InsertChar(shift ? '{' : '[');
            else if (IsNewPress(Keys.OemCloseBrackets, current, previous)) InsertChar(shift ? '}' : ']');
            else if (IsNewPress(Keys.OemMinus, current, previous)) InsertChar(shift ? '_' : '-');
            else if (IsNewPress(Keys.OemPlus, current, previous)) InsertChar(shift ? '+' : '=');
            else if (IsNewPress(Keys.OemPipe, current, previous) || IsNewPress(Keys.OemBackslash, current, previous)) InsertChar(shift ? '|' : '\\');
            else if (IsNewPress(Keys.OemTilde, current, previous)) InsertChar(shift ? '~' : '`');
        }

        private void InsertText(string text)
        {
            foreach (char c in text)
                InsertChar(c);
        }

        private void InsertChar(char c)
        {
            string line = _lines[_cursorLine];
            _lines[_cursorLine] = line.Insert(_cursorColumn, c.ToString());
            _cursorColumn++;
            EnsureCursorInBounds();
        }

        private void InsertNewLine()
        {
            string line = _lines[_cursorLine];
            string left = line.Substring(0, Math.Min(_cursorColumn, line.Length));
            string right = line.Substring(Math.Min(_cursorColumn, line.Length));
            _lines[_cursorLine] = left;
            _lines.Insert(_cursorLine + 1, right);
            _cursorLine++;
            _cursorColumn = 0;
        }

        private void Backspace()
        {
            if (_cursorColumn > 0)
            {
                string line = _lines[_cursorLine];
                _lines[_cursorLine] = line.Remove(_cursorColumn - 1, 1);
                _cursorColumn--;
            }
            else if (_cursorLine > 0)
            {
                int prevLen = _lines[_cursorLine - 1].Length;
                _lines[_cursorLine - 1] += _lines[_cursorLine];
                _lines.RemoveAt(_cursorLine);
                _cursorLine--;
                _cursorColumn = prevLen;
            }
            EnsureCursorInBounds();
        }

        private void Delete()
        {
            string line = _lines[_cursorLine];
            if (_cursorColumn < line.Length)
            {
                _lines[_cursorLine] = line.Remove(_cursorColumn, 1);
            }
            else if (_cursorLine < _lines.Count - 1)
            {
                _lines[_cursorLine] += _lines[_cursorLine + 1];
                _lines.RemoveAt(_cursorLine + 1);
            }
            EnsureCursorInBounds();
        }

        private void MoveLeft()
        {
            if (_cursorColumn > 0)
            {
                _cursorColumn--;
            }
            else if (_cursorLine > 0)
            {
                _cursorLine--;
                _cursorColumn = _lines[_cursorLine].Length;
            }
        }

        private void MoveRight()
        {
            if (_cursorColumn < _lines[_cursorLine].Length)
            {
                _cursorColumn++;
            }
            else if (_cursorLine < _lines.Count - 1)
            {
                _cursorLine++;
                _cursorColumn = 0;
            }
        }

        private void MoveUp()
        {
            if (_cursorLine > 0)
            {
                _cursorLine--;
                _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
            }
        }

        private void MoveDown()
        {
            if (_cursorLine < _lines.Count - 1)
            {
                _cursorLine++;
                _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
            }
        }

        private void MoveLineStart() => _cursorColumn = 0;

        private void MoveLineEnd() => _cursorColumn = _lines[_cursorLine].Length;

        private void EnsureCursorInBounds()
        {
            if (_cursorLine < 0) _cursorLine = 0;
            if (_cursorLine >= _lines.Count) _cursorLine = _lines.Count - 1;
            _cursorColumn = Math.Clamp(_cursorColumn, 0, _lines[_cursorLine].Length);
        }

        private void EnsureCursorVisible(int linesPerPage)
        {
            _firstVisibleLine = Math.Clamp(_firstVisibleLine, 0, Math.Max(0, _lines.Count - linesPerPage));
            if (_cursorLine < _firstVisibleLine)
                _firstVisibleLine = _cursorLine;
            else if (_cursorLine >= _firstVisibleLine + linesPerPage)
                _firstVisibleLine = _cursorLine - linesPerPage + 1;
        }

        private int GetLinesPerPage()
        {
            // Mirror DrawContent layout math
            int visibleHeight = Math.Max(1, ContentRect.Height - 4 - Padding);
            return Math.Max(1, visibleHeight / _lineHeight);
        }

        private void CompileAndRun()
        {
            var computer = Machine.COMPUTER;
            if (computer == null)
            {
                SetStatus("Machine not ready", true);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("#ORG 0xC00000");
            sb.AppendLine("#RUN 0xC00000");
            foreach (var line in _lines)
            {
                sb.AppendLine(line);
            }
            sb.AppendLine("RET");

            string source = sb.ToString();

            CompileLog.Reset();
            Assembler assembler = new();
            assembler.Build(source, "<immediate>");

            if (assembler.Errors > 0 || assembler.BlockManager.HasCollisions())
            {
                SetStatus($"Compile errors: {assembler.Errors}", true);
                return;
            }

            var blocks = assembler.BlockManager.GetBlocks();
            if (blocks.Count == 0)
            {
                SetStatus("No code generated", true);
                return;
            }

            foreach (var block in blocks)
            {
                computer.LoadMemAt(block.Start, block.Data);
            }

            // Push return address then jump to immediate code
            uint returnIpo = computer.CPU.REGS.IPO;
            computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, returnIpo);
            computer.CPU.REGS.IPO = ImmediateAddress;

            // If we are in step-by-step, run the snippet immediately and return
            if (DebugState.StepByStep)
            {
                const int maxSteps = 5000; // safety net
                bool returned = RunImmediateSynchronously(computer, returnIpo, maxSteps);
                if (!returned)
                {
                    SetStatus("Immediate did not return within limit", true);
                    return;
                }
            }
            else
            {
                // Let the machine run; ensure it continues if paused
                DebugState.MoveNext = true;
            }

            SetStatus("Ran immediate", false);
        }

        private static bool RunImmediateSynchronously(Computer computer, uint returnIpo, int maxSteps)
        {
            // Temporarily disable step-by-step gating while we execute the snippet
            bool prevStep = DebugState.StepByStep;
            bool prevMoveNext = DebugState.MoveNext;
            DebugState.StepByStep = false;
            DebugState.MoveNext = false;

            int steps = 0;
            while (steps < maxSteps)
            {
                computer.ExecuteNextInstruction();
                steps++;

                if (computer.CPU.REGS.IPO == returnIpo)
                {
                    break;
                }
            }

            // Restore step mode
            DebugState.StepByStep = prevStep;
            DebugState.MoveNext = prevMoveNext;

            return computer.CPU.REGS.IPO == returnIpo;
        }

        private void SetStatus(string message, bool isError)
        {
            _statusMessage = message;
            _statusTimer = StatusSeconds;
            if (isError)
                _caretVisible = true; // reset blink on errors for visibility
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            var pixel = Renderer.GetPixelTexture();

            int visibleHeight = Math.Max(1, contentRect.Height - 4 - Padding);
            int linesPerPage = Math.Max(1, visibleHeight / _lineHeight);
            EnsureCursorVisible(linesPerPage);

            int maxLineIndex = Math.Min(_lines.Count, _firstVisibleLine + linesPerPage);
            int y = contentRect.Y + Padding;
            int maxWidth = contentRect.Width - Padding * 2;

            for (int i = _firstVisibleLine; i < maxLineIndex; i++)
            {
                string line = _lines[i];

                // Current line highlight when focused
                if (IsFocused && i == _cursorLine)
                {
                    Rectangle lineRect = new(
                        contentRect.X + 1,
                        y - 1,
                        contentRect.Width - 2,
                        _lineHeight + 2
                    );
                    spriteBatch.Draw(pixel, lineRect, theme.WindowBackgroundFocused * 0.5f);
                }

                ServiceGraphics.DrawText(
                    _font,
                    line,
                    contentRect.X + Padding,
                    y,
                    maxWidth,
                    theme.TextPrimary,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.DrawOutline | ServiceFontFlags.Monospace | ServiceFontFlags.DisableKerning),
                    0xFF
                );

                // Caret
                if (IsFocused && i == _cursorLine && _caretVisible)
                {
                    int caretX = contentRect.X + Padding + _charWidth * _cursorColumn;
                    Rectangle caretRect = new(caretX, y, 2, _lineHeight - 2);
                    spriteBatch.Draw(pixel, caretRect, theme.TextPrimary);
                }

                y += _lineHeight;
            }

            // Status line at bottom
            if (!string.IsNullOrEmpty(_statusMessage))
            {
                ServiceGraphics.DrawText(
                    _font,
                    _statusMessage,
                    contentRect.X + Padding,
                    contentRect.Bottom - _lineHeight - Padding / 2,
                    maxWidth,
                    theme.TextPrimary,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.DrawOutline | ServiceFontFlags.Monospace | ServiceFontFlags.DisableKerning),
                    0xFF
                );
            }
        }

        private static bool IsNewPress(Keys key, KeyboardState current, KeyboardState previous) =>
            current.IsKeyDown(key) && previous.IsKeyUp(key);

        private static bool IsShiftDown(KeyboardState state) =>
            state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);

        private static bool IsCtrlDown(KeyboardState state) =>
            state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl);
    }
}

