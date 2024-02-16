/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;

namespace PRAM_lib.Code.Gateway
{
    /// <summary>
    /// A class that represents a gateway between a processor and a memory, used for parallel processors
    /// </summary>
    internal class ParallelGateway : IGateway
    {
        internal MachineMemory? Memory { get; set; }
        internal InstrPointer? InstructionPointer { get; set; }
        internal Jumps.JumpMemory? jumpMemory { get; set; }
        internal int ParallelIndex { get; set; }
        public event Action HaltNotify;

        public ParallelGateway(MachineMemory refMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory, int parallelIndex)
        {
            Memory = refMemory;
            InstructionPointer = refInstructionPointer;
            this.jumpMemory = refJumpMemory;
            ParallelIndex = parallelIndex;
            HaltNotify = delegate { };
        }

        public ParallelGateway()
        {
            HaltNotify = delegate { };
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

        public void ParallelDoStart(int count, int index)
        {
            throw new NotImplementedException();
        }

        public int GetParallelIndex()
        {
            return ParallelIndex;
        }

        public void Halt()
        {
            HaltNotify?.Invoke();
        }
    }
}
