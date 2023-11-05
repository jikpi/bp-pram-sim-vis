using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.Jumps;

namespace PRAM_lib.Machine.Container
{
    internal class ParallelMachineContainer
    {
        internal CodeMemory CodeMemory { get; private set; }
        internal JumpMemory JumpMemory { get; private set; }
        internal int NumberOfProcessors { get; private set; }
        public ParallelMachineContainer(CodeMemory codeMemory, JumpMemory jumpMemory, int numberOfProcessors)
        {
            CodeMemory = codeMemory;
            JumpMemory = jumpMemory;
            NumberOfProcessors = numberOfProcessors;
        }
    }
}