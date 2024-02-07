using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Machine.Container;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;
using System.Reflection.Metadata.Ecma335;

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

        public GatewayParallelMemoryAccessInfo? IllegalMemoryReadIndex { get; private set; }
        public GatewayParallelMemoryAccessInfo? IllegalMemoryWriteIndex { get; private set; }
        public bool CRXW { get; set; }
        public bool ERXW { get { return !CRXW; } set { CRXW = !value; } }
        public bool XRCW { get; set; }
        public bool XREW { get { return !XRCW; } set { XRCW = !value; } }

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

        //Dictionary with memory cell as index, and a list of accesses to that cell by parallel processors
        public Dictionary<int, List<ParallelAccessInfo>> ParallelAccessCycle { get; private set; }
        //Hashset with memory cell index that was accessed, always for a single parallel machine
        private HashSet<int> ReadSingleInstructionAccess;
        private HashSet<int> WriteSingleInstructionAccess;

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

            ReadSingleInstructionAccess = new HashSet<int>();
            WriteSingleInstructionAccess = new HashSet<int>();
            ParallelAccessCycle = new Dictionary<int, List<ParallelAccessInfo>>();

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

            ParallelAccessCycle.Clear();
            ReadSingleInstructionAccess.Clear();
            WriteSingleInstructionAccess.Clear();

            AccessingParallel = false;

            IllegalMemoryReadIndex = null;
            IllegalMemoryWriteIndex = null;
        }

        // Parallel processors were launched, and will now be accessing memory
        public void AccessingParallelStart()
        {
            AccessingParallel = true;
            ParallelAccessCycle.Clear();
            ReadSingleInstructionAccess.Clear();
            WriteSingleInstructionAccess.Clear();
        }

        // Summarize the memory access of a single parallel processor
        private void SummarizeParallelMemoryAccess(int parallelMachineIndex, int relevantParallelMachineCodeIndex)
        {
            //Add read accesses
            foreach (int index in ReadSingleInstructionAccess)
            {
                if(ParallelAccessCycle.ContainsKey(index) == false)
                {
                    ParallelAccessCycle.Add(index, new List<ParallelAccessInfo>());
                }

                ParallelAccessCycle[index].Add(new ParallelAccessInfo(ParallelAccessType.Read, index, parallelMachineIndex, relevantParallelMachineCodeIndex));
            }

            //Add write accesses
            foreach (int index in WriteSingleInstructionAccess)
            {
                if(ParallelAccessCycle.ContainsKey(index) == false)
                {
                    ParallelAccessCycle.Add(index, new List<ParallelAccessInfo>());
                }

                ParallelAccessCycle[index].Add(new ParallelAccessInfo(ParallelAccessType.Write, index, parallelMachineIndex, relevantParallelMachineCodeIndex));
            }
        }

        private void NoteSingleReadAccess(int index)
        {
            if (AccessingParallel)
            {
                ReadSingleInstructionAccess.Add(index);
            }
        }

        private void NoteSingleWriteAccess(int index)
        {
            if (AccessingParallel)
            {
                WriteSingleInstructionAccess.Add(index);
            }
        }

        // A parallel processor has accessed memory, now check if it is legal
        public void SingleParallelInstructionExecuted(int parallelMachineIndex, int parallelMachineRelevantCodeIndex)
        {
            if (AccessingParallel)
            {
                SummarizeParallelMemoryAccess(parallelMachineIndex, parallelMachineRelevantCodeIndex);

                ReadSingleInstructionAccess.Clear();
                WriteSingleInstructionAccess.Clear();
            }
        }

        public void AccessingParallelStep()
        {
            if (AccessingParallel)
            {
                ParallelAccessCycle.Clear();
                ReadSingleInstructionAccess.Clear();
                WriteSingleInstructionAccess.Clear();
            }
        }

        // The parallel processors have all finished, and the machine is now in a not parallel state
        public void AccessingParallelEnd()
        {
            AccessingParallel = false;
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
