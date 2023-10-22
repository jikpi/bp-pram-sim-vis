using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class JumpTo : IInstruction
    {
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }
        public string JumpToLabel { get; set; }

        public JumpTo(string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex)
        {
            JumpToLabel = jumpToLabelString;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute(MasterGateway gateway)
        {
            // Get index for the InstructionPointer to jump to
            int InstructionPointerIndex = gateway.jumpMemory.GetJump(JumpToLabel);
            gateway.InstructionPointer.Value = InstructionPointerIndex;
        }
    }
}
