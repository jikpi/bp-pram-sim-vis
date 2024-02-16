using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    /// <summary>
    /// Instruction that reads from input, and writes to shared memory. Optionally, it can read from specified index in input.
    /// </summary>
    internal class ReadInput : IInstruction
    {
        private GatewayIndexSet gateway { get; }
        public bool Sequential { get; }

        private int _readIndex;

        public int InputMemoryIndex
        {
            get => Sequential ? _readIndex : throw new Exception("Debug error");
            private set => _readIndex = value;
        }
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }

        public ReadInput(GatewayIndexSet gateway, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            Sequential = false;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public ReadInput(GatewayIndexSet gateway, int inputIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            InputMemoryIndex = inputIndex;
            Sequential = true;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            if (Sequential)
            {
                // Read from input memory at specified index, and write to shared memory at specified index

                gateway.Write(gateway.ReadInput(_readIndex));
            }
            else
            {
                // Read from input memory, and write to shared memory at specified index

                gateway.Write(gateway.ReadInput(null));
            }
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            throw new NotImplementedException();
        }
    }
}