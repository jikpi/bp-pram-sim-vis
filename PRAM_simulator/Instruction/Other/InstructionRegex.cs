using System.Text.RegularExpressions;

namespace PRAM_lib.Instruction.Other
{
    internal class InstructionRegex
    {
        public Regex Comment { get; set; } //Not a real instruction
        public Regex ReadInput { get; set; }
        public Regex WriteOutput { get; set; }
        public Regex AssignResult { get; set; }
        public Regex ResultIs_Cell { get; set; }
        public Regex ResultIs_Cell2Cell { get; set; }
        public Regex ResultIs_Cell2Constant { get; set; }
        public Regex ResultIs_Constant2Cell { get; set; } //Supplemental regex for ResultIs_Cell2Constant alternative
        public Regex ResultIs_CellPointer { get; set; }
        public Regex ResultIs_Constant { get; set; }
        public Regex WritePointer { get; set; }
        public Regex JumpToInstruction { get; set; }
        public Regex JumpToLabel { get; set; } //Not a real instruction
        public Regex IfJumpTo { get; set; }

        public InstructionRegex()
        {
            Comment = new Regex(@"^#.*$"); //0 groups

            ReadInput = new Regex(@"^S(\d+) := READ\((\d+|)\)\s*$"); //2 groups
            WriteOutput = new Regex(@"^WRITE\((\d+)\)\s*$"); //1 group

            AssignResult = new Regex(@"^S(\d+) := (.*)\s*$"); //2 groups
            WritePointer = new Regex(@"^\[S(\d+)\] := (.*)\s*$"); //2 groups
            //Results for the two above
            ResultIs_Cell = new Regex(@"^S(\d+)\s*$"); //1 group
            ResultIs_Cell2Cell = new Regex(@"^S(\d+) (\+|\-|\*|\/|%) S(\d+)\s*$"); //3 groups
            ResultIs_Cell2Constant = new Regex(@"^S(\d+) (\+|\-|\*|\/|%) (\d+)\s*$"); //3 groups
            ResultIs_Constant2Cell = new Regex(@"^(\d+) (\+|\-|\*|\/|%) S(\d+)\s*$"); //3 groups
            ResultIs_CellPointer = new Regex(@"^\[S(\d+)\]\s*$"); //1 group
            ResultIs_Constant = new Regex(@"^(\d+|-\d+)\s*$"); //1 group

            JumpToInstruction = new Regex(@"^goto :([0-z]*)\s*$"); //1 group
            JumpToLabel = new Regex(@"^:([0-z]*)\s*$"); //1 group
            IfJumpTo = new Regex(@"^if \((S|)((?:-|)\d+) (==|!=|<|>|<=|>=) (S|)((?:-|)\d+)\) goto :([0-z]*)\s*$"); // 6 groups



        }
    }
}
