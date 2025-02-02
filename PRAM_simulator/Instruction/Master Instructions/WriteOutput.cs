﻿/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    /// <summary>
    /// A class representing a write-output instruction. Writes to output memory from shared memory at specified index.
    /// </summary>
    internal class WriteOutput : IInstruction
    {
        public IResultSet Result { get; }
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }

        private GatewayIndexSet gateway;

        public WriteOutput(GatewayIndexSet gateway, IResultSet result, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            Result = result;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public virtual void Execute()
        {
            // Write to output memory from shared memory at specified index
            gateway.WriteOutput(Result.GetResult());
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            throw new NotImplementedException();
        }
    }
}
