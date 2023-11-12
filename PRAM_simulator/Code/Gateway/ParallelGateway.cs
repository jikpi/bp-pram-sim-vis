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
    internal class ParallelGateway : IGateway
    {
        internal SharedMemory? Memory { get; set; }
        internal InstrPointer? InstructionPointer { get; set; }
        internal Jumps.JumpMemory? jumpMemory { get; set; }
        internal int ParallelIndex { get; set; }

        public ParallelGateway(SharedMemory refMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory, int parallelIndex)
        {
            Memory = refMemory;
            InstructionPointer = refInstructionPointer;
            this.jumpMemory = refJumpMemory;
            ParallelIndex = parallelIndex;
        }

        public ParallelGateway()
        {
        }

        public int Read(int index)
        {
            if(Memory == null)
            {
                throw new Exception("Memory is not set");
            }
            return Memory.Read(index).Value;
        }

        public void Write(int index, int value)
        {
            if (Memory == null)
            {
                throw new Exception("Memory is not set");
            }
            Memory.Write(index, value);
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
            if (jumpMemory == null)
            {
                throw new Exception("Jump memory is not set");
            }
            return jumpMemory.GetJump(label);
        }

        public void JumpTo(int index)
        {
            if (InstructionPointer == null)
            {
                throw new Exception("Instruction pointer is not set");
            }
            InstructionPointer.Value = index;
        }

        public void ParallelDoStart(int count)
        {
            throw new NotImplementedException();
        }

        public int GetParallelIndex()
        {
            return ParallelIndex;
        }
    }
}
