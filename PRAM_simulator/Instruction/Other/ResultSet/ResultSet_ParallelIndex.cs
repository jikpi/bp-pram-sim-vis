using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.ResultSet
{
    internal class ResultSet_ParallelIndex : IResultSet
    {
        GatewayIndexSet gateway { get; }
        public ResultSet_ParallelIndex(GatewayIndexSet gateway)
        {
            this.gateway = gateway;
        }

        public int GetResult()
        {
            return gateway.GetParallelIndex();
        }
    }
}
