using System.Text.RegularExpressions;

namespace PRAM_lib.Instruction.Other
{
    internal class InstructionRegex
    {
        public Regex Comment { get; set; } //Not a real instruction
        public Regex ReadInput { get; set; }
        public Regex WriteOutput { get; set; }
        public Regex SetMemoryToResult { get; set; }
        public Regex ResultSet_Cell { get; set; }
        public Regex ResultSet_CellOpCell { get; set; }
        public Regex ResultSet_CellOpConstant { get; set; }
        public Regex ResultSet_ConstantOpCell { get; set; } //Supplemental regex for ResultSet_CellOpConstant alternative
        public Regex ResultSet_Pointer { get; set; }
        public Regex ResultSet_Constant { get; set; }
        public Regex SetPointerToResult { get; set; }
        public Regex JumpToInstruction { get; set; }
        public Regex JumpToLabel { get; set; } //Not a real instruction
        public Regex IfJumpTo { get; set; }

        public InstructionRegex()
        {
            Comment = new Regex(@"^#.*$"); //0 groups

            ReadInput = new Regex(@"^S(\d+) := READ\((\d+|)\)\s*$"); //2 groups
            WriteOutput = new Regex(@"^WRITE\((\d+)\)\s*$"); //1 group

            SetMemoryToResult = new Regex(@"^S(\d+) := (.*)\s*$"); //2 groups
            SetPointerToResult = new Regex(@"^\[S(\d+)\] := (.*)\s*$"); //2 groups
            //Results for the two above
            ResultSet_Cell = new Regex(@"^S(\d+)\s*$"); //1 group
            ResultSet_CellOpCell = new Regex(@"^S(\d+) (\+|\-|\*|\/|%) S(\d+)\s*$"); //3 groups
            ResultSet_CellOpConstant = new Regex(@"^S(\d+) (\+|\-|\*|\/|%) (\d+)\s*$"); //3 groups
            ResultSet_ConstantOpCell = new Regex(@"^(\d+) (\+|\-|\*|\/|%) S(\d+)\s*$"); //3 groups
            ResultSet_Pointer = new Regex(@"^\[S(\d+)\]\s*$"); //1 group
            ResultSet_Constant = new Regex(@"^(\d+|-\d+)\s*$"); //1 group

            JumpToInstruction = new Regex(@"^goto :([0-z]*)\s*$"); //1 group
            JumpToLabel = new Regex(@"^:([0-z]*)\s*$"); //1 group
            IfJumpTo = new Regex(@"^if \((S|)((?:-|)\d+) (==|!=|<|>|<=|>=) (S|)((?:-|)\d+)\) goto :([0-z]*)\s*$"); // 6 groups



        }
    }
}
