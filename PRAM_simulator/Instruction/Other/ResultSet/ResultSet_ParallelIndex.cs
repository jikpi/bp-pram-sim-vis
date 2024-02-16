/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.ResultSet
{
    /// <summary>
    /// A result set holding the index of the parallel machine.
    /// </summary>
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

        public IResultSet DeepCopyToParallel(ParallelGateway gateway)
        {
            return new ResultSet_ParallelIndex(this.gateway.DeepCopyToParallel(gateway));
        }
    }
}
