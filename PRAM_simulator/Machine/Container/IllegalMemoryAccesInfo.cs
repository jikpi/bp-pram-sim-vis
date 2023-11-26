using PRAM_lib.Memory;
using System.Collections.ObjectModel;

namespace PRAM_lib.Machine.Container
{
    public class IllegalMemoryAccesInfo
    {
        public int? IllegalMemoryReadIndex { get; private set; }
        public int? IllegalMemoryWriteIndex { get; private set; }
        public List<int> IllegalMemoryParallelMachineIndexes { get; private set; }
        public bool IllegalRead { get; private set; }
        public bool IllegalWrite { get => !IllegalRead; }
        public ObservableCollection<MemoryCell> IllegalMemoryWhereAccessed { get; private set; }

        public IllegalMemoryAccesInfo(List<int> machineIndexes, ObservableCollection<MemoryCell> whereAccessed, int? readIndex = null, int? writeIndex = null)
        {
            IllegalMemoryParallelMachineIndexes = machineIndexes;
            IllegalMemoryWhereAccessed = whereAccessed;
            IllegalRead = readIndex != null;
            IllegalMemoryReadIndex = readIndex;
            IllegalMemoryWriteIndex = writeIndex;
        }
    }
}
