using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Machine.Container
{
    public enum ParallelAccessType
    {
        Read,
        Write
    }
    public class ParallelAccessInfo
    {
        public ParallelAccessType Type { get; private set; }
        public int MemoryIndex { get; private set; }
        public int ParallelMachineIndex { get; private set; }
        public int CodeLineIndex { get; private set; }

        public ParallelAccessInfo(ParallelAccessType type, int index, int parallelMachineIndex, int codeLineIndex)
        {
            Type = type;
            MemoryIndex = index;
            ParallelMachineIndex = parallelMachineIndex;
            CodeLineIndex = codeLineIndex;
        }
    }
}
