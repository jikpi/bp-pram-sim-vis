﻿using PRAM_lib.Code.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PRAM_lib.Instruction.Other.Interface
{
    internal interface IInstruction
    {
        public void Execute(MasterGateway gateway); //A method that executes an instruction
        public int InstructionPointerIndex { get; set; } //An index of an instruction in a code memory
        public int CodeInstructionLineIndex { get; set; } //An index of the line in the code editor
    }
}
