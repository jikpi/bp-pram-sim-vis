using PRAM_lib.Instruction.Other.InstructionResult.Interface;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A constant result for an instruction, that is for example S2 := 5
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
    }
}
