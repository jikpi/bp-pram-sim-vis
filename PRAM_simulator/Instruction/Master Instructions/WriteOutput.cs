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
        public int MemoryIndex { get; set; }
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }

        public WriteOutput(int sharedMemoryIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            MemoryIndex = sharedMemoryIndex;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute(Gateway gateway)
        {
            // Write to output memory from shared memory at specified index
            gateway.OutputMemory.Write(gateway.SharedMemory.Read(MemoryIndex).Value);
        }

    }
}
