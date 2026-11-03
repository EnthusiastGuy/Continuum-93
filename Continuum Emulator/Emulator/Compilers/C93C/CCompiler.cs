using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Continuum93.Emulator.Compilers.C93C
{
    /// <summary>
    /// Compiles a C source file to Continuum 93 assembly.
    ///
    /// Supported: char/short/int/long (8/16/32-bit, signed and unsigned), _Bool, pointers, arrays (multi-dimensional),
    /// structs and unions (packed), enums, typedefs, function pointers, all C operators, if/else, while, do/while, for,
    /// switch, break/continue, goto, static locals, global initializers, string literals, the preprocessor
    /// (#define with arguments, #include, conditionals) and inline assembly via asm("...").
    /// Not supported: floating point, 64-bit integers, structs passed or returned by value, variadic function
    /// definitions, bit-fields, designated initializers, compound literals and variable-length arrays.
    /// </summary>
    public sealed class CCompiler
    {
        public List<CCompileError> ErrorList { get; } = [];
        public int Errors => ErrorList.Count;
        public string Log { get; private set; } = "";
        public string GeneratedAssembly { get; private set; } = "";

        /// <summary>Resolves #include "name": receives the include name and the including file, returns source or null.</summary>
        public Func<string, string, string> IncludeResolver { get; set; }

        public uint CodeOrigin { get; set; } = CCodeGen.CodeOrigin;

        public string Compile(string source, string fileName = "main.c")
        {
            ErrorList.Clear();
            GeneratedAssembly = "";
            var log = new StringBuilder();

            try
            {
                var pp = new CPreprocessor(ResolveInclude, ErrorList);
                List<CToken> tokens = pp.Process(source, fileName);
                if (ErrorList.Count > 0)
                    throw new CCompileAbort();
                log.AppendLine($"Preprocessed {fileName}: {tokens.Count} tokens");

                var unit = new CTranslationUnit();
                var parser = new CParser(tokens, ErrorList, unit);
                parser.ParseTranslationUnit();

                // The runtime library is parsed into the same unit; user definitions take precedence
                var libPp = new CPreprocessor(ResolveInclude, ErrorList);
                List<CToken> libTokens = libPp.Process(CRuntime.LibrarySource, "<runtime>");
                if (ErrorList.Count > 0)
                    throw new CCompileAbort();
                parser.ParseAdditional(libTokens, libraryMode: true);

                CheckProgram(unit, parser, fileName);

                var gen = new CCodeGen(ErrorList, parser.LabelCounter);
                GeneratedAssembly = gen.Generate(unit, CodeOrigin);
                log.AppendLine($"Compiled {unit.Functions.Count} function(s), {unit.Globals.Count} global(s), {unit.Strings.Count} string literal(s)");
            }
            catch (CCompileAbort)
            {
                // Errors already recorded
            }

            foreach (CCompileError e in ErrorList)
                log.AppendLine(e.ToString());
            Log = log.ToString();

            return Errors == 0 ? GeneratedAssembly : "";
        }

        private string ResolveInclude(string name, string includingFile)
        {
            string builtin = CRuntime.GetHeader(name);
            if (builtin != null)
                return builtin;
            return IncludeResolver?.Invoke(name, includingFile);
        }

        private void CheckProgram(CTranslationUnit unit, CParser parser, string fileName)
        {
            CSymbol main = parser.AllGlobals.FirstOrDefault(s => s.Name == "main" && s.Kind == CSymbolKind.Function && s.IsDefined);
            if (main == null)
            {
                ErrorList.Add(new CCompileError(fileName, 1, 1, "Undefined reference to 'main'"));
                throw new CCompileAbort();
            }

            // Every referenced function/global must be defined somewhere (only checked for reachable code)
            var defined = new HashSet<string>(unit.Functions.Select(f => f.Symbol.Label));
            foreach (CSymbol g in unit.Globals.Where(g => g.IsDefined))
                defined.Add(g.Label);
            defined.Add(CCodeGen.HeapStartLabel);
            foreach (var s in unit.Strings)
                defined.Add(s.Label);

            var byLabel = unit.Functions.ToDictionary(f => f.Symbol.Label);
            var seen = new HashSet<string>();
            var work = new Stack<string>();
            work.Push(main.Label);
            foreach (CSymbol g in unit.Globals)
                foreach (CRelocation r in g.Relocations)
                    work.Push(r.TargetLabel);

            while (work.Count > 0)
            {
                string l = work.Pop();
                if (!seen.Add(l))
                    continue;
                if (!defined.Contains(l))
                {
                    string name = l.StartsWith(".c_") ? l[3..] : l;
                    ErrorList.Add(new CCompileError(fileName, 1, 1, $"Undefined reference to '{name}'"));
                    continue;
                }
                if (byLabel.TryGetValue(l, out CFunction f))
                    foreach (string c in f.CalledFunctions)
                        work.Push(c);
            }

            if (ErrorList.Count > 0)
                throw new CCompileAbort();
        }
    }
}
