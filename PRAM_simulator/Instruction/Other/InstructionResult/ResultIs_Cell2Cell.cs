using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A class that represents a result of an instruction, that is for example S2 := S2 + S3
    internal class ResultIs_Cell2Cell : ResultSubGroup
    {
        public int LeftCellIndex { get; private set; }
        public int RightCellIndex { get; private set; }
        public Operation Operation { get; private set; }

        public ResultIs_Cell2Cell(int leftCellIndex, int rightCellIndex, Operation operation)
        {
            LeftCellIndex = leftCellIndex;
            RightCellIndex = rightCellIndex;
            Operation = operation;
        }

        public int GetResult(Gateway gateway)
        {
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
                        throw new LocalException("Division by zero");
                    return left / right;
                case Operation.Mod:
                    return left % right;
                default:
                    throw new Exception("Debug exception");
            }
        }
    }
}
