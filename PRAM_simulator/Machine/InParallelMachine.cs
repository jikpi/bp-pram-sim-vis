using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Jumps;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;
using PRAM_lib.Processor.Interface;

namespace PRAM_lib.Processor
{
    internal class InParallelMachine : IProcessor
    {
        internal SharedMemory Memory { get; private set; }
        internal CodeMemory CodeMemory { get; private set; }
        internal JumpMemory JumpMemory { get; private set; }
        internal ParallelGateway Gateway { get; private set; }
        public string? ExecutionErrorMessage { get; private set; }
        public int? ExecutionErrorLineIndex { get; private set; }
        public bool IsCrashed { get; private set; }
        public bool IsHalted { get; private set; }
        public InstrPointer IP { get; private set; }
        public int ProcessorIndex { get; private set; }


        public InParallelMachine(int processorIndex, CodeMemory codeMemory, JumpMemory jumpMemory, ParallelGateway parallelGateway)
        {
            Memory = new SharedMemory();
            CodeMemory = codeMemory;
            JumpMemory = jumpMemory;
            IP = new InstrPointer(0);
            Gateway = parallelGateway;
            IsCrashed = false;
            IsHalted = false;
            ProcessorIndex = processorIndex;

            Gateway.Memory = Memory;
            Gateway.InstructionPointer = IP;
            Gateway.jumpMemory = JumpMemory;
        }
    }
}
