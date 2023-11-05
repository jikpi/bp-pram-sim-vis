using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class SetMemoryToResult : IInstruction
    {
        public IResultSet Result { get; set; }
        public int CodeInstructionLineIndex { get; set; }
        public int InstructionPointerIndex { get; set; }
        public GatewayIndexSet gateway { get; set; }

        public SetMemoryToResult(GatewayIndexSet gateway, IResultSet result, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            Result = result;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public virtual void Execute()
        {
            // Write to memory at specified index from result
            gateway.Write(Result.GetResult());
        }
    }
}
