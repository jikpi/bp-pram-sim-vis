using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A class that represents a result of an instruction, that is for example S2 := [S3]
    internal class ResultSet_Pointer : IResultSet
    {
        public int CellIndex { get; private set; }
        public ResultSet_Pointer(int cellIndex)
        {
            CellIndex = cellIndex;
        }

        public virtual int GetResult(IGatewayAccessLocal gateway)
        {
            // Get value of the cell, that will be used as a pointer
            //int pointed = gateway.SharedMemory.Read(CellIndex).Value;
            int pointed = gateway.Read(CellIndex);

            // Read the value from the pointed cell
            //return gateway.SharedMemory.Read(pointed).Value;
            return gateway.Read(pointed);
        }
    }
}
