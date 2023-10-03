using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.InstructionResult
{
    //A constant result for an instruction, that is for example S2 := 5
    internal class ResultSet_Constant : IResultSet
    {
        public int ConstantValue { get; private set; }

        public ResultSet_Constant(int constantValue)
        {
            ConstantValue = constantValue;
        }
        public int GetResult(Gateway gateway)
        {
            return ConstantValue;
        }
    }
}
