using PRAM_lib.Code.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.InstructionResult.Interface
{
    public enum Operation
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod
    }

    //An interface for instruction result, that is for example S2 := <RESULT>
    internal interface IInstructionResult
    {
        public int GetResult(Gateway gateway);
    }
}
