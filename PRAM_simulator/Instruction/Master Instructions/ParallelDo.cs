using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class ParallelDo : IInstruction
    {
        public int InstructionPointerIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CodeInstructionLineIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private int Count { get; set; }

        public ParallelDo(int instructionPointerIndex, int codeInstructionIndex, int count)
        {
            InstructionPointerIndex = instructionPointerIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
            Count = count;
        }
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
