using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A class that represents a result of an instruction, that is for example S2 := [S3]
    internal class ResultIs_CellPointer : IInstructionResult
    {
        public int CellIndex { get; private set; }
        public ResultIs_CellPointer(int cellIndex)
        {
            CellIndex = cellIndex;
        }

        public int GetResult(Gateway gateway)
        {
            int pointed = gateway.SharedMemory.Read(CellIndex).Value;
            return gateway.SharedMemory.Read(pointed).Value;
        }
    }
}
