using PRAM_lib.Code.Gateway.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.Interface
{
    internal interface IParallelInstruction
    {
        internal IGatewayAccessParallel MasterGateway { get; set; }

    }
}
