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
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }
        GatewayIndexSet gateway { get; set; }
        private int Count { get; set; }

        public ParallelDo(GatewayIndexSet gateway, int instructionPointerIndex, int codeInstructionIndex, int count)
        {
            this.gateway = gateway;
            InstructionPointerIndex = instructionPointerIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
            Count = count;
        }
        public void Execute()
        {
            gateway.ParallelDo(Count);
        }
    }
}
