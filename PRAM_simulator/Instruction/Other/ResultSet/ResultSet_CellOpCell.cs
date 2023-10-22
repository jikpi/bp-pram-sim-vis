﻿using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A class that represents a result of an instruction, with a left cell, operator, and right cell, that is for example S2 := S2 + S3
    internal class ResultSet_CellOpCell : IResultSet
    {
        public int LeftCellIndex { get; private set; }
        public int RightCellIndex { get; private set; }
        public Operation Operation { get; private set; }

        public ResultSet_CellOpCell(int leftCellIndex, int rightCellIndex, Operation operation)
        {
            LeftCellIndex = leftCellIndex;
            RightCellIndex = rightCellIndex;
            Operation = operation;
        }

        public int GetResult(MasterGateway gateway)
        {
            // Get values of the cells, that will be used in the operation
            int left = gateway.SharedMemory.Read(LeftCellIndex).Value;
            int right = gateway.SharedMemory.Read(RightCellIndex).Value;
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
    }
}
