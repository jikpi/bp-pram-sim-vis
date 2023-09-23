using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class WriteOutput : IInstruction
    {
        public int SharedMemoryIndex { get; set; }
        public int VirtualInstructionIndex { get; set; }

        public WriteOutput(int sharedMemoryIndex, int virtualInstructionIndex)
        {
            SharedMemoryIndex = sharedMemoryIndex;
            VirtualInstructionIndex = virtualInstructionIndex;
        }

        public void Execute(Gateway gateway)
        {
            gateway.OutputMemory.Write(gateway.SharedMemory.Read(SharedMemoryIndex).Value);
        }

    }
}
