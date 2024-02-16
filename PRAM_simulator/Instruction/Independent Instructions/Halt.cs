using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Independent_Instructions
{
    /// <summary>
    /// A class representing a halt instruction.
    /// </summary>
    internal class Halt : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }

        public GatewayIndexSet Gateway;

        public Halt(GatewayIndexSet gateway, int virtualInstructionIndex, int codeInstructionIndex)
        {
            this.Gateway = gateway;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public void Execute()
        {
            Gateway.Halt();
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            return new Halt(Gateway.DeepCopyToParallel(gateway), InstructionPointerIndex, CodeInstructionLineIndex);
        }
    }
}
