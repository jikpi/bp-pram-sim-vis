using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class WriteOutput : IInstruction
    {
        public IResultSet Result { get; set; }
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }

        public WriteOutput(IResultSet result, int virtualInstructionIndex, int codeInstructionIndex)
        {
            Result = result;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public virtual void Execute(IGatewayAccessLocal gateway)
        {
            // Write to output memory from shared memory at specified index
            //gateway.OutputMemory.Write(Result.GetResult(gateway));
            gateway.WriteOutput(Result.GetResult(gateway));
        }

    }
}
