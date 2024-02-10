using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class ParallelDo : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }
        private GatewayIndexSet gateway { get; }

        public ParallelDo(GatewayIndexSet gateway, int instructionPointerIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            InstructionPointerIndex = instructionPointerIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }
        public void Execute()
        {
            gateway.ParallelDo();
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            throw new NotImplementedException();
        }
    }
}
