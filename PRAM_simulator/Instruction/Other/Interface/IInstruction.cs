namespace PRAM_lib.Instruction.Other.Interface
{
    internal interface IInstruction
    {
        public void Execute(); //A method that executes an instruction
        public int InstructionPointerIndex { get; } //An index of an instruction in a code memory
        public int CodeInstructionLineIndex { get; } //An index of the line in the code editor
    }
}
