using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class SetMemoryToResult : IInstruction
    {
        public IResultSet Result { get; }
        public int CodeInstructionLineIndex { get; }
        public int InstructionPointerIndex { get; }
        public GatewayIndexSet Gateway { get; }

        public SetMemoryToResult(GatewayIndexSet gateway, IResultSet result, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.Gateway = gateway;
            Result = result;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public virtual void Execute()
        {
            // Write to memory at specified index from result
            Gateway.Write(Result.GetResult());
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            return new SetMemoryToResult(Gateway.DeepCopyToParallel(gateway), Result.DeepCopyToParallel(gateway), InstructionPointerIndex, CodeInstructionLineIndex);
        }
    }
}