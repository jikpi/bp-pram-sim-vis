using PRAM_lib.Memory;
using PRAM_lib.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.Gateway
{
    //A class that represents a gateway between a processor and a memory
    internal class Gateway
    {
        internal SharedMemory SharedMemory { get; set; }
        internal IOMemory InputMemory { get; set; }
        internal IOMemory OutputMemory { get; set; }
        internal InstructionPointer InstructionPointer { get; set; }
        internal Jumps.JumpMemory jumpMemory { get; set; }

        public Gateway(SharedMemory sharedMemory, IOMemory inputMemory, IOMemory outputMemory, InstructionPointer instructionPointer, Jumps.JumpMemory jumpMemory)
        {
            SharedMemory = sharedMemory;
            InputMemory = inputMemory;
            OutputMemory = outputMemory;
            InstructionPointer = instructionPointer;
            this.jumpMemory = jumpMemory;
        }
    }
}
