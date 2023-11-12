using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Parallel_Instructions
{
    //A class that represents a indirection between local parallel machine memory and shared memory
    //For example P{L1} := {i}
    internal class IndirectMultiMemoryToResult : IInstruction
    {
        public int InstructionPointerIndex { get; }

        public int CodeInstructionLineIndex { get; }

        public GatewayIndexSet gateway { get; }
        public IResultSet addressingResult { get; }
        public IResultSet valueResult { get; }

        public IndirectMultiMemoryToResult(GatewayIndexSet gateway, IResultSet addressingResult, IResultSet valueResult, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            this.addressingResult = addressingResult;
            this.valueResult = valueResult;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            // Get value of the cell, that will be used as a pointer
            int pointed = addressingResult.GetResult();

            // Get the resulting value of ResultSet (<RESULT>)
            int value = valueResult.GetResult();

            // Write the value to the pointed cell
            gateway.Write(pointed, value);
        }
    }
}
