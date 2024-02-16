using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    /// <summary>
    /// A constant result for an instruction, that is for example S2 := 5
    /// </summary>
    internal class ResultSet_Constant : IResultSet
    {
        public int ConstantValue { get; }

        public ResultSet_Constant(int constantValue)
        {
            ConstantValue = constantValue;
        }
        public virtual int GetResult()
        {
            return ConstantValue;
        }

        public IResultSet DeepCopyToParallel(ParallelGateway gateway)
        {
            return new ResultSet_Constant(ConstantValue);
        }
    }
}
