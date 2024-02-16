/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway;

namespace PRAM_lib.Instruction.Other.InstructionResult.Interface
{
    public enum ComparisonMethod
    {
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual
    }
    public enum Operation
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod
    }

    /// <summary>
    /// For instruction "SetMemoryToResult", where SetMemoryToResult is only "S2 := " and the rest is all possible <RESULT>
    /// An interface for instruction result, that is for example S2 := <RESULT>
    /// </summary>
    internal interface IResultSet
    {
        public int GetResult();
        public IResultSet DeepCopyToParallel(ParallelGateway gateway);
    }

    /// <summary>
    /// For instruction "ComparisonSet"
    /// </summary>
    internal interface IComparisonSet
    {
        public bool GetResult();
        public IComparisonSet DeepCopyToParallel(ParallelGateway gateway);
    }
}
