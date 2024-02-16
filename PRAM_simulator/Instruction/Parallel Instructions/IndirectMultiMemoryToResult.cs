/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Parallel_Instructions
{
    /// <summary>
    /// A class that represents a indirection between any memory
    /// For example P{L1} := {i}
    /// </summary>

    internal class IndirectMultiMemoryToResult : IInstruction
    {
        public int InstructionPointerIndex { get; }

        public int CodeInstructionLineIndex { get; }

        public GatewayIndexSet Gateway { get; }
        public IResultSet AddressingResult { get; }
        public IResultSet ValueResult { get; }

        public IndirectMultiMemoryToResult(GatewayIndexSet gateway, IResultSet addressingResult, IResultSet valueResult, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.Gateway = gateway;
            this.AddressingResult = addressingResult;
            this.ValueResult = valueResult;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            // Get value of the cell, that will be used as a pointer
            int pointed = AddressingResult.GetResult();

            // Get the resulting value of ResultSet (<RESULT>)
            int value = ValueResult.GetResult();

            // Write the value to the pointed cell
            Gateway.Write(pointed, value);
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            return new IndirectMultiMemoryToResult(Gateway.DeepCopyToParallel(gateway),
                AddressingResult.DeepCopyToParallel(gateway),
                ValueResult.DeepCopyToParallel(gateway), InstructionPointerIndex, CodeInstructionLineIndex);
        }
    }
}
