using PRAM_lib.Code.Gateway;

namespace PRAM_lib.Instruction.Other.Interface
{
    internal interface IInstruction
    {
        public void Execute(); //A method that executes an instruction
        public int InstructionPointerIndex { get; } //An index of an instruction in a code memory
        public int CodeInstructionLineIndex { get; } //An index of the line in the code editor

        //A method that replaces a parallel gateway with a new one, used for deep copy of a processor
        public IInstruction DeepCopyToParallel(ParallelGateway gateway); 
    }
}
