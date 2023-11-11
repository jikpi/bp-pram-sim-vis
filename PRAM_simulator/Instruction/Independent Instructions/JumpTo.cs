using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class JumpTo : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }
        public string JumpToLabel { get; }

        public GatewayIndexSet gateway;

        public JumpTo(GatewayIndexSet gateway, string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            JumpToLabel = jumpToLabelString;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            // Get index for the InstructionPointer to jump to
            int InstructionPointerIndex = gateway.GetJump(JumpToLabel);

            gateway.JumpTo(InstructionPointerIndex);
        }
    }
}
