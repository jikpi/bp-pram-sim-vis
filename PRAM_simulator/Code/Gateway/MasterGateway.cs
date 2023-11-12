using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;

namespace PRAM_lib.Code.Gateway
{
    //A class that represents a gateway between a processor and a memory
    internal class MasterGateway : IGateway
    {
        private MachineMemory SharedMemory { get; set; }
        private IOMemory InputMemory { get; set; }
        private IOMemory OutputMemory { get; set; }
        private InstrPointer InstructionPointer { get; set; }
        private Jumps.JumpMemory JumpMemory { get; set; }
        public event Action ParallelDoLaunch;

        public MasterGateway(MachineMemory refSharedMemory, IOMemory refInputMemory, IOMemory refOutputMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;
            ParallelDoLaunch = delegate { };
        }

        public void Refresh(MachineMemory refSharedMemory, IOMemory refInputMemory, IOMemory refOutputMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;
        }

        public int Read(int cellIndex)
        {
            return SharedMemory.Read(cellIndex).Value;
        }

        public void Write(int cellIndex, int value)
        {
            SharedMemory.Write(cellIndex, value);
        }

        public int ReadInput(int cellIndex)
        {
            return InputMemory.Read(cellIndex).Value;
        }

        public int ReadInput()
        {
            return InputMemory.Read().Value;
        }

        public void WriteOutput(int value)
        {
            OutputMemory.Write(value);
        }

        public int GetJump(string label)
        {
            return JumpMemory.GetJump(label);
        }

        public void JumpTo(int index)
        {
            InstructionPointer.Value = index;
        }
        public void ParallelDoStart()
        {
            ParallelDoLaunch?.Invoke();
        }

        public object PRead(int memoryIndex)
        {
            //TODO implement
            return SharedMemory.Read(memoryIndex);
        }

        public void PWrite(int memoryIndex, int value)
        {
            //TODO implement
            SharedMemory.Write(memoryIndex, value);
        }

        public int GetParallelIndex()
        {
            return 0;
        }
    }
}
