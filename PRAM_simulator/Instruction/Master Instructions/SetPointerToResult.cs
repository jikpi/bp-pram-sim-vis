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
    //A class that represents a [S1] := <RESULT> instruction, where <RESULT> is a ResultSet
    internal class SetPointerToResult : IInstruction
    {
        public int InstructionPointerIndex { get; set; }
        public int CodeInstructionLineIndex { get; set; }
        public int LeftPointingMemoryIndex { get; set; }
        public IResultSet RightValueMemoryIndex { get; set; }

        public SetPointerToResult(int leftPointingMemoryIndex, IResultSet rightValueMemoryIndex, int virtualInstructionIndex, int codeInstructionIndex)
        {
            LeftPointingMemoryIndex = leftPointingMemoryIndex;
            RightValueMemoryIndex = rightValueMemoryIndex;
            InstructionPointerIndex = virtualInstructionIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
        }

        public virtual void Execute(IGatewayAccessLocal gateway)
        {
            // Get value of the cell, that will be used as a pointer
            //int pointed = gateway.SharedMemory.Read(LeftPointingSharedMemoryIndex).Value;
            int pointed = gateway.Read(LeftPointingMemoryIndex);

            // Get the resulting value of ResultSet (<RESULT>)
            int value = RightValueMemoryIndex.GetResult(gateway);

            // Write the value to the pointed cell
            //gateway.SharedMemory.Write(pointed, value);
            gateway.Write(pointed, value);

        }


    }
}
