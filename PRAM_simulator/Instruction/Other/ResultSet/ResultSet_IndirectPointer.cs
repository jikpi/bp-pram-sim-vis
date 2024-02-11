using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.ResultSet
{
    internal class ResultSet_IndirectPointer : IResultSet
    {
        public GatewayIndexSet Gateway { get; }
        public IResultSet AddressingResult { get; }

        public ResultSet_IndirectPointer(GatewayIndexSet gateway, IResultSet addressingResult)
        {
            Gateway = gateway;
            AddressingResult = addressingResult;
        }
        public int GetResult()
        {
            // Get value of the result set, that will be used as a pointer
            int pointed = AddressingResult.GetResult();

            return Gateway.Read(pointed);
        }

        public IResultSet DeepCopyToParallel(ParallelGateway gateway)
        {
            return new ResultSet_IndirectPointer(Gateway.DeepCopyToParallel(gateway), AddressingResult.DeepCopyToParallel(gateway));
        }
    }
}
