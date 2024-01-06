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
        public event Action HaltNotify;

        public int? IllegalMemoryReadIndex { get; private set; }
        public int? IllegalMemoryWriteIndex { get; private set; }
        public bool CRXW { get; set; }
        public bool ERXW { get { return !CRXW; } set { CRXW = !value; } }
        public bool XRCW { get; set; }
        public bool XRXW { get { return !XRCW; } set { XRCW = !value; } }

        private bool AccessingParallel;

        internal class GatewayParallelMemoryAccessInfo
        {
            public bool IsAccessed { get; set; }
            public List<int> AccessingParallelMachineIndex { get; set; }

            public GatewayParallelMemoryAccessInfo()
            {
                IsAccessed = false;
                AccessingParallelMachineIndex = new List<int>();
            }
        }
        public List<GatewayParallelMemoryAccessInfo> ReadAccessed { get; private set; }
        private List<bool> ReadSingleInstructionAccess;

        public List<GatewayParallelMemoryAccessInfo> WriteAccessed { get; private set; }
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
            HaltNotify = delegate { };

            ReadAccessed = new List<GatewayParallelMemoryAccessInfo>();
            ReadSingleInstructionAccess = new List<bool>();

            WriteAccessed = new List<GatewayParallelMemoryAccessInfo>();
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

            ReadAccessed.Clear();
            ReadSingleInstructionAccess = new List<bool>();

            WriteAccessed.Clear();
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
        private bool CheckReadAccess(int parallelMachineIndex)
        {
            if (ReadAccessed.Count < ReadSingleInstructionAccess.Count)
            {
                ReadAccessed.Capacity = ReadSingleInstructionAccess.Count + 1;
                for (int i = ReadAccessed.Count; i < ReadSingleInstructionAccess.Count; i++)
                {
                    ReadAccessed.Add(new GatewayParallelMemoryAccessInfo());
                }
            }

            for (int i = 0; i < ReadAccessed.Count; i++)
            {
                if (ReadAccessed[i].IsAccessed && ReadSingleInstructionAccess[i])
                {
                    IllegalMemoryReadIndex = i;
                    ReadAccessed[i].AccessingParallelMachineIndex.Add(parallelMachineIndex);
                    return false;
                }
                else
                {
                    // If the index is out of range, then it is not accessed, since no entry was made for it in the last execution
                    if (i >= ReadAccessed.Count)
                    {
                        continue;
                    }

                    ReadAccessed[i].IsAccessed = ReadSingleInstructionAccess[i];
                    ReadAccessed[i].AccessingParallelMachineIndex.Add(parallelMachineIndex);
                }
            }

            return true;
        }

        // Check write access
        private bool CheckWriteAccess(int parallelMachineIndex)
        {
            if (WriteAccessed.Count < WriteSingleInstructionAccess.Count)
            {
                for (int i = WriteAccessed.Count; i < WriteSingleInstructionAccess.Count; i++)
                {
                    WriteAccessed.Add(new GatewayParallelMemoryAccessInfo());
                }
            }

            for (int i = 0; i < WriteAccessed.Count; i++)
            {
                if (WriteAccessed[i].IsAccessed && WriteSingleInstructionAccess[i])
                {
                    IllegalMemoryWriteIndex = i;
                    WriteAccessed[i].AccessingParallelMachineIndex.Add(parallelMachineIndex);
                    return false;
                }
                else
                {
                    if (i >= WriteAccessed.Count)
                    {
                        continue;
                    }

                    WriteAccessed[i].IsAccessed = WriteSingleInstructionAccess[i];
                    WriteAccessed[i].AccessingParallelMachineIndex.Add(parallelMachineIndex);
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
        public void SingleParallelInstructionExecuted(int parallelMachineIndex)
        {
            if (AccessingParallel)
            {
                if (!CRXW)
                {
                    CheckReadAccess(parallelMachineIndex);
                }

                if (!XRCW)
                {
                    CheckWriteAccess(parallelMachineIndex);
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

        public void Halt()
        {
            HaltNotify?.Invoke();
        }
    }
}
