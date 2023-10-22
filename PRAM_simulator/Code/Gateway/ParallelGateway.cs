using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.Gateway
{
    internal class ParallelGateway : IGatewayAccessLocal
    {
        internal SharedMemory Memory { get; set; }
        internal InstrPointer InstructionPointer { get; set; }
        internal Jumps.JumpMemory jumpMemory { get; set; }

        public ParallelGateway(SharedMemory refMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory)
        {
            Memory = refMemory;
            InstructionPointer = refInstructionPointer;
            this.jumpMemory = refJumpMemory;
        }

        public int Read(int index)
        {
            throw new NotImplementedException();
        }

        public void Write(int index, int value)
        {
            throw new NotImplementedException();
        }

        public int ReadInput(int index)
        {
            throw new NotImplementedException();
        }

        public int ReadInput()
        {
            throw new NotImplementedException();
        }

        public void WriteOutput(int value)
        {
            throw new NotImplementedException();
        }

        public int GetJump(string label)
        {
            throw new NotImplementedException();
        }

        public void JumpTo(int index)
        {
            throw new NotImplementedException();
        }

        public void ParallelDoStart(int count)
        {
            throw new NotImplementedException();
        }
    }
}
