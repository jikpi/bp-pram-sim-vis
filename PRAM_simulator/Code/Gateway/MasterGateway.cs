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

        public int? IllegalMemoryReadIndex { get; private set; }
        public int? IllegalMemoryWriteIndex { get; private set; }
        public bool CREW { get; set; }

        private bool AccessingParallel;
        private List<bool> ReadAccessed;
        private List<bool> ReadSingleInstructionAccess;

        private List<int> WriteAccessed;
        private List<int> WriteSingleInstructionAccess;
        public void AccessingParallelStart()
        {
            AccessingParallel = true;
            ReadAccessed.Clear();
            ReadSingleInstructionAccess.Clear();
        }

        private bool CheckReadAccess()
        {
            if (ReadAccessed.Count < ReadSingleInstructionAccess.Count)
            {
                for (int i = ReadAccessed.Count; i < ReadSingleInstructionAccess.Count; i++)
                {
                    ReadAccessed.Add(false);
                }
            }

            for (int i = 0; i < ReadAccessed.Count; i++)
            {
                if (ReadAccessed[i] && ReadSingleInstructionAccess[i])
                {
                    IllegalMemoryReadIndex = i;
                    return false;
                }
                else
                {
                    ReadAccessed[i] = ReadSingleInstructionAccess[i];
                }
            }

            return true;
        }

        private bool CheckWriteAccess()
        {
            if (WriteAccessed.Count < WriteSingleInstructionAccess.Count)
            {
                for (int i = WriteAccessed.Count; i < WriteSingleInstructionAccess.Count; i++)
                {
                    WriteAccessed.Add(0);
                }
            }

            for (int i = 0; i < WriteAccessed.Count; i++)
            {
                if (WriteAccessed[i] > 1 && WriteSingleInstructionAccess[i] > 1)
                {
                    IllegalMemoryWriteIndex = i;
                    return false;
                }
                else
                {
                    WriteAccessed[i] = WriteSingleInstructionAccess[i];
                }
            }

            return true;
        }

        public void AccessingParallelInstructionEnd()
        {
            if (AccessingParallel)
            {
                if(!CREW)
                {
                    CheckReadAccess();
                }

                ReadSingleInstructionAccess.Clear();
                WriteSingleInstructionAccess.Clear();
            }
        }

        public void AccessingParallelEnd()
        {
            AccessingParallel = false;
            ReadAccessed.Clear();
            ReadSingleInstructionAccess.Clear();
            WriteAccessed.Clear();
            WriteSingleInstructionAccess.Clear();
        }

        public MasterGateway(MachineMemory refSharedMemory, IOMemory refInputMemory, IOMemory refOutputMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory, bool CREW)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;
            this.CREW = CREW;
            ParallelDoLaunch = delegate { };

            ReadAccessed = new List<bool>();
            ReadSingleInstructionAccess = new List<bool>();

            WriteAccessed = new List<int>();
            WriteSingleInstructionAccess = new List<int>();

            AccessingParallel = false;
            
            IllegalMemoryReadIndex = null;
            IllegalMemoryWriteIndex = null;
        }

        public void Refresh(MachineMemory refSharedMemory, IOMemory refInputMemory, IOMemory refOutputMemory, InstrPointer refInstructionPointer, Jumps.JumpMemory refJumpMemory, bool CREW)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;
            this.CREW = CREW;

            ReadAccessed = new List<bool>();
            ReadSingleInstructionAccess = new List<bool>();

            WriteAccessed = new List<int>();
            WriteSingleInstructionAccess = new List<int>();

            AccessingParallel = false;

            IllegalMemoryReadIndex = null;
            IllegalMemoryWriteIndex = null;
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
