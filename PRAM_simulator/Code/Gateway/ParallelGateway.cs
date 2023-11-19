using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;

namespace PRAM_lib.Code.Gateway
{
    internal class ParallelGateway : IGateway
    {
        internal MachineMemory? Memory { get; set; }
        internal InstrPointer? InstructionPointer { get; set; }
        internal Jumps.JumpMemory? jumpMemory { get; set; }
        internal int ParallelIndex { get; set; }

        public ParallelGateway(MachineMemory refMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory, int parallelIndex)
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
            if (Memory == null)
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
                throw new Exception("Debug exception: Jump memory in parallel gateway not set");
            }
            return jumpMemory.GetJump(label);
        }

        public void JumpTo(int index)
        {
            if (InstructionPointer == null)
            {
                throw new Exception("Debug exception: Instruction pointer not set");
            }
            InstructionPointer.Value = index;
        }

        public void ParallelDoStart()
        {
            throw new NotImplementedException();
        }

        public int GetParallelIndex()
        {
            return ParallelIndex;
        }
    }
}
