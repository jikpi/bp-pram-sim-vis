using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class IfJumpTo : IInstruction
    {
        public string JumpToLabel { get; set; }
        public ComparisonSet comparisonSet { get; set; }
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }
        public GatewayIndexSet gateway;

        public IfJumpTo(GatewayIndexSet gateway, string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex, ComparisonSet comparisonSet)
        {
            this.gateway = gateway;
            JumpToLabel = jumpToLabelString;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
            this.comparisonSet = comparisonSet;
        }

        public void Execute()
        {
            // Retrieve boolean result from ComparisonSet
            if (!comparisonSet.GetResult())
            {
                return;
            }

            // Get index of instruction to jump to, and set instruction pointer to that index
            int InstructionPointerIndex = gateway.GetJump(JumpToLabel);

            gateway.JumpTo(InstructionPointerIndex);
        }

    }
}
