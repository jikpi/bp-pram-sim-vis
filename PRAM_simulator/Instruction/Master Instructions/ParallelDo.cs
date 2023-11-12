using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class ParallelDo : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }
        GatewayIndexSet gateway { get; }

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
    }
}
