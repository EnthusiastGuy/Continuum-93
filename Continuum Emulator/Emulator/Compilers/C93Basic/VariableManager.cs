using System;
using System.Collections.Generic;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Manages variable memory allocation.
    /// </summary>
    public class VariableManager
    {
        private uint _codeEndAddress;

        public VariableManager()
        {
        }

        /// <summary>
        /// Sets the end address of the code section to allocate variables after it.
        /// </summary>
        public void SetCodeEndAddress(uint address)
        {
            _codeEndAddress = address;
        }

        /// <summary>
        /// Generates a label name for a variable.
        /// </summary>
        public string GenerateVariableLabel(string variableName, VariableType type)
        {
            // Remove type suffix from variable name (# for float, $ for string)
            string baseName = variableName.TrimEnd('#', '$');
            
            // Sanitize the name (replace invalid characters with underscores)
            baseName = System.Text.RegularExpressions.Regex.Replace(baseName, @"[^a-zA-Z0-9_]", "_");
            
            string typePrefix = type switch
            {
                VariableType.Integer => "int",
                VariableType.Float => "float",
                VariableType.String => "str",
                _ => "var"
            };
            
            return $".var_{typePrefix}_{baseName}";
        }

        /// <summary>
        /// Gets the size in bytes for a variable type.
        /// </summary>
        private int GetVariableSizeInternal(VariableType type, List<int> dimensions)
        {
            int elementSize = type switch
            {
                VariableType.Integer => 4,
                VariableType.Float => 4,
                VariableType.String => 256, // Max string length (can be adjusted)
                _ => 4
            };

            if (dimensions == null || dimensions.Count == 0)
            {
                return elementSize;
            }

            // Calculate array size
            int totalElements = 1;
            foreach (int dim in dimensions)
            {
                totalElements *= (dim + 1); // Arrays are 0-indexed, so size is dim+1
            }

            return totalElements * elementSize;
        }

        /// <summary>
        /// Gets the size in bytes for a variable type (for arrays).
        /// </summary>
        public int GetVariableSize(VariableType type, List<int> dimensions = null)
        {
            return GetVariableSizeInternal(type, dimensions ?? new List<int>());
        }
        
        /// <summary>
        /// Gets a dummy address for temporary buffers (not used with label-based variables).
        /// </summary>
        public uint GetVariableSectionEnd()
        {
            return 0; // Not used anymore, but kept for compatibility
        }
    }
}

