using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    // A class that represents a "comparison set" between two values, whether cell or constant on either side, for example "(S0 == S1)".
    // Is used in IfJumpTo instruction, to determine whether to jump or not.
    // Determines whether the comparison is between a cell and a cell, value and a value, etc, by checking for null values, which
    // combinations determine what is being compared.
    internal class ComparisonSet : IComparisonSet
    {
        internal GatewayIndexSet? leftGateway { get; set; }
        internal GatewayIndexSet? rightGateway { get; set; }
        internal int? LeftValue;
        internal int? RightValue;
        internal ComparisonMethod? ComparisonMethod;

        public ComparisonSet(GatewayIndexSet? leftGateway = null, GatewayIndexSet? rightGateway = null, ComparisonMethod? method = null, int? leftValue = null, int? rightValue = null)
        {
            this.leftGateway = leftGateway;
            this.rightGateway = rightGateway;
            LeftValue = leftValue;
            RightValue = rightValue;
            ComparisonMethod = method;
        }

        public virtual bool GetResult()
        {
            if (ComparisonMethod == null)
                throw new Exception("Debug: ComparisonMethod is null");

            int leftValue;
            int rightValue;

            // Determine what is being compared, get the values, and compare them. Then return the result.
            if (leftGateway != null)
                leftValue = leftGateway.Read();
            else if (LeftValue != null)
                leftValue = LeftValue.Value;
            else
                throw new Exception("Debug: Left is none");

            if (rightGateway != null)
                rightValue = rightGateway.Read();
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
