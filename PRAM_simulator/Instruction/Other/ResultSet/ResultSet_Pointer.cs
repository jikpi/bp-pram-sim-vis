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
        GatewayIndexSet gateway { get; set; }
        public ResultSet_Pointer(GatewayIndexSet gateway)
        {
            this.gateway = gateway;
        }

        public virtual int GetResult()
        {
            // Get value of the cell, that will be used as a pointer
            int pointed = gateway.Read();

            // Read the value from the pointed cell
            return gateway.Read(pointed);
        }
    }
}
