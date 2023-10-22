using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    //For instruction "SetMemoryToResult", where SetMemoryToResult is only "S2 := " and the rest is all possible <RESULT>
    //An interface for instruction result, that is for example S2 := <RESULT>
    internal interface IResultSet
    {
        public int GetResult(IGatewayAccessLocal gateway);
    }

    // For instruction "ComparisonSet"
    internal interface IComparisonSet
    {
        public bool GetResult(IGatewayAccessLocal gateway);
    }
}
