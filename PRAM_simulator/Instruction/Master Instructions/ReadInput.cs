using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    //Instruction that reads from input, and writes to shared memory. Optionally, it can read from specified index in input.

    //POZNAMKA: IMPLEMENTACE READINPUT POUZE DO SHARED MEMORY
    internal class ReadInput : IInstruction
    {
        public int SharedMemoryIndex { get; private set; }
        public bool Sequential { get; private set; }

        private int _readIndex;

        public int InputMemoryIndex
        {
            get => Sequential ? _readIndex : throw new Exception("Debug error");
            private set => _readIndex = value;
        }
        public int VirtualInstructionIndex { get; set; }

        public ReadInput(int sharedMemoryIndex, int virtualInstructionIndex)
        {
            SharedMemoryIndex = sharedMemoryIndex;
            Sequential = false;
            VirtualInstructionIndex = virtualInstructionIndex;
        }

        public ReadInput(int sharedMemoryIndex, int inputIndex, int virtualInstructionIndex)
        {
            SharedMemoryIndex = sharedMemoryIndex;
            InputMemoryIndex = inputIndex;
            Sequential = true;
            VirtualInstructionIndex = virtualInstructionIndex;
        }

        public void Execute(Gateway gateway)
        {
            if (Sequential)
            {
                //gateway.SharedMemory.Cells[InputIndex].Value = gateway.InputMemory.Read(ReadIndex).Value;
                gateway.SharedMemory.Write(SharedMemoryIndex, gateway.InputMemory.Read(InputMemoryIndex).Value);
            }
            else
            {
                //gateway.SharedMemory.Cells[InputIndex].Value = gateway.InputMemory.Read().Value;
                gateway.SharedMemory.Write(SharedMemoryIndex, gateway.InputMemory.Read().Value);
            }
        }
    }
}