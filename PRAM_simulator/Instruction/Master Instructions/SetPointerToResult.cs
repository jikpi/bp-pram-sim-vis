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
    //A class that represents a [S1] := <RESULT> instruction, where <RESULT> is a InstructionResult
    internal class SetPointerToResult : IInstruction
    {
        public int VirtualInstructionIndex { get; set; }
        public int CodeInstructionIndex { get; set; }
        public int LeftPointingSharedMemoryIndex { get; set; }
        public IResultSet RightValueSharedMemoryIndex { get; set; }

        public SetPointerToResult(int leftPointingSharedMemoryIndex, IResultSet rightValueSharedMemoryIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            LeftPointingSharedMemoryIndex = leftPointingSharedMemoryIndex;
            RightValueSharedMemoryIndex = rightValueSharedMemoryIndex;
            VirtualInstructionIndex = virtualInstructionIndex;
            CodeInstructionIndex = codeInstructionIndex;
        }

        public void Execute(Gateway gateway)
        {
            int pointed = gateway.SharedMemory.Read(LeftPointingSharedMemoryIndex).Value;
            int value = RightValueSharedMemoryIndex.GetResult(gateway);
            gateway.SharedMemory.Write(pointed, value);
        }


    }
}
