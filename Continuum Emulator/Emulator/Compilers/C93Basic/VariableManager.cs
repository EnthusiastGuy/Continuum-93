using System;
using System.Collections.Generic;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Manages variable memory allocation.
    /// </summary>
    public class VariableManager
    {
        private uint _nextVariableAddress;
        private uint _codeEndAddress;
        private const uint VARIABLE_SECTION_START = 0x100000; // Start variables after code section

        public VariableManager()
        {
            _nextVariableAddress = VARIABLE_SECTION_START;
        }

        /// <summary>
        /// Sets the end address of the code section to allocate variables after it.
        /// </summary>
        public void SetCodeEndAddress(uint address)
        {
            _codeEndAddress = address;
            _nextVariableAddress = Math.Max(_codeEndAddress, VARIABLE_SECTION_START);
        }

        /// <summary>
        /// Allocates memory for a variable.
        /// </summary>
        public uint AllocateVariable(VariableType type, List<int> dimensions = null)
        {
            int size = GetVariableSize(type, dimensions);
            uint address = _nextVariableAddress;
            _nextVariableAddress += (uint)size;
            return address;
        }

        /// <summary>
        /// Gets the size in bytes for a variable type.
        /// </summary>
        private int GetVariableSize(VariableType type, List<int> dimensions)
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
        /// Gets the current variable section end address.
        /// </summary>
        public uint GetVariableSectionEnd()
        {
            return _nextVariableAddress;
        }
    }
}

