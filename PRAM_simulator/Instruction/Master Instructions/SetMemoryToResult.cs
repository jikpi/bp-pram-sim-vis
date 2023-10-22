using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class SetMemoryToResult : IInstruction
    {
        public IResultSet Result { get; set; }
        public int CodeInstructionLineIndex { get; set; }
        public int InstructionPointerIndex { get; set; }
        public int MemoryIndex { get; set; }

        public SetMemoryToResult(int sharedMemoryIndex, IResultSet result, int virtualInstructionIndex, int codeInstructionIndex)
        {
            MemoryIndex = sharedMemoryIndex;
            Result = result;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute(MasterGateway gateway)
        {
            // Write to memory at specified index from result
            gateway.SharedMemory.Write(MemoryIndex, Result.GetResult(gateway));
        }
    }
}
