using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    //A class that represents a [S1] := <RESULT> instruction, where <RESULT> is a ResultSet
    internal class SetPointerToResult : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }
        public GatewayIndexSet Gateway { get; }
        public IResultSet RightValueMemoryIndex { get; }

        public SetPointerToResult(GatewayIndexSet gateway, IResultSet rightValueMemoryIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.Gateway = gateway;
            RightValueMemoryIndex = rightValueMemoryIndex;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public virtual void Execute()
        {
            // Get value of the cell, that will be used as a pointer
            int pointed = Gateway.Read();

            // Get the resulting value of ResultSet (<RESULT>)
            int value = RightValueMemoryIndex.GetResult();

            // Write the value to the pointed cell
            Gateway.Write(pointed, value);

        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            return new SetPointerToResult(Gateway.DeepCopyToParallel(gateway), RightValueMemoryIndex.DeepCopyToParallel(gateway), InstructionPointerIndex, CodeInstructionLineIndex);
        }
    }
}
