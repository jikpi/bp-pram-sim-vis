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
    internal class ResultIs_Cell2Constant : ResultSubGroup
    {
        public int CellIndex { get; private set; }
        public int ConstantValue { get; private set; }

        public Operation operation { get; private set; }
        public ResultIs_Cell2Constant(int cellIndex, int constantValue, Operation operation)
        {
            CellIndex = cellIndex;
            ConstantValue = constantValue;
            this.operation = operation;
        }
        public int GetResult(Gateway gateway)
        {
            int cellValue = gateway.SharedMemory.Read(CellIndex).Value;
            switch (operation)
            {
                case Operation.Add:
                    return cellValue + ConstantValue;
                case Operation.Sub:
                    return cellValue - ConstantValue;
                case Operation.Mul:
                    return cellValue * ConstantValue;
                case Operation.Div:
                    if(ConstantValue == 0)
                        throw new LocalException("Division by zero");
                    return cellValue / ConstantValue;
                case Operation.Mod:
                    return cellValue % ConstantValue;
                default:
                    throw new Exception("Unknown operation");
            }
        }
    }
}
