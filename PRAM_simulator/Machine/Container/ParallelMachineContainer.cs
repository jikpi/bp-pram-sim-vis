using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Jumps;

namespace PRAM_lib.Machine.Container
{
    internal class ParallelMachineContainer
    {
        internal CodeMemory CodeMemory { get; private set; }
        internal JumpMemory JumpMemory { get; private set; }
        internal int NumberOfProcessors { get; private set; }
        internal ParallelGateway PGateway { get; set; }
        public ParallelMachineContainer(ParallelGateway gateway, CodeMemory codeMemory, JumpMemory jumpMemory, int numberOfProcessors)
        {
            PGateway = gateway;
            CodeMemory = codeMemory;
            JumpMemory = jumpMemory;
            NumberOfProcessors = numberOfProcessors;
        }
    }
}