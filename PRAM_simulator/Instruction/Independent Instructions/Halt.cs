using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Independent_Instructions
{
    internal class Halt : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }

        public GatewayIndexSet gateway;

        public Halt(GatewayIndexSet gateway, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.gateway = gateway;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            gateway.Halt();
        }
    }
}
