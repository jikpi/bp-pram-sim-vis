/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    /// <summary>
    /// A class representing an if-jump-to instruction.
    /// </summary>
    internal class IfJumpTo : IInstruction
    {
        public string JumpToLabel { get; }
        public ComparisonSet comparisonSet { get; }
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }
        public GatewayIndexSet Gateway;

        public IfJumpTo(GatewayIndexSet gateway, string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex, ComparisonSet comparisonSet)
        {
            this.Gateway = gateway;
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
            int InstructionPointerIndex = Gateway.GetJump(JumpToLabel);

            Gateway.JumpTo(InstructionPointerIndex);
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            GatewayIndexSet gatewayIndexSet = Gateway.DeepCopyToParallel(gateway);
            ComparisonSet comparisonSet = (ComparisonSet)this.comparisonSet.DeepCopyToParallel(gateway);
            return new IfJumpTo(gatewayIndexSet, JumpToLabel, InstructionPointerIndex, CodeInstructionLineIndex, comparisonSet);
        }
    }
}
