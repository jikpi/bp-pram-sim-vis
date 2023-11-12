using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.ResultSet
{
    internal class ResultSet_ParallelIndex : IResultSet
    {
        private GatewayIndexSet gateway { get; }
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
