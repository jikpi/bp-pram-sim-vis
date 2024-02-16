/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    /// <summary>
    /// A class that represents a "comparison set" between two values, whether cell or constant on either side, for example "(S0 == S1)".
    /// Is used in IfJumpTo instruction, to determine whether to jump or not.
    /// Determines whether the comparison is between a cell and a cell, value and a value, etc, by checking for null values, which
    /// combinations determine what is being compared.
    /// </summary>
    internal class ComparisonSet : IComparisonSet
    {
        internal GatewayIndexSet? LeftGateway { get; }
        internal GatewayIndexSet? RightGateway { get; }
        internal int? LeftValue { get; }
        internal int? RightValue { get; }
        internal ComparisonMethod? ComparisonMethod { get; }

        public ComparisonSet(GatewayIndexSet? leftGateway = null, GatewayIndexSet? rightGateway = null, ComparisonMethod? method = null, int? leftValue = null, int? rightValue = null)
        {
            this.LeftGateway = leftGateway;
            this.RightGateway = rightGateway;
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
            if (LeftGateway != null)
                leftValue = LeftGateway.Read();
            else if (LeftValue != null)
                leftValue = LeftValue.Value;
            else
                throw new Exception("Debug: Left is none");

            if (RightGateway != null)
                rightValue = RightGateway.Read();
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

        public IComparisonSet DeepCopyToParallel(ParallelGateway gateway)
        {
            GatewayIndexSet? leftGateway = null;
            if (LeftGateway != null)
            {
                leftGateway = LeftGateway.DeepCopyToParallel(gateway);
            }

            GatewayIndexSet? rightGateway = null;
            if (RightGateway != null)
            {
                rightGateway = RightGateway.DeepCopyToParallel(gateway);
            }

            return new ComparisonSet(leftGateway, rightGateway, ComparisonMethod, LeftValue, RightValue);
        }
    }
}
