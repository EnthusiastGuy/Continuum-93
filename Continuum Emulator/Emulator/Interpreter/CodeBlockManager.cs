using System.Collections.Generic;

namespace Continuum93.Emulator.Interpreter
{
    public class CodeBlockManager
    {
        private readonly List<CodeBlock> _blocks = new();
        private readonly List<string> _collisions = new();

        public List<CodeBlock> GetBlocks()
        {
            return _blocks;
        }

        public void AddBlock(uint orgAddress)
        {
            _blocks.Add(new CodeBlock(orgAddress));
        }

        public void AddDataToBlockId(byte[] data, int index)
        {
            _blocks[index].Data = data;
            _blocks[index].End = (uint)(_blocks[index].Start + data.Length);
        }

        public int BlocksCount()
        {
            return _blocks.Count;
        }

        public void Reset()
        {
            _blocks.Clear();
            _collisions.Clear();
        }

        public void Validate()
        {
            for (int i = 0; i < _blocks.Count; i++)
                for (int j = 0; j < _blocks.Count && j != i; j++)
                {
                    bool startIncluded = _blocks[i].Start >= _blocks[j].Start && _blocks[i].Start <= _blocks[j].End;
                    bool endIncluded = _blocks[i].End <= _blocks[j].End && _blocks[i].End >= _blocks[j].Start;

                    if (startIncluded && endIncluded)   // Check if complete inclusion
                    {
                        _collisions.Add(
                            string.Format("Blocks starting at address {1} completely includes block starting at {0} ({2} bytes)",
                            _blocks[i].Start, _blocks[j].Start, _blocks[i].End - _blocks[i].Start
                            ));
                    }
                    else if (startIncluded)           // Check if partial upward inclusion
                    {
                        _collisions.Add(
                            string.Format("Blocks starting at addresses {0} and {1} collide for {2} bytes",
                            _blocks[i].Start, _blocks[j].Start, _blocks[j].End - _blocks[i].Start
                            ));
                    }
                    else if (endIncluded)             // Check if partial downward inclusion
                    {
                        _collisions.Add(
                            string.Format("Blocks starting at addresses {0} and {1} collide for {2} bytes",
                            _blocks[i].Start, _blocks[j].Start, _blocks[j].End - _blocks[i].Start
                            ));
                    }
                }
        }

        public bool HasCollisions()
        {
            return _collisions.Count > 0;
        }

        public List<string> GetCollisions()
        {
            return _collisions;
        }
    }
}
