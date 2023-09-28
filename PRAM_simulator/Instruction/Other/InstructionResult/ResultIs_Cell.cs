using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A class that represents a result of an instruction, that is: S2 := S3
    internal class ResultIs_Cell : ResultSubGroup
    {
        public int CellIndex { get; private set; }
        public ResultIs_Cell(int cellIndex)
        {
            CellIndex = cellIndex;
        }
        public int GetResult(Gateway gateway)
        {
            return gateway.SharedMemory.Read(CellIndex).Value;
        }
    }
}
