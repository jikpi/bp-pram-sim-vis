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
        public bool CRXW { get; set; }
        public bool ERXW { get { return !CRXW; } set { CRXW = !value; } }
        public bool XRCW { get; set; }
        public bool XRXW { get { return !XRCW; } set { XRCW = !value; } }

        private bool AccessingParallel;
        private List<bool> ReadAccessed;
        private List<bool> ReadSingleInstructionAccess;

        private List<bool> WriteAccessed;
        private List<bool> WriteSingleInstructionAccess;

        public MasterGateway(MachineMemory refSharedMemory,
            IOMemory refInputMemory,
            IOMemory refOutputMemory,
            InstrPointer refInstructionPointer,
            Jumps.JumpMemory refJumpMemory,
            bool CREW)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;
            this.CRXW = CREW;
            ParallelDoLaunch = delegate { };

            ReadAccessed = new List<bool>();
            ReadSingleInstructionAccess = new List<bool>();

            WriteAccessed = new List<bool>();
            WriteSingleInstructionAccess = new List<bool>();

            AccessingParallel = false;

            IllegalMemoryReadIndex = null;
            IllegalMemoryWriteIndex = null;
        }

        public void Refresh(MachineMemory refSharedMemory,
            IOMemory refInputMemory,
            IOMemory refOutputMemory,
            InstrPointer refInstructionPointer,
            Jumps.JumpMemory refJumpMemory,
            bool CRXW = true,
            bool XRCW = true)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;
            this.CRXW = CRXW;
            this.XRCW = XRCW;

            ReadAccessed = new List<bool>();
            ReadSingleInstructionAccess = new List<bool>();

            WriteAccessed = new List<bool>();
            WriteSingleInstructionAccess = new List<bool>();

            AccessingParallel = false;

            IllegalMemoryReadIndex = null;
            IllegalMemoryWriteIndex = null;
        }

        // Parallel processors were launched, and will now be accessing memory
        public void AccessingParallelStart()
        {
            AccessingParallel = true;
            ReadAccessed.Clear();
            ReadSingleInstructionAccess.Clear();
        }


        //Check read access
        private bool CheckReadAccess()
        {
            if (ReadAccessed.Count < ReadSingleInstructionAccess.Count)
            {
                ReadAccessed.Capacity = ReadSingleInstructionAccess.Count + 1;
                for (int i = ReadAccessed.Count; i < ReadSingleInstructionAccess.Count; i++)
                {
                    ReadAccessed.Add(default);
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

        // Check write access
        private bool CheckWriteAccess()
        {
            if (WriteAccessed.Count < WriteSingleInstructionAccess.Count)
            {
                for (int i = WriteAccessed.Count; i < WriteSingleInstructionAccess.Count; i++)
                {
                    WriteAccessed.Add(default);
                }
            }

            for (int i = 0; i < WriteAccessed.Count; i++)
            {
                if (WriteAccessed[i] && WriteSingleInstructionAccess[i])
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

        private void NoteSingleReadAccess(int index)
        {
            if (AccessingParallel)
            {
                if (ReadSingleInstructionAccess.Count <= index)
                {
                    for (int i = ReadSingleInstructionAccess.Count; i <= index; i++)
                    {
                        ReadSingleInstructionAccess.Add(default);
                    }
                }

                ReadSingleInstructionAccess[index] = true;
            }
        }

        private void NoteSingleWriteAccess(int index)
        {
            if (AccessingParallel)
            {
                if (WriteSingleInstructionAccess.Count <= index)
                {
                    for (int i = WriteSingleInstructionAccess.Count; i <= index; i++)
                    {
                        WriteSingleInstructionAccess.Add(default);
                    }
                }

                WriteSingleInstructionAccess[index] = true;
            }
        }

        // A parallel processor has accessed memory, now check if it is legal
        public void AccessingParallelInstructionEnd()
        {
            if (AccessingParallel)
            {
                if (!CRXW)
                {
                    CheckReadAccess();
                }

                if (!XRCW)
                {
                    CheckWriteAccess();
                }

                ReadSingleInstructionAccess.Clear();
                WriteSingleInstructionAccess.Clear();
            }
        }

        // The parallel processors have all finished, and the machine is now in a not parallel state
        public void AccessingParallelEnd()
        {
            AccessingParallel = false;
            ReadAccessed.Clear();
            ReadSingleInstructionAccess.Clear();
            WriteAccessed.Clear();
            WriteSingleInstructionAccess.Clear();
        }

        // Primary read access
        public int Read(int cellIndex)
        {
            NoteSingleReadAccess(cellIndex);
            return SharedMemory.Read(cellIndex).Value;
        }

        // Primary write access
        public void Write(int cellIndex, int value)
        {
            NoteSingleWriteAccess(cellIndex);
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

        public int GetParallelIndex()
        {
            return 0;
        }
    }
}
