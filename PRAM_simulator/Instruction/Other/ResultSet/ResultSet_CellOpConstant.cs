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
    /// A class that represents a result of an instruction, for example: S2 := S2 + 5
    /// </summary>
    internal class ResultSet_CellOpConstant : IResultSet
    {
        public GatewayIndexSet Gateway { get; }
        public int ConstantValue { get; }
        public Operation operation { get; }
        private bool IsLeftCell { get; }
        public ResultSet_CellOpConstant(GatewayIndexSet gateway, int constantValue, Operation operation, bool isLeftCell = true)
        {
            this.Gateway = gateway;
            ConstantValue = constantValue;
            this.operation = operation;
            IsLeftCell = isLeftCell;
        }
        public virtual int GetResult()
        {
            int RightValue = 0;
            int LeftValue = 0;

            if (IsLeftCell)
            {
                LeftValue = Gateway.Read();
                RightValue = ConstantValue;
            }
            else
            {
                LeftValue = ConstantValue;
                RightValue = Gateway.Read();

            }


            switch (operation)
            {
                case Operation.Add:
                    return LeftValue + RightValue;
                case Operation.Sub:
                    return LeftValue - RightValue;
                case Operation.Mul:
                    return LeftValue * RightValue;
                case Operation.Div:
                    if (RightValue == 0)
                        throw new LocalException(ExceptionMessages.DivisionByZero());
                    return LeftValue / RightValue;
                case Operation.Mod:
                    return LeftValue % RightValue;
                default:
                    throw new Exception("Unknown operation");
            }
        }

        public IResultSet DeepCopyToParallel(ParallelGateway gateway)
        {
            return new ResultSet_CellOpConstant(this.Gateway.DeepCopyToParallel(gateway), ConstantValue, operation, IsLeftCell);
        }
    }
}
