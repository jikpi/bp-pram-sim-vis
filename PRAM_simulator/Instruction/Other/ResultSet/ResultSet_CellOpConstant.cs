using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A class that represents a result of an instruction, for example: S2 := S2 + 5
    internal class ResultSet_CellOpConstant : IResultSet
    {
        public int CellIndex { get; private set; }
        public int ConstantValue { get; private set; }
        public Operation operation { get; private set; }
        private bool IsLeftCell { get; set; }
        public ResultSet_CellOpConstant(int cellIndex, int constantValue, Operation operation, bool isLeftCell = true)
        {
            CellIndex = cellIndex;
            ConstantValue = constantValue;
            this.operation = operation;
            IsLeftCell = isLeftCell;
        }
        public int GetResult(Gateway gateway)
        {
            int RightValue = 0;
            int LeftValue = 0;

            if(IsLeftCell) 
            {
                LeftValue = gateway.SharedMemory.Read(CellIndex).Value;
                RightValue = ConstantValue;
            }
            else
            {
                LeftValue = ConstantValue;
                RightValue = gateway.SharedMemory.Read(CellIndex).Value;

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
                    if(RightValue == 0)
                        throw new LocalException("Division by zero");
                    return LeftValue / RightValue;
                case Operation.Mod:
                    return LeftValue % RightValue;
                default:
                    throw new Exception("Unknown operation");
            }
        }
    }
}
