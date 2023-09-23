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

    //For instruction "AssignResult", where AssignResult is only "S2 := " and the rest is all possible <RESULT>
    //An interface for instruction result, that is for example S2 := <RESULT>
    internal interface IInstructionResult
    {
        public int GetResult(Gateway gateway);
    }
}
