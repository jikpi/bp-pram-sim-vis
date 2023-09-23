using PRAM_lib.Instruction.Master_Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other
{
    internal class InstructionRegex
    {
        public Regex ReadInput { get; set; }
        public Regex WriteOutput { get; set; }
        public Regex AssignResult { get; set; }
        public Regex ResultIs_Cell { get; set; }
        public Regex ResultIs_Cell2Cell { get; set; }
        public Regex ResultIs_Cell2Constant { get; set; }
        public Regex ResultIs_CellPointer { get; set; }
        public Regex ResultIs_Constant { get; set; }


        public InstructionRegex()
        {
            ReadInput = new Regex(@"^S(\d+) := READ\((\d+|)\)\s*$"); //2 groups
            WriteOutput = new Regex(@"^WRITE\((\d+)\)\s*$"); //1 group

            AssignResult = new Regex(@"^S(\d+) := (.*)\s*$"); //2 groups
            ResultIs_Cell = new Regex(@"^S(\d+)\s*$"); //1 group
            ResultIs_Cell2Cell = new Regex(@"^S(\d+) (\+|\-|\*|\/|%) S(\d+)\s*$"); //3 groups
            ResultIs_Cell2Constant = new Regex(@"^S(\d+) (\+|\-|\*|\/|%) (\d+)\s*$"); //3 groups
            ResultIs_CellPointer = new Regex(@"^\[S(\d+)\]\s*$"); //1 group
            ResultIs_Constant = new Regex(@"^(\d+|-\d+)\s*$"); //1 group


        }
    }
}
