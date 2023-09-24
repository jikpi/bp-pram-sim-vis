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
        public int VirtualInstructionIndex { get; set; }
        public int CodeInstructionIndex { get; set; }
        public string JumpToLabel { get; set; }

        public JumpTo(string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex)
        {
            JumpToLabel = jumpToLabelString;
            VirtualInstructionIndex = virtualInstructionIndex;
            CodeInstructionIndex = codeInstructionIndex;
        }

        public void Execute(Gateway gateway)
        {
            int virtualIndex = gateway.jumpMemory.GetJump(JumpToLabel);
            gateway.InstructionPointer.Value = virtualIndex;
        }
    }
}
