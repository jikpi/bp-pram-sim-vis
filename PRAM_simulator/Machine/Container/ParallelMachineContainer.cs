using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Jumps;
using PRAM_lib.Processor;

namespace PRAM_lib.Machine.Container
{
    internal class ParallelMachineContainer
    {
        internal List<InParallelMachine> ParallelMachines { get; private set; }
        internal int NumberOfProcessors { get => ParallelMachines.Count; }
        public ParallelMachineContainer(List<InParallelMachine> inParallelMachines)
        {
            ParallelMachines = inParallelMachines;
        }
    }
}