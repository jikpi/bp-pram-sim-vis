using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class WritePointer : IInstruction
    {
        public int VirtualInstructionIndex { get; set; }
        public int CodeInstructionIndex { get; set; }
        public int LeftPointingSharedMemoryIndex { get; set; }
        public int RightValueSharedMemoryIndex { get; set; }

        public WritePointer(int leftPointingSharedMemoryIndex, int rightValueSharedMemoryIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            LeftPointingSharedMemoryIndex = leftPointingSharedMemoryIndex;
            RightValueSharedMemoryIndex = rightValueSharedMemoryIndex;
            VirtualInstructionIndex = virtualInstructionIndex;
            CodeInstructionIndex = codeInstructionIndex;
        }

        public void Execute(Gateway gateway)
        {
            int pointed = gateway.SharedMemory.Read(LeftPointingSharedMemoryIndex).Value;
            int value = gateway.SharedMemory.Read(pointed).Value;
            gateway.SharedMemory.Write(RightValueSharedMemoryIndex, value);
        }


    }
}
