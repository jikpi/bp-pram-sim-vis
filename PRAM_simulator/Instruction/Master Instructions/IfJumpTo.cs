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
        public string JumpToLabel { get; set; }
        public ComparisonSet comparisonSet { get; set; }
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }

        public IfJumpTo(string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex, ComparisonSet comparisonSet)
        {
            JumpToLabel = jumpToLabelString;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
            this.comparisonSet = comparisonSet;
        }

        public void Execute(Gateway gateway)
        {
            // Retrieve result from ComparisonSet
            if(!comparisonSet.GetResult(gateway))
            {
                return;
            }

            // Get index of instruction to jump to, and set instruction pointer to that index
            int InstructionPointerIndex = gateway.jumpMemory.GetJump(JumpToLabel);
            gateway.InstructionPointer.Value = InstructionPointerIndex;
        }

    }
}
