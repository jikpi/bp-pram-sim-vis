using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class JumpTo : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }
        public string JumpToLabel { get; }

        public GatewayIndexSet Gateway;

        public JumpTo(GatewayIndexSet gateway, string jumpToLabelString, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.Gateway = gateway;
            JumpToLabel = jumpToLabelString;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            // Get index for the InstructionPointer to jump to
            int InstructionPointerIndex = Gateway.GetJump(JumpToLabel);

            Gateway.JumpTo(InstructionPointerIndex);
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            return new JumpTo(Gateway.DeepCopyToParallel(gateway), JumpToLabel, InstructionPointerIndex, CodeInstructionLineIndex);
        }
    }
}
