using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Continuum93.Emulator.Interpreter;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Main BASIC compiler class. Compiles BASIC source code to assembly.
    /// </summary>
    public class BasicCompiler
    {
        private readonly Lexer _lexer;
        private readonly Parser _parser;
        private readonly Emitter _emitter;
        private readonly SymbolTable _symbolTable;
        private readonly VariableManager _variableManager;

        public int Errors { get; private set; }
        public string Log { get; private set; } = "";
        public string GeneratedAssembly { get; private set; } = "";

        public BasicCompiler()
        {
            _symbolTable = new SymbolTable();
            _variableManager = new VariableManager();
            _lexer = new Lexer();
            _parser = new Parser(_symbolTable);
            _emitter = new Emitter(_symbolTable, _variableManager);
        }

        /// <summary>
        /// Compiles BASIC source code to assembly.
        /// </summary>
        /// <param name="source">BASIC source code</param>
        /// <param name="codeStartAddress">Starting address for generated code (default: 0x080000)</param>
        /// <returns>Generated assembly code</returns>
        public string Compile(string source, uint codeStartAddress = 0x080000)
        {
            Errors = 0;
            Log = "";
            GeneratedAssembly = "";

            try
            {
                // Tokenize
                List<Token> tokens = _lexer.Tokenize(source);
                if (_lexer.Errors.Count > 0)
                {
                    foreach (var error in _lexer.Errors)
                    {
                        Log += $"Lexer error at line {error.Line}: {error.Message}\n";
                        Errors++;
                    }
                    return "";
                }

                // Debug: log tokens
                Log += $"Tokenized {tokens.Count} tokens:\n";
                foreach (var token in tokens.Take(20)) // First 20 tokens
                {
                    Log += $"  {token.Type} '{token.Value}' at {token.Line}:{token.Column}\n";
                }

                // Parse
                ProgramNode program = _parser.Parse(tokens);
                Log += $"Parsed program with {program?.Statements?.Count ?? 0} statements\n";
                if (program != null && program.Statements != null)
                {
                    foreach (var stmt in program.Statements)
                    {
                        Log += $"  Statement: {stmt.GetType().Name} at line {stmt.Line}\n";
                    }
                }
                if (program == null || program.Statements.Count == 0)
                {
                    Log += "Parser error: No statements found in program\n";
                    Errors++;
                    // Still try to emit to see what we have
                }
                if (_parser.Errors.Count > 0)
                {
                    foreach (var error in _parser.Errors)
                    {
                        Log += $"Parser error at line {error.Line}: {error.Message}\n";
                        Errors++;
                    }
                    // Don't return empty - try to emit what we can
                }

                // Emit assembly
                _emitter.SetCodeStartAddress(codeStartAddress);
                GeneratedAssembly = _emitter.Emit(program);
                
                // Add debug info to log
                if (program != null)
                {
                    Log += $"Parsed {program.Statements.Count} statements\n";
                    foreach (var stmt in program.Statements)
                    {
                        Log += $"  - {stmt.GetType().Name} at line {stmt.Line}\n";
                    }
                }
                
                if (_emitter.Errors.Count > 0)
                {
                    foreach (var error in _emitter.Errors)
                    {
                        Log += $"Emitter error: {error.Message}\n";
                        Errors++;
                    }
                }

                // Return assembly even if there are errors (for debugging)
                return GeneratedAssembly;
            }
            catch (Exception ex)
            {
                Log += $"Compiler error: {ex.Message}\n";
                Errors++;
                return "";
            }
        }

        /// <summary>
        /// Gets the run address (entry point) of the compiled program.
        /// </summary>
        public uint GetRunAddress()
        {
            return _emitter.GetRunAddress();
        }
    }
}

