using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Instruction.Master_Instructions
{
    internal class ParallelDo : IInstruction
    {
        public int InstructionPointerIndex { get; }
        public int CodeInstructionLineIndex { get; }
        private GatewayIndexSet gateway { get; }
        private IResultSet NumberOfProcessors { get; set; }
        private int ParallelIndex { get; set; }

        public ParallelDo(GatewayIndexSet gateway, int instructionPointerIndex, int codeInstructionIndex, IResultSet numberOfProcessors, int parallelIndex)
        {
            this.gateway = gateway;
            InstructionPointerIndex = instructionPointerIndex;
            CodeInstructionLineIndex = codeInstructionIndex;
            NumberOfProcessors = numberOfProcessors;
            ParallelIndex = parallelIndex;
        }
        public void Execute()
        {
            int numberOfProcessors = NumberOfProcessors.GetResult();
            if(numberOfProcessors == 0)
            {
                return;
            }

            if(numberOfProcessors < 0 || numberOfProcessors > 10000)
            {
                throw new LocalException(ExceptionMessages.BadNumberOfParallelProcessors(numberOfProcessors));
            }

            gateway.ParallelDo(numberOfProcessors, ParallelIndex);
        }

        public IInstruction DeepCopyToParallel(ParallelGateway gateway)
        {
            throw new NotImplementedException();
        }
    }
}
