using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.Gateway
{
    //A class that represents a gateway between a processor and a memory
    internal class MasterGateway
    {
        internal SharedMemory SharedMemory { get; set; }
        internal IOMemory InputMemory { get; set; }
        internal IOMemory OutputMemory { get; set; }
        internal InstrPointer InstructionPointer { get; set; }
        internal Jumps.JumpMemory jumpMemory { get; set; }

        public MasterGateway(SharedMemory refSharedMemory, IOMemory refInputMemory, IOMemory refOutputMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            this.jumpMemory = refJumpMemory;
        }
    }
}
