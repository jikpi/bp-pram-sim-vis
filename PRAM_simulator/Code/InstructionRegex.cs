using System.Text.RegularExpressions;

namespace PRAM_lib.Code
{
    public class InstructionRegex
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
        public Regex ParallelStart { get; set; }
        public Regex ParallelEnd { get; set; }
        public string ParallelCell; //Not a real instruction
        public Regex ResultSet_ParallelIndex { get; set; }
        public Regex IndirectMultiMemoryToResult { get; set; }


        public InstructionRegex()
        {
            Comment = new Regex(@"^#.*$"); //0 groups

            ReadInput = new Regex(@"^[A-Z](\d+) := READ\((\d+|)\)\s*$"); //2 groups

            WriteOutput = new Regex(@"^WRITE\((.*)\)\s*$"); //1 group
            SetMemoryToResult = new Regex(@"^([A-Z])(\d+) := (.*)\s*$"); //3 groups
            SetPointerToResult = new Regex(@"^\[([A-Z])(\d+)\] := (.*)\s*$"); //3 groups
            //Result sets for instructions that require it
            ResultSet_Cell = new Regex(@"^([A-Z])(\d+)\s*$"); //2 groups
            ResultSet_CellOpCell = new Regex(@"^([A-Z])(\d+) (\+|\-|\*|\/|%) ([A-Z])(\d+)\s*$"); //5 groups
            ResultSet_CellOpConstant = new Regex(@"^([A-Z])(\d+) (\+|\-|\*|\/|%) (\d+)\s*$"); //4 groups
            ResultSet_ConstantOpCell = new Regex(@"^(\d+) (\+|\-|\*|\/|%) ([A-Z])(\d+)\s*$"); //4 groups
            ResultSet_Pointer = new Regex(@"^\[([A-Z])(\d+)\]\s*$"); //2 groups
            ResultSet_Constant = new Regex(@"^(\d+|-\d+)\s*$"); //1 group

            JumpToInstruction = new Regex(@"^goto :([0-z]*)\s*$"); //1 group
            JumpToLabel = new Regex(@"^:([0-z]*)\s*$"); //1 group
            IfJumpTo = new Regex(@"^if \(([A-Z]|)((?:-|)\d+) (==|!=|<|>|<=|>=) ([A-Z]|)((?:-|)\d+)\) goto :([0-z]*)\s*$"); // 6 groups

            //Parallel instructions
            ParallelStart = new Regex(@"^pardo (\d+)\s*$"); //1 group
            ParallelEnd = new Regex(@"^parend\s*$"); //0 groups
            ParallelCell = "S";
            IndirectMultiMemoryToResult = new Regex(@"^([A-Z]){(.*)} := (.*)\s*$"); //3 groups

            //Result set for parallel instructions
            ResultSet_ParallelIndex = new Regex(@"^{i}\s*$"); //1 group






        }
    }
}
