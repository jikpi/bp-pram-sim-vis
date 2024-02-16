/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    /// <summary>
    /// A class that represents a result of an instruction, with a left cell, operator, and right cell, that is for example S2 := S2 + S3
    /// </summary>
    internal class ResultSet_CellOpCell : IResultSet
    {
        public GatewayIndexSet leftGateway { get; }
        public GatewayIndexSet rightGateway { get; }
        public Operation Operation { get; }

        public ResultSet_CellOpCell(GatewayIndexSet leftGateway, GatewayIndexSet rightGateway, Operation operation)
        {
            this.leftGateway = leftGateway;
            this.rightGateway = rightGateway;
            Operation = operation;
        }

        public virtual int GetResult()
        {
            // Get values of the cells, that will be used in the operation
            int left = leftGateway.Read();
            int right = rightGateway.Read();
            switch (Operation)
            {
                case Operation.Add:
                    return left + right;
                case Operation.Sub:
                    return left - right;
                case Operation.Mul:
                    return left * right;
                case Operation.Div:
                    if (right == 0)
                        throw new LocalException(ExceptionMessages.DivisionByZero());
                    return left / right;
                case Operation.Mod:
                    return left % right;
                default:
                    throw new Exception("Debug exception");
            }
        }

        public IResultSet DeepCopyToParallel(ParallelGateway gateway)
        {
            return new ResultSet_CellOpCell(leftGateway.DeepCopyToParallel(gateway),
                rightGateway.DeepCopyToParallel(gateway), Operation);
        }
    }
}
