using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other.Interface;

namespace PRAM_lib.Code.CodeMemory
{
    //A class that represents a memory of a processor
    internal class CodeMemory
    {
        internal List<IInstruction> Instructions { get; set; }
        public CodeMemory()
        {
            Instructions = new List<IInstruction>();
        }

        public CodeMemory DeepCopyToParallel(ParallelGateway parallelGateway)
        {
            CodeMemory newCodeMemory = new CodeMemory();
            foreach (IInstruction instruction in Instructions)
            {
                newCodeMemory.Instructions.Add(instruction.DeepCopyToParallel(parallelGateway));
            }
            return newCodeMemory;
        }
    }
}
