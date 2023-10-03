using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{

    internal class ComparisonSet : IComparisonSet
    {
        internal int? LeftCell;
        internal int? RightCell;
        internal int? LeftValue;
        internal int? RightValue;
        internal ComparisonMethod? ComparisonMethod;

        public ComparisonSet(ComparisonMethod? method = null, int? leftCell = null, int? rightCell = null, int? leftValue = null, int? rightValue = null)
        {
            LeftCell = leftCell;
            RightCell = rightCell;
            LeftValue = leftValue;
            RightValue = rightValue;
            ComparisonMethod = method;
        }

        public bool GetResult(Gateway gateway)
        {
            if (ComparisonMethod == null)
                throw new Exception("Debug: ComparisonMethod is null");

            int leftValue;
            int rightValue;

            if (LeftCell != null)
                leftValue = gateway.SharedMemory.Read(LeftCell.Value).Value;
            else if (LeftValue != null)
                leftValue = LeftValue.Value;
            else
                throw new Exception("Debug: Left is none");

            if (RightCell != null)
                rightValue = gateway.SharedMemory.Read(RightCell.Value).Value;
            else if (RightValue != null)
                rightValue = RightValue.Value;
            else
                throw new Exception("Debug: Right is none");

            switch (this.ComparisonMethod)
            {
                case Interface.ComparisonMethod.Equal:
                    return leftValue == rightValue;
                case Interface.ComparisonMethod.NotEqual:
                    return leftValue != rightValue;
                case Interface.ComparisonMethod.Less:
                    return leftValue < rightValue;
                case Interface.ComparisonMethod.LessOrEqual:
                    return leftValue <= rightValue;
                case Interface.ComparisonMethod.Greater:
                    return leftValue > rightValue;
                case Interface.ComparisonMethod.GreaterOrEqual:
                    return leftValue >= rightValue;
                default:
                    throw new Exception("Debug: ComparisonMethod is not defined");
            }
        }
    }
}
