using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    //Instruction that reads from input, and writes to shared memory. Optionally, it can read from specified index in input.

    //POZNAMKA: IMPLEMENTACE READINPUT POUZE DO SHARED MEMORY
    //TODO THIS IS A RESULT SET, REFACTOR AFTER PARALLELIZATION ARCHITECTURE IS DONE
    internal class ReadInput : IInstruction
    {
        public int MemoryIndex { get; private set; }
        public bool Sequential { get; private set; }

        private int _readIndex;

        public int InputMemoryIndex
        {
            get => Sequential ? _readIndex : throw new Exception("Debug error");
            private set => _readIndex = value;
        }
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }

        public ReadInput(int sharedMemoryIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            MemoryIndex = sharedMemoryIndex;
            Sequential = false;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public ReadInput(int sharedMemoryIndex, int inputIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            MemoryIndex = sharedMemoryIndex;
            InputMemoryIndex = inputIndex;
            Sequential = true;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute(IGatewayAccessLocal gateway)
        {
            if (Sequential)
            {
                // Read from input memory at specified index, and write to shared memory at specified index

                //gateway.SharedMemory.Write(MemoryIndex, gateway.InputMemory.Read(InputMemoryIndex).Value);
                gateway.Write(MemoryIndex, gateway.Read(InputMemoryIndex));
            }
            else
            {
                // Read from input memory, and write to shared memory at specified index

                //gateway.SharedMemory.Write(MemoryIndex, gateway.InputMemory.Read().Value);
                gateway.Write(MemoryIndex, gateway.ReadInput());
            }
        }
    }
}