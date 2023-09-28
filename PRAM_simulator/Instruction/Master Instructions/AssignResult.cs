using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class AssignResult : IInstruction
    {
        public int VirtualInstructionIndex { get; set; }
        public int SharedMemoryIndex { get; set; }
        public ResultSubGroup Result { get; set; }
        public int CodeInstructionIndex { get; set; }

        public AssignResult(int sharedMemoryIndex, ResultSubGroup result, int virtualInstructionIndex, int codeInstructionIndex)
        {
            SharedMemoryIndex = sharedMemoryIndex;
            Result = result;
            VirtualInstructionIndex = virtualInstructionIndex;
            CodeInstructionIndex = codeInstructionIndex;
        }

        public void Execute(Gateway gateway)
        {
            gateway.SharedMemory.Write(SharedMemoryIndex, Result.GetResult(gateway));
        }
    }
}
