using System.Collections.Generic;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Manages symbols (labels, variables) in the program.
    /// </summary>
    public class SymbolTable
    {
        private Dictionary<string, uint> _labels = new Dictionary<string, uint>();
        private Dictionary<string, SymbolInfo> _variables = new Dictionary<string, SymbolInfo>();

        public void AddLabel(string name, uint address)
        {
            _labels[name] = address;
        }

        public uint GetLabelAddress(string name)
        {
            return _labels.TryGetValue(name, out uint address) ? address : 0;
        }

        public bool HasLabel(string name)
        {
            return _labels.ContainsKey(name);
        }

        public void AddVariable(string name, SymbolInfo info)
        {
            _variables[name] = info;
        }

        public SymbolInfo GetVariable(string name)
        {
            return _variables.TryGetValue(name, out SymbolInfo info) ? info : null;
        }

        public bool HasVariable(string name)
        {
            return _variables.ContainsKey(name);
        }

        public IEnumerable<string> GetAllLabels()
        {
            return _labels.Keys;
        }

        public IEnumerable<string> GetAllVariables()
        {
            return _variables.Keys;
        }
    }

    public class SymbolInfo
    {
        public string Name { get; set; }
        public VariableType Type { get; set; }
        public uint Address { get; set; } // Kept for backward compatibility, but Label should be used instead
        public string Label { get; set; } // Label for the variable (e.g., ".var_int_i")
        public int Size { get; set; } // Size in bytes
        public List<int> ArrayDimensions { get; set; } = new List<int>(); // For arrays
        public bool IsArray => ArrayDimensions.Count > 0;
        public string InitialValue { get; set; } // For string literals (e.g., "Hello!")
    }
}

