using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Continuum93.Emulator;
using Continuum93.Emulator.Controls;
using Continuum93.CodeAnalysis;
using Continuum93.Emulator.Mnemonics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Continuum93.Emulator.CPU;

namespace Continuum93.ServiceModule.UI
{
    /// <summary>
    /// Watcher window for Service Mode. Allows users to set conditions on registers,
    /// float registers, and memory. When all conditions are true, switches to step-by-step mode.
    /// </summary>
    public class WatcherWindow : Window
    {
        private readonly ServiceFont _font = Fonts.ModernDOS_12x18;
        private readonly int _lineHeight;
        private readonly int _charWidth;
        private const float CaretBlinkSeconds = 0.5f;

        private readonly List<string> _lines = new() { "" };
        private int _cursorLine;
        private int _cursorColumn;
        private int _firstVisibleLine;
        private bool _dirty;

        private float _blinkTimer;
        private bool _caretVisible = true;

        private readonly List<WatchCondition> _conditions = new();
        private bool _conditionsValid = true;

        public WatcherWindow(
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

            // Only evaluate conditions when Service Mode is enabled
            if (Service.STATE.ServiceMode && Machine.COMPUTER != null)
            {
                ParseAndEvaluateConditions();
            }

            if (!Service.STATE.UseServiceView || !IsFocused)
                return;

            var current = InputKeyboard.GetCurrentKeyboarsState();
            var previous = InputKeyboard.GetPreviousKeyboardState();

            HandleNavigation(current, previous);
            HandleEditing(current, previous);

            EnsureCursorVisible(GetLinesPerPage());
        }

        private void ParseAndEvaluateConditions()
        {
            _conditions.Clear();
            _conditionsValid = true;

            foreach (var line in _lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                var condition = ParseCondition(trimmed);
                if (condition != null)
                {
                    _conditions.Add(condition);
                }
                else
                {
                    _conditionsValid = false;
                }
            }

            // If all conditions are valid and all are true, switch to step-by-step
            if (_conditionsValid && _conditions.Count > 0)
            {
                bool allTrue = true;
                foreach (var condition in _conditions)
                {
                    if (!EvaluateCondition(condition))
                    {
                        allTrue = false;
                        break;
                    }
                }

                if (allTrue && !DebugState.StepByStep)
                {
                    DebugState.StepByStep = true;
                    DebugState.MoveNext = false;
                }
            }
        }

        private WatchCondition ParseCondition(string line)
        {
            // Remove comments (everything after ;)
            int commentIndex = line.IndexOf(';');
            if (commentIndex >= 0)
                line = line.Substring(0, commentIndex).Trim();

            if (string.IsNullOrEmpty(line))
                return null;

            // Match operators: =, !=, <, <=, >, >=
            var operatorPattern = @"\s*(==|!=|<=|>=|<|>)\s*";
            var match = Regex.Match(line, operatorPattern);
            if (!match.Success)
                return null;

            string op = match.Groups[1].Value;
            int opIndex = match.Index;
            string left = line.Substring(0, opIndex).Trim();
            string right = line.Substring(opIndex + match.Length).Trim();

            // Normalize = to == for consistency
            if (op == "=") op = "==";

            // Parse left side (register, float register, or memory)
            WatchCondition condition = new WatchCondition { Operator = op };

            // Memory condition: (0x000000) or (0x100000)
            if (left.StartsWith("(") && left.EndsWith(")"))
            {
                string addrStr = left.Substring(1, left.Length - 2).Trim();
                if (TryParseAddress(addrStr, out uint address))
                {
                    condition.Type = ConditionType.Memory;
                    condition.MemoryAddress = address;
                }
                else
                {
                    return null; // Invalid address
                }
            }
            // Float register: F0, F1, etc.
            else if (left.StartsWith("F", StringComparison.OrdinalIgnoreCase) && left.Length > 1)
            {
                string indexStr = left.Substring(1);
                if (byte.TryParse(indexStr, out byte index) && index <= 15)
                {
                    condition.Type = ConditionType.FloatRegister;
                    condition.FloatRegisterIndex = index;
                }
                else
                {
                    return null; // Invalid float register
                }
            }
            // Regular register: A, AB, XYZ, BCDE, etc.
            else
            {
                condition.Type = ConditionType.Register;
                condition.RegisterName = left.ToUpper();
            }

            // Parse right side (value)
            if (condition.Type == ConditionType.FloatRegister)
            {
                if (float.TryParse(right, out float floatValue))
                {
                    condition.FloatValue = floatValue;
                }
                else
                {
                    return null; // Invalid float value
                }
            }
            else
            {
                if (TryParseValue(right, out ulong value))
                {
                    condition.Value = value;
                }
                else
                {
                    return null; // Invalid value
                }
            }

            return condition;
        }

        private bool TryParseAddress(string str, out uint address)
        {
            address = 0;
            str = str.Trim();
            
            if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(2);
                if (uint.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out address))
                    return true;
            }
            else if (uint.TryParse(str, out address))
            {
                return true;
            }

            return false;
        }

        private bool TryParseValue(string str, out ulong value)
        {
            value = 0;
            str = str.Trim();

            if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(2);
                if (ulong.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out value))
                    return true;
            }
            else if (ulong.TryParse(str, out value))
            {
                return true;
            }

            return false;
        }

        private bool EvaluateCondition(WatchCondition condition)
        {
            if (Machine.COMPUTER == null)
                return false;

            var computer = Machine.COMPUTER;
            var regs = computer.CPU.REGS;
            var memc = computer.MEMC;

            if (condition.Type == ConditionType.Register)
            {
                ulong actualValue = GetRegisterValue(regs, condition.RegisterName);
                return CompareValues(actualValue, condition.Value, condition.Operator);
            }
            else if (condition.Type == ConditionType.FloatRegister)
            {
                float actualValue = computer.CPU.FREGS.GetRegister(condition.FloatRegisterIndex);
                return CompareFloatValues(actualValue, condition.FloatValue, condition.Operator);
            }
            else // Memory
            {
                byte actualValue = memc.Get8bitFromRAM(condition.MemoryAddress);
                return CompareValues(actualValue, condition.Value, condition.Operator);
            }
        }

        private ulong GetRegisterValue(Registers regs, string registerName)
        {
            // Check if it's a known register
            if (!Mnem.REG.TryGetValue(registerName, out byte index))
                return 0;

            // Determine size based on register name length
            int length = registerName.Length;
            if (length == 1)
            {
                // 8-bit register
                return regs.Get8BitRegister(index);
            }
            else if (length == 2)
            {
                // 16-bit register
                return regs.Get16BitRegister(index);
            }
            else if (length == 3)
            {
                // 24-bit register
                return regs.Get24BitRegister(index);
            }
            else if (length == 4)
            {
                // 32-bit register
                return regs.Get32BitRegister(index);
            }

            return 0;
        }

        private bool CompareValues(ulong actual, ulong expected, string op)
        {
            return op switch
            {
                "==" => actual == expected,
                "!=" => actual != expected,
                "<" => actual < expected,
                "<=" => actual <= expected,
                ">" => actual > expected,
                ">=" => actual >= expected,
                _ => false
            };
        }

        private bool CompareFloatValues(float actual, float expected, string op)
        {
            return op switch
            {
                "==" => Math.Abs(actual - expected) < float.Epsilon,
                "!=" => Math.Abs(actual - expected) >= float.Epsilon,
                "<" => actual < expected,
                "<=" => actual <= expected,
                ">" => actual > expected,
                ">=" => actual >= expected,
                _ => false
            };
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

            if (_dirty)
            {
                _dirty = false;
                ServiceLayoutManager.Save();
            }
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
            _dirty = true;
        }

        private void InsertChar(char c)
        {
            string line = _lines[_cursorLine];
            _lines[_cursorLine] = line.Insert(_cursorColumn, c.ToString());
            _cursorColumn++;
            EnsureCursorInBounds();
            _dirty = true;
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
            _dirty = true;
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
            _dirty = true;
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
            _dirty = true;
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

        public string GetText() => string.Join("\n", _lines);

        public void SetText(string text)
        {
            _lines.Clear();
            if (string.IsNullOrEmpty(text))
            {
                _lines.Add("");
            }
            else
            {
                var split = text.Replace("\r", string.Empty).Split('\n');
                _lines.AddRange(split);
            }
            _cursorLine = Math.Clamp(_cursorLine, 0, _lines.Count - 1);
            _cursorColumn = Math.Clamp(_cursorColumn, 0, _lines[_cursorLine].Length);
            _dirty = false;
        }

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
            int visibleHeight = Math.Max(1, ContentRect.Height - 4 - Padding);
            return Math.Max(1, visibleHeight / _lineHeight);
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

                // Color code: valid condition = normal, invalid = red
                Color lineColor = theme.TextPrimary;
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var trimmed = line.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        // Quick validation check
                        var testCondition = ParseCondition(trimmed);
                        if (testCondition == null)
                        {
                            lineColor = theme.TextIndianRed;
                        }
                    }
                }

                ServiceGraphics.DrawText(
                    _font,
                    line,
                    contentRect.X + Padding,
                    y,
                    maxWidth,
                    lineColor,
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
        }

        private static bool IsNewPress(Keys key, KeyboardState current, KeyboardState previous) =>
            current.IsKeyDown(key) && previous.IsKeyUp(key);

        private static bool IsShiftDown(KeyboardState state) =>
            state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);

        private static bool IsCtrlDown(KeyboardState state) =>
            state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl);
    }

    internal enum ConditionType
    {
        Register,
        FloatRegister,
        Memory
    }

    internal class WatchCondition
    {
        public ConditionType Type;
        public string RegisterName; // For register conditions
        public byte FloatRegisterIndex; // For float register conditions
        public uint MemoryAddress; // For memory conditions
        public ulong Value; // For register/memory conditions
        public float FloatValue; // For float register conditions
        public string Operator; // ==, !=, <, <=, >, >=
    }
}

