using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Independent_Instructions
{
    internal class NoOperation : IInstruction
    {
        public int InstructionPointerIndex { get; set; }

        public int CodeInstructionLineIndex { get; set; }

        public NoOperation(int virtualInstructionIndex, int codeInstructionIndex)
        {
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            //Do nothing
        }

        public void DeepCopyToParallel(IGateway gateway)
        {
            return;
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            return new NoOperation(InstructionPointerIndex, CodeInstructionLineIndex);
        }
    }
}
