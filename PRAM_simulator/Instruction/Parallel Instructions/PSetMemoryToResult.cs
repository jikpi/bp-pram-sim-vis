using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Master_Instructions;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Parallel_Instructions
{
    internal class PSetMemoryToResult : SetMemoryToResult, IParallelInstruction
    {
        public IGatewayAccessParallel MasterGateway { get; set; }

        public PSetMemoryToResult(IGatewayAccessParallel masterGateway, int sharedMemoryIndex, IResultSet result, int virtualInstructionIndex, int codeInstructionIndex) : base(sharedMemoryIndex, result, virtualInstructionIndex, codeInstructionIndex)
        {
            MasterGateway = masterGateway;
        }

        public override void Execute(IGatewayAccessLocal localGateway)
        {
            // Write to shared memory at specified index from result

            //MasterGateway.PWrite(MemoryIndex, Result.GetResult(localGateway));
            MasterGateway.PWrite(MemoryIndex, 256);
        }
    }
}
