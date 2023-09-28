using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class IfJumpTo : IInstruction
    {
        public int VirtualInstructionIndex { get; set; }
        public int CodeInstructionIndex { get; set; }
        public string JumpToLabel { get; set; }
        public ComparisonSet comparisonSet { get; set; }

        public IfJumpTo(string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex, ComparisonSet comparisonSet)
        {
            JumpToLabel = jumpToLabelString;
            VirtualInstructionIndex = virtualInstructionIndex;
            CodeInstructionIndex = codeInstructionIndex;
            this.comparisonSet = comparisonSet;
        }

        public void Execute(Gateway gateway)
        {
            if(!comparisonSet.GetResult(gateway))
            {
                return;
            }

            int virtualIndex = gateway.jumpMemory.GetJump(JumpToLabel);
            gateway.InstructionPointer.Value = virtualIndex;
        }

    }
}
