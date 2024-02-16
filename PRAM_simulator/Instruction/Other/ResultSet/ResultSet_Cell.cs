using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    /// <summary>
    /// A class that represents a result of an instruction, with a cell that is: S2 := S3
    /// </summary>
    internal class ResultSet_Cell : IResultSet
    {
        public GatewayIndexSet gateway { get; }
        public ResultSet_Cell(GatewayIndexSet gateway)
        {
            this.gateway = gateway;
        }
        public virtual int GetResult()
        {
            return gateway.Read();
        }

        public IResultSet DeepCopyToParallel(ParallelGateway gateway)
        {
            return new ResultSet_Cell(this.gateway.DeepCopyToParallel(gateway));
        }
    }
}
