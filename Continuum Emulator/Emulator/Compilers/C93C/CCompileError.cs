using System;

namespace Continuum93.Emulator.Compilers.C93C
{
    public sealed class CCompileError
    {
        public string File { get; }
        public int Line { get; }
        public int Column { get; }
        public string Message { get; }

        public CCompileError(string file, int line, int column, string message)
        {
            File = file;
            Line = line;
            Column = column;
            Message = message;
        }

        public override string ToString() => $"{File}:{Line}:{Column}: error: {Message}";
    }

    /// <summary>Thrown to abort compilation after an unrecoverable error (the error is already recorded).</summary>
    internal sealed class CCompileAbort : Exception
    {
    }
}
