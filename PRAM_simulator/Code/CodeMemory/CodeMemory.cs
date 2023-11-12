using PRAM_lib.Instruction.Other.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
