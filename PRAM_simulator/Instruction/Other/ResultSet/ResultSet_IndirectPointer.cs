/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.ResultSet
{
    /// <summary>
    /// A result set that allows the reading of a value by pointers. Independent of the memory type.
    /// </summary>
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
