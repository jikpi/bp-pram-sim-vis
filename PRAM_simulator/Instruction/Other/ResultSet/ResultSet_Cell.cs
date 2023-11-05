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
    //A class that represents a result of an instruction, with a cell that is: S2 := S3
    internal class ResultSet_Cell : IResultSet
    {
        public GatewayIndexSet gateway { get; set; }
        public ResultSet_Cell(GatewayIndexSet gateway)
        {
            this.gateway = gateway;
        }
        public virtual int GetResult()
        {
            return gateway.Read();
        }
    }
}
